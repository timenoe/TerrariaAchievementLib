﻿using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Achievements;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for progression flag(s) to be set
    /// </summary>
    public class CustomProgressionFlagCondition : CustomIdCondition
    {
        /// <summary>
        /// Base condition identifier (used for saving to achievements.dat)
        /// </summary>
        private const string CustomName = "CUSTOM_PROGRESSION_FLAG";


        /// <summary>
        /// True if the class has been hooked to the helper event
        /// </summary>
        private static bool _isHooked;

        /// <summary>
        /// IDs and the conditions that are listening for them to be triggered
        /// </summary>
        protected static readonly Dictionary<int, List<CustomProgressionFlagCondition>> _listeners = [];


        /// <summary>
        /// Creates a condition that listens for the progression flag to be set
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Progression flag ID to listen for</param>
        private CustomProgressionFlagCondition(ConditionRequirements reqs, int id) : base(CustomName, reqs, [id]) => Listen();

        /// <summary>
        /// Creates a condition that listens for any of the progression flags to be set
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Progression flag IDs to listen for</param>
        private CustomProgressionFlagCondition(ConditionRequirements reqs, int[] ids) : base(CustomName, reqs, ids) => Listen();


        /// <summary>
        /// Helper to create a condition that listens for the progression flag to be set
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Progression flag ID to listen for</param>
        /// <returns>Progression flag set achievement condition</returns>
        public static CustomAchievementCondition Set(ConditionRequirements reqs, int id) => new CustomProgressionFlagCondition(reqs, id);

        /// <summary>
        /// Helper to create a condition that listens for any of the progression flags to be set
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Progression flag IDs to listen for</param>
        /// <returns>Progression flag set achievement condition</returns>
        public static CustomAchievementCondition SetAny(ConditionRequirements reqs, params int[] ids) => new CustomProgressionFlagCondition(reqs, ids);

        /// <summary>
        /// Helper to create a condition that listens for all of the progression flags to be set
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Progression flag IDs to listen for</param>
        /// <returns>Progression flag set achievement conditions</returns>
        public static List<CustomAchievementCondition> SetAll(ConditionRequirements reqs, params int[] ids)
        {
            List<CustomAchievementCondition> conditions = [];
            foreach (var id in ids)
                conditions.Add(new CustomProgressionFlagCondition(reqs, id));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when a progression flag is set
        /// </summary>
        /// <param name="id">Flag ID that was set</param>
        private static void AchievementsHelper_OnProgressionEvent(int id)
        {
            if (!IsListeningForId(id, _listeners, out var conditions))
                return;

            foreach (var condition in conditions)
            {
                if (condition.Reqs.Pass(Main.LocalPlayer))
                    condition.Complete();
            }
        }

        /// <summary>
        /// Listen for events so the condition can be completed
        /// </summary>
        private void Listen()
        {
            if (!_isHooked)
            {
                AchievementsHelper.OnProgressionEvent += AchievementsHelper_OnProgressionEvent;
                _isHooked = true;
            }

            ListenForId(this, _listeners);
        }
    }
}
