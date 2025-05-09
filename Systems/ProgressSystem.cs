﻿using System.Collections.Generic;
using Terraria;
using Terraria.Achievements;
using Terraria.ModLoader;
using TerrariaAchievementLib.Tools;

namespace TerrariaAchievementLib.Systems
{
    /// <summary>
    /// Used to display progress notifications
    /// </summary>
    public class ProgressSystem : ModSystem
    {
        /// <summary>
        /// True if progress notifications are enabled
        /// </summary>
        private static bool _enabled;

        /// <summary>
        /// True if progress notifications are enabled
        /// </summary>
        private static bool _enabledVanilla;


        public override void OnModLoad()
        {
            if (Main.dedServ)
                return;

            On_Achievement.OnConditionComplete += On_Achievement_OnConditionComplete;
        }

        public override void OnModUnload()
        {
            if (Main.dedServ)
                return;

            On_Achievement.OnConditionComplete -= On_Achievement_OnConditionComplete;
        }

        /// <summary>
        /// Enables progress notifications for achievements<br/>
        /// Only one mod in a group should enable vanilla notifications to prevent duplicates
        /// </summary>
        public static void SetEnabled(bool enabled, bool includeVanilla = false)
        {
            _enabled = enabled;
            _enabledVanilla = enabled && includeVanilla;
        }

        /// <summary>
        /// Detour to display a progress notification for tracked achievements when a condition completes
        /// </summary>
        /// <param name="orig">Original OnConditionComplete method</param>
        /// <param name="self">Achievement associated with the condition</param>
        /// <param name="condition">Achievement condition that completed</param>
        private void On_Achievement_OnConditionComplete(On_Achievement.orig_OnConditionComplete orig, Achievement self, AchievementCondition condition)
        {
            orig.Invoke(self, condition);

            // Only show notifications for achievements tracking indivudual elements
            IAchievementTracker tracker = (IAchievementTracker)typeof(Achievement).GetField("_tracker", AchievementSystem.ReflectionFlags)?.GetValue(self);
            if (tracker == null || tracker is not ConditionsCompletedTracker)
                return;

            bool progressCustom = _enabled && AchievementSystem.Instance.IsCustomAchievement(self.Name);
            bool progressVanilla = _enabledVanilla && AchievementTool.IsVanillaAchievement(self);

            // Only notifications for custom or vanilla achievements if applicable
            if (progressCustom || progressVanilla)
            {
                Dictionary<string, AchievementCondition> conditions = (Dictionary<string, AchievementCondition>)typeof(Achievement).GetField("_conditions", AchievementSystem.ReflectionFlags)?.GetValue(self);
                int completedConditionsCount = (int)typeof(Achievement).GetField("_completedCount", AchievementSystem.ReflectionFlags)?.GetValue(self);

                if (completedConditionsCount < conditions.Count)
                    LogTool.ChatLog($"You made progress on [a:{self.Name}]: {completedConditionsCount}/{conditions.Count}");
            }
        }
    }
}
