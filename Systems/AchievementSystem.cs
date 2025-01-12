using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Achievements;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.UI.Elements;
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
        /// Flags to use during reflection
        /// </summary>
        private const BindingFlags ReflectionFlags = BindingFlags.NonPublic | BindingFlags.Instance;


        /// <summary>
        /// Current achievement icon index in the texture
        /// </summary>
        private int _iconIndex = 0;

        /// <summary>
        /// Achievement icon texture
        /// </summary>
        private Asset<Texture2D> _texture;


        /// <summary>
        /// Unique achievement name header
        /// </summary>
        protected abstract string Identifier { get; }

        /// <summary>
        /// Achievement icon texture path
        /// </summary>
        protected abstract string TexturePath { get; }

        /// <summary>
        /// Achievement icon texture
        /// </summary>
        public Asset<Texture2D> Texture => _texture;


        public override void OnModLoad()
        {
            RegisterNewAchievements();
            LoadNewTexture();
            LoadSaveData();

            On_AchievementsHelper.HandleSpecialEvent += On_AchievementsHelper_HandleSpecialEvent;
            On_UIAchievementListItem.ctor += On_UIAchievementListItem_ctor;
            On_InGamePopups.AchievementUnlockedPopup.ctor += AchievementUnlockedPopup_ctor;
        }

        public override void OnModUnload()
        {
            UnregisterNewAchievements();

            On_UIAchievementListItem.ctor -= On_UIAchievementListItem_ctor;
            On_InGamePopups.AchievementUnlockedPopup.ctor -= AchievementUnlockedPopup_ctor;
        }

        /// <summary>
        /// Register new achievements to the in-game list<br/>
        /// Involves consecutive calls to RegisterNewAchievement
        /// </summary>
        protected abstract void RegisterNewAchievements();


        /// <summary>
        /// Register a new achievement to the in-game list
        /// </summary>
        /// <param name="name">Achievement name</param>
        /// <param name="conds">Achievement conditions</param>
        /// <param name="cat">Achievement category</param>
        protected void RegisterNewAchievement(string name, List<AchievementCondition> conds, AchievementCategory cat, bool useTracker)
        {
            // Add unique achievement header to the name if needed
            if (!name.Contains(Identifier))
                name = $"{Identifier}{name}";

            Achievement ach = new(name);
            if (!IsNewAchievement(ach))
                return;

            foreach (AchievementCondition condition in conds)
                ach.AddCondition(condition);

            if (useTracker)
                ach.UseConditionsCompletedTracker();

            Main.Achievements.Register(ach);
            Main.Achievements.RegisterAchievementCategory(name, cat);
            Main.Achievements.RegisterIconIndex(name, _iconIndex++);
        }

        /// <summary>
        /// Returns true if an achievement was added from this system<br/>
        /// Checks the achievement name for the unique header<br/>
        /// Used to not modify other achievement textures ctor hooks
        /// </summary>
        /// <param name="ach">Achievement to check</param>
        /// <returns>True if the achievement is new to this system</returns>
        private bool IsNewAchievement(Achievement ach) => ach.Name.Contains(Identifier);

        /// <summary>
        /// Unregister all achievements that were added from this system
        /// </summary>
        private void UnregisterNewAchievements()
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
                if (!IsNewAchievement(ach.Value))
                    continue;

                achs.Remove(ach.Key);
                icons.Remove(ach.Key);
            }
        }

        /// <summary>
        /// Load the achievement texture from the provided abstract path
        /// </summary>
        private void LoadNewTexture() => _texture = ModContent.Request<Texture2D>(TexturePath);

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
        /// Detour to notify achievement conditions when a special flag is raised<br/><br/>
        /// The vanilla game didn't to this, and instead opted for manually named flags<br/>
        /// in the conditions, even though special flag IDs are used elsewhere in the code
        /// </summary>
        /// <param name="orig">Original HandleSpecialEvent</param>
        /// <param name="player">Player that raised the special event</param>
        /// <param name="eventID">Special event ID</param>
        private void On_AchievementsHelper_HandleSpecialEvent(On_AchievementsHelper.orig_HandleSpecialEvent orig, Player player, int eventID)
        {
            orig.Invoke(player, eventID);
            CustomAchievementsHelper.NotifySpecialFlag(player, eventID);
        }

        /// <summary>
        /// Detour to replace the vanilla achievement texture when a UIAchievementListItem is created
        /// </summary>
        /// <param name="orig">Original ctor</param>
        /// <param name="self">UIAchievementListItem being created</param>
        /// <param name="achievement">achievement ctor parameter</param>
        /// <param name="largeForOtherLanguages">largeForOtherLanguages ctor parameter</param>
        private void On_UIAchievementListItem_ctor(On_UIAchievementListItem.orig_ctor orig, UIAchievementListItem self, Achievement achievement, bool largeForOtherLanguages)
        {
            orig.Invoke(self, achievement, largeForOtherLanguages);

            if (!IsNewAchievement(achievement))
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
            icon = new(Texture, frame);
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
        /// <param name="achievement">achievement ctor parameter</param>
        private void AchievementUnlockedPopup_ctor(On_InGamePopups.AchievementUnlockedPopup.orig_ctor orig, InGamePopups.AchievementUnlockedPopup self, Achievement achievement)
        {
            orig.Invoke(self, achievement);

            // Don't modify vanilla achievement textures
            if (!IsNewAchievement(achievement))
                return;

            FieldInfo info = typeof(InGamePopups.AchievementUnlockedPopup).GetField("_achievementTexture", ReflectionFlags);
            if (info == null)
                return;
            info.SetValue(self, Texture);
        }
    }
}
