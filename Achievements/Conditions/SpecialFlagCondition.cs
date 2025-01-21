﻿using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Achievements;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for special flag(s) to be set
    /// </summary>
    public class SpecialFlagCondition : AchIdCondition
    {
        /// <summary>
        /// Base condition identifier (used for saving to achievements.dat)
        /// </summary>
        private const string CustomName = "CUSTOM_SPECIAL_FLAG";


        /// <summary>
        /// True if the class has been hooked to the helper event
        /// </summary>
        private static bool _isHooked;

        /// <summary>
        /// IDs and the conditions that are listening for them to be triggered
        /// </summary>
        private static readonly Dictionary<int, List<SpecialFlagCondition>> _listeners = [];


        /// <summary>
        /// Creates a condition that listens for the special flag to be set
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Special flag ID to listen for</param>
        private SpecialFlagCondition(ConditionReqs reqs, int id) : base(CustomName, reqs, [id]) => Listen();

        /// <summary>
        /// Creates a condition that listens for any of the special flags to be set
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Special flag IDs to listen for</param>
        private SpecialFlagCondition(ConditionReqs reqs, int[] ids) : base(CustomName, reqs, ids) => Listen();


        /// <summary>
        /// Helper to create a condition that listens for the special flag to be set
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Special flag ID to listen for</param>
        /// <returns>Special flag set achievement condition</returns>
        public static AchCondition Set(ConditionReqs reqs, int id) => new SpecialFlagCondition(reqs, id);

        /// <summary>
        /// Helper to create a condition that listens for any of the special flags to be set
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Special flag IDs to listen for</param>
        /// <returns>Special flag set achievement condition</returns>
        public static AchCondition SetAny(ConditionReqs reqs, params int[] ids) => new SpecialFlagCondition(reqs, ids);

        /// <summary>
        /// Helper to create a condition that listens for all of the special flags to be set
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Special flag IDs to listen for</param>
        /// <returns>Special flag set achievement conditions</returns>
        public static List<AchCondition> SetAll(ConditionReqs reqs, params int[] ids)
        {
            List<AchCondition> conditions = [];
            foreach (var id in ids)
                conditions.Add(new SpecialFlagCondition(reqs, id));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when a special flag is set
        /// </summary>
        /// <param name="player">Player that set the special flag</param>
        /// <param name="id">Flag ID that was set</param>
        private void AchHelper_OnFlagSpecial(Player player, int id)
        {
            if (!IsListeningForId(id, _listeners, out var conditions))
                return;

            foreach (var condition in conditions)
            {
                if (condition.Reqs.Pass(player))
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
                AchHelper.OnFlagSpecial += AchHelper_OnFlagSpecial;
                _isHooked = true;
            }

            ListenForId(this, _listeners);
        }
    }
}
