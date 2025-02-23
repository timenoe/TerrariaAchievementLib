using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Achievements;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace TerrariaAchievementLib.Systems
{
    /// <summary>
    /// Used to modify custom achievement icons
    /// </summary>
    public class IconSystem : ModSystem
    {
        /// <summary>
        /// Maximum number of achievement icons in a single texture
        /// </summary>
        public const int MaxTextureIconSize = 120;


        /// <summary>
        /// Achievement icons indexes relative to the texture<br/>
        /// Texture size cannot exceed vanilla, so this allows for reference of multiple textures
        /// </summary>
        private readonly Dictionary<string, int> _relativeIconIndexes = [];

        /// <summary>
        /// Achievement icon textures
        /// </summary>
        private readonly List<Asset<Texture2D>> _textures = [];

        /// <summary>
        /// Current absolute achievement icon index
        /// </summary>
        private int _absoluteIconIndex = 0;


        public override void OnModLoad()
        {
            if (Main.dedServ)
                return;

            On_InGamePopups.AchievementUnlockedPopup.ctor += On_AchievementUnlockedPopup_ctor;
            On_UIAchievementListItem.ctor += On_UIAchievementListItem_ctor;
        }

        public override void OnModUnload()
        {
            if (Main.dedServ)
                return;

            On_InGamePopups.AchievementUnlockedPopup.ctor -= On_AchievementUnlockedPopup_ctor;
            On_UIAchievementListItem.ctor -= On_UIAchievementListItem_ctor;
        }

        /// <summary>
        /// Load the achievement icon textures from the provided paths
        /// </summary>
        public void LoadAchTextures(List<string> iconTexturePaths)
        {
            foreach (string path in iconTexturePaths)
                _textures.Add(ModContent.Request<Texture2D>(path));
        }

        /// <summary>
        /// Register a new achievement with one condition to the in-game list
        /// </summary>
        /// <param name="internalName">Internal achievement name</param>
        public void RegisterAchievementIcon(string internalName)
        {
            // Achievement texture size cannot exceed vanilla, so cache true index
            _relativeIconIndexes[internalName] = _absoluteIconIndex;
            Main.Achievements.RegisterIconIndex(internalName, _absoluteIconIndex++ % MaxTextureIconSize);

            if (_absoluteIconIndex / MaxTextureIconSize > _textures.Count - 1)
                throw new Exception($"Only {_textures.Count} achievement textures were defined. Achievement {_absoluteIconIndex + 1} is out of range (One texture can only hold {MaxTextureIconSize} icons).");
        }

        /// <summary>
        /// Detour to replace the vanilla achievement texture when a AchievementUnlockedPopup is created
        /// </summary>
        /// <param name="orig">Original ctor method</param>
        /// <param name="self">AchievementUnlockedPopup being created</param>
        /// <param name="achievement">Achievement to base the pop-up on</param>
        private void On_AchievementUnlockedPopup_ctor(On_InGamePopups.AchievementUnlockedPopup.orig_ctor orig, InGamePopups.AchievementUnlockedPopup self, Achievement achievement)
        {
            orig.Invoke(self, achievement);

            // Only modify custom achievement textures
            if (!AchievementSystem.Instance.IsCustomAchievement(achievement.Name))
                return;

            typeof(InGamePopups.AchievementUnlockedPopup).GetField("_achievementTexture", AchievementSystem.ReflectionFlags)?.SetValue(self, _textures[_relativeIconIndexes[achievement.Name] / MaxTextureIconSize]);
        }

        /// <summary>
        /// Detour to replace the vanilla achievement texture when a UIAchievementListItem is created
        /// </summary>
        /// <param name="orig">Original ctor method</param>
        /// <param name="self">UIAchievementListItem being created</param>
        /// <param name="achievement">Achievement to base the list item on</param>
        /// <param name="largeForOtherLanguages">True if large for other languages</param>
        private void On_UIAchievementListItem_ctor(On_UIAchievementListItem.orig_ctor orig, UIAchievementListItem self, Achievement achievement, bool largeForOtherLanguages)
        {
            orig.Invoke(self, achievement, largeForOtherLanguages);

            // Only modify custom achievement textures
            if (!AchievementSystem.Instance.IsCustomAchievement(achievement.Name))
                return;

            // Get icon frame and modifier
            Rectangle frame = (Rectangle)typeof(UIAchievementListItem).GetField("_iconFrame", AchievementSystem.ReflectionFlags)?.GetValue(self);
            bool large = (bool)typeof(UIAchievementListItem).GetField("_large", AchievementSystem.ReflectionFlags)?.GetValue(self);

            // Apply new achievement texture using frame and modifier
            FieldInfo info = typeof(UIAchievementListItem).GetField("_achievementIcon", AchievementSystem.ReflectionFlags);
            UIImageFramed icon = (UIImageFramed)info?.GetValue(self);
            icon.Remove();
            icon = new(_textures[_relativeIconIndexes[achievement.Name] / MaxTextureIconSize], frame);
            icon.Left.Set(large.ToInt() * 6, 0f);
            icon.Top.Set(large.ToInt() * 12, 0f);
            info?.SetValue(self, icon);
            self.Append(icon);

            // Bring border back on top
            info = typeof(UIAchievementListItem).GetField("_achievementIconBorders", AchievementSystem.ReflectionFlags);
            if (info == null)
                return;
            UIImage border = (UIImage)info.GetValue(self);
            border.Remove();
            self.Append(border);
        }
    }
}
