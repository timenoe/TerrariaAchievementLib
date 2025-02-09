﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Achievements;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using TerrariaAchievementLib.Achievements;

namespace TerrariaAchievementLib.Systems
{
    /// <summary>
    /// Used to add new achievements to the in-game list
    /// </summary>
    public abstract class AchievementSystem : ModSystem
    {
        /// <summary>
        /// Maximum number of achievement icons in a texture
        /// </summary>
        private const int MaxAchievementIcons = 120;

        /// <summary>
        /// Flags to use during reflection
        /// </summary>
        private const BindingFlags ReflectionFlags = BindingFlags.NonPublic | BindingFlags.Instance;


        /// <summary>
        /// Achievement icon texture
        /// </summary>
        private readonly List<Asset<Texture2D>> _textures = [];

        /// <summary>
        /// True achievement icon index<br/>
        /// Texture size cannot exceed vanilla, so this allows for reference of multiple textures
        /// </summary>
        private readonly Dictionary<string, int> _iconIndexes = [];

        /// <summary>
        /// Current achievement icon index in the texture
        /// </summary>
        private int _iconIndex = 0;


        /// <summary>
        /// Unique achievement name header
        /// </summary>
        protected abstract string Identifier { get; }

        /// <summary>
        /// Achievement icon texture path
        /// </summary>
        protected abstract List<string> TexturePaths { get; }

        /// <summary>
        /// Achievement icon texture
        /// </summary>
        public List<Asset<Texture2D>> Textures => _textures;


        public override void OnModLoad()
        {
            if (Main.dedServ)
                return;

            RegisterAchievements();
            LoadAchTextures();
            LoadSaveData();

            On_AchievementsHelper.HandleOnEquip += On_AchievementsHelper_HandleOnEquip;
            On_AchievementsHelper.HandleSpecialEvent += On_AchievementsHelper_HandleSpecialEvent;
            On_UIAchievementListItem.ctor += On_UIAchievementListItem_ctor;
            On_InGamePopups.AchievementUnlockedPopup.ctor += AchievementUnlockedPopup_ctor;
        }

        public override void OnModUnload()
        {
            if (Main.dedServ)
                return;

            UnregisterAchievements();
            On_AchievementsHelper.HandleOnEquip -= On_AchievementsHelper_HandleOnEquip;
            On_AchievementsHelper.HandleSpecialEvent -= On_AchievementsHelper_HandleSpecialEvent;
            On_UIAchievementListItem.ctor -= On_UIAchievementListItem_ctor;
            On_InGamePopups.AchievementUnlockedPopup.ctor -= AchievementUnlockedPopup_ctor;
        }

        /// <summary>
        /// Register new achievements to the in-game list<br/>
        /// Involves consecutive calls to RegisterNewAchievement
        /// </summary>
        protected abstract void RegisterAchievements();


        /// <summary>
        /// Register a new achievement with one condition to the in-game list
        /// </summary>
        /// <param name="name">Achievement name</param>
        /// <param name="cond">Achievement condition</param>
        /// <param name="cat">Achievement category</param>
        protected void RegisterAchievement(string name, AchCondition cond, AchievementCategory cat)
        {
            // Add unique achievement header to the name if needed
            if (!name.StartsWith(Identifier))
                name = $"{Identifier}_{name}";

            Achievement ach = new(name);
            ach.AddCondition(cond);

            Main.Achievements.Register(ach);
            Main.Achievements.RegisterAchievementCategory(name, cat);

            // Achievement texture size cannot exceed vanilla, so cache true index
            _iconIndexes[name] = _iconIndex;
            Main.Achievements.RegisterIconIndex(name, _iconIndex++ % MaxAchievementIcons);

            if (_iconIndex / MaxAchievementIcons > TexturePaths.Count - 1)
                throw new System.Exception($"Only {TexturePaths.Count} achievement textures were defined. Achievement {_iconIndex + 1} is out of range (One texture can only hold 120 icons).");
        }

        /// <summary>
        /// Register a new achievement with multiple conditions to the in-game list
        /// </summary>
        /// <param name="name">Achievement name</param>
        /// <param name="conds">Achievement conditions</param>
        /// <param name="track">True if tracking completed conditions total in the in-game menu</param>
        /// <param name="cat">Achievement category</param>
        protected void RegisterAchievement(string name, List<AchCondition> conds, bool track, AchievementCategory cat)
        {
            // Add unique achievement header to the name if needed
            if (!name.StartsWith(Identifier))
                name = $"{Identifier}_{name}";

            Achievement ach = new(name);
            foreach (var condition in conds)
                ach.AddCondition(condition);

            if (track)
                ach.UseConditionsCompletedTracker();

            Main.Achievements.Register(ach);
            Main.Achievements.RegisterAchievementCategory(name, cat);

            // Achievement texture size cannot exceed vanilla, so cache true index
            _iconIndexes[name] = _iconIndex;
            Main.Achievements.RegisterIconIndex(name, _iconIndex++ % MaxAchievementIcons);

            if (_iconIndex / MaxAchievementIcons > TexturePaths.Count - 1)
                throw new System.Exception($"Only {TexturePaths.Count} achievement textures were defined. Achievement {_iconIndex + 1} is out of range (One texture can only hold 120 icons).");
        }

        /// <summary>
        /// Returns true if an achievement was added from this system<br/>
        /// Checks the achievement name for the unique header<br/>
        /// Used to not modify other achievement textures ctor hooks
        /// </summary>
        /// <param name="ach">Achievement to check</param>
        /// <returns>True if the achievement is new to this system</returns>
        private bool IsMyAchievement(Achievement ach) => ach.Name.StartsWith(Identifier);

        /// <summary>
        /// Unregister all achievements that were added from this system
        /// </summary>
        private void UnregisterAchievements()
        {
            FieldInfo info = typeof(AchievementManager).GetField("_achievements", ReflectionFlags);
            if (info == null)
                return;

            Dictionary<string, Achievement> achs = (Dictionary<string, Achievement>)info.GetValue(Main.Achievements);
            if (achs == null)
                return;

            info = typeof(AchievementManager).GetField("_achievementIconIndexes", ReflectionFlags);
            if (info == null)
                return;

            Dictionary<string, int> icons = (Dictionary<string, int>)info.GetValue(Main.Achievements);
            if (icons == null)
                return;

            Dictionary<string, Achievement> achs_copy = new(achs);
            foreach (KeyValuePair<string, Achievement> ach in achs_copy)
            {
                if (!IsMyAchievement(ach.Value))
                    continue;

                achs.Remove(ach.Key);
                icons.Remove(ach.Key);
            }
        }

        /// <summary>
        /// Load the achievement texture from the provided abstract path
        /// </summary>
        private void LoadAchTextures()
        {
            foreach (var path in TexturePaths)
                _textures.Add(ModContent.Request<Texture2D>(path));
        }

        /// <summary>
        /// Load any save data from achievements.dat if applicable
        /// </summary>
        private static void LoadSaveData()
        {
            FieldInfo info = typeof(AchievementManager).GetField("_achievements", ReflectionFlags);
            if (info == null)
                return;
            Dictionary<string, Achievement> achievements = (Dictionary<string, Achievement>)info.GetValue(Main.Achievements);

            // Clear existing achievement progress before loading again
            // Bug in the vanilla code causes issues during two consecutive loads
            foreach (KeyValuePair<string, Achievement> achievement in achievements)
                achievement.Value.ClearProgress();

            Main.Achievements.Load();
        }

        /// <summary>
        /// Detour to notify achievement conditions when an item has been equipped
        /// </summary>
        /// <param name="orig">Original HandleOnEquip method</param>
        /// <param name="player">Player that equipped the item</param>
        /// <param name="item">Item ID that was equipped</param>
        /// <param name="context">Item slot context ID</param>
        private void On_AchievementsHelper_HandleOnEquip(On_AchievementsHelper.orig_HandleOnEquip orig, Player player, Item item, int context)
        {
            orig.Invoke(player, item, context);

            // Apply custom context ID if applicable
            if (item.wingSlot > 0)
                context = AchData.ItemSlotContextID.EquipWings;

            // Notify with just the item slot context ID for equipping anything in that slot
            AchHelper.NotifyItemEquip(player, context, ItemID.None);
            // Notify with the item slot context ID and the specific item ID
            AchHelper.NotifyItemEquip(player, context, item.type);
        }

        /// <summary>
        /// Detour to notify achievement conditions when a special flag is raised<br/><br/>
        /// The vanilla game didn't do this, and instead opted for manually named flags<br/>
        /// in the conditions, even though special flag IDs are used elsewhere in the code
        /// </summary>
        /// <param name="orig">Original HandleSpecialEvent method</param>
        /// <param name="player">Player that raised the special event</param>
        /// <param name="eventID">Special event ID</param>
        private void On_AchievementsHelper_HandleSpecialEvent(On_AchievementsHelper.orig_HandleSpecialEvent orig, Player player, int eventID)
        {
            orig.Invoke(player, eventID);

            AchHelper.NotifyFlagSpecial(player, eventID);
        }

        /// <summary>
        /// Detour to replace the vanilla achievement texture when a UIAchievementListItem is created
        /// </summary>
        /// <param name="orig">Original ctor</param>
        /// <param name="self">UIAchievementListItem being created</param>
        /// <param name="achievement">Achievement to base the list item on</param>
        /// <param name="largeForOtherLanguages">True if large for other languages</param>
        private void On_UIAchievementListItem_ctor(On_UIAchievementListItem.orig_ctor orig, UIAchievementListItem self, Achievement achievement, bool largeForOtherLanguages)
        {
            orig.Invoke(self, achievement, largeForOtherLanguages);

            if (!IsMyAchievement(achievement))
                return;

            // Get icon frame
            FieldInfo info = typeof(UIAchievementListItem).GetField("_iconFrame", ReflectionFlags);
            if (info == null)
                return;
            Rectangle frame = (Rectangle)info.GetValue(self);

            // Get large modifier
            info = typeof(UIAchievementListItem).GetField("_large", ReflectionFlags);
            if (info == null)
                return;
            bool large = (bool)info.GetValue(self);

            // Apply new achievement texture using frame and modifier
            info = typeof(UIAchievementListItem).GetField("_achievementIcon", ReflectionFlags);
            if (info == null)
                return;
            UIImageFramed icon = (UIImageFramed)info.GetValue(self);
            icon.Remove();
            icon = new(Textures[_iconIndexes[achievement.Name] / MaxAchievementIcons], frame);
            icon.Left.Set(large.ToInt() * 6, 0f);
            icon.Top.Set(large.ToInt() * 12, 0f);
            info.SetValue(self, icon);
            self.Append(icon);

            // Bring border back on top
            info = typeof(UIAchievementListItem).GetField("_achievementIconBorders", ReflectionFlags);
            if (info == null)
                return;
            UIImage border = (UIImage)info.GetValue(self);
            border.Remove();
            self.Append(border);
        }

        /// <summary>
        /// Detour to replace the vanilla achievement texture when a AchievementUnlockedPopup is created
        /// </summary>
        /// <param name="orig">Original ctor</param>
        /// <param name="self">AchievementUnlockedPopup being created</param>
        /// <param name="achievement">Achievement to base the pop-up on</param>
        private void AchievementUnlockedPopup_ctor(On_InGamePopups.AchievementUnlockedPopup.orig_ctor orig, InGamePopups.AchievementUnlockedPopup self, Achievement achievement)
        {
            orig.Invoke(self, achievement);

            // Don't modify vanilla achievement textures
            if (!IsMyAchievement(achievement))
                return;

            FieldInfo info = typeof(InGamePopups.AchievementUnlockedPopup).GetField("_achievementTexture", ReflectionFlags);
            if (info == null)
                return;
            info.SetValue(self, Textures[_iconIndexes[achievement.Name] / MaxAchievementIcons]);
        }
    }
}
