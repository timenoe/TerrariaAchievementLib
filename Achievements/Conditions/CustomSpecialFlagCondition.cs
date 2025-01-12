using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Achievements;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for special flag(s) to be set
    /// </summary>
    public class CustomSpecialFlagCondition : CustomIdCondition
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
        /// Creates a condition that listens for the special flag to be set
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Special flag ID to listen for</param>
        private CustomSpecialFlagCondition(ConditionRequirements reqs, int id) : base(CustomName, reqs, [id]) { }

        /// <summary>
        /// Creates a condition that listens for any of the special flags to be set
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Special flag IDs to listen for</param>
        private CustomSpecialFlagCondition(ConditionRequirements reqs, int[] ids) : base(CustomName, reqs, ids) { }


        protected override void HookIdEvent()
        {
            if (!_isHooked)
            {
                CustomAchievementsHelper.OnSpecialFlag += CustomAchievementsHelper_OnSpecialFlag;
                _isHooked = true;
            }
        }

        /// <summary>
        /// Helper to create a condition that listens for the special flag to be set
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Special flag ID to listen for</param>
        /// <returns>Special flag set achievement condition</returns>
        public static CustomAchievementCondition Set(ConditionRequirements reqs, int id) => new CustomSpecialFlagCondition(reqs, id);

        /// <summary>
        /// Helper to create a condition that listens for any of the special flags to be set
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Special flag IDs to listen for</param>
        /// <returns>Special flag set achievement condition</returns>
        public static CustomAchievementCondition SetAny(ConditionRequirements reqs, params int[] ids) => new CustomSpecialFlagCondition(reqs, ids);

        /// <summary>
        /// Helper to create a condition that listens for all of the special flags to be set
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Special flag IDs to listen for</param>
        /// <returns>Special flag set achievement conditions</returns>
        public static List<CustomAchievementCondition> SetAll(ConditionRequirements reqs, params int[] ids)
        {
            List<CustomAchievementCondition> conditions = [];
            foreach (var id in ids)
                conditions.Add(new CustomSpecialFlagCondition(reqs, id));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when a special flag is set
        /// </summary>
        /// <param name="player">Player that set the special flag</param>
        /// <param name="id">Flag ID that was set</param>
        private static void CustomAchievementsHelper_OnSpecialFlag(Player player, int id)
        {
            if (!IsListeningForId(id, out var conditions))
                return;

            foreach (var condition in conditions)
            {
                if (condition.Reqs.Pass(player))
                    condition.Complete();
            }
        }
    }
}
