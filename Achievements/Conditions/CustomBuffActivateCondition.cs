using System.Collections.Generic;
using Terraria;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for buff(s) to be activated
    /// </summary>
    public class CustomBuffActivateCondition : CustomIdCondition
    {
        /// <summary>
        /// Base condition identifier (used for saving to achievements.dat)
        /// </summary>
        private const string CustomName = "CUSTOM_BUFF_ACTIVATE";


        /// <summary>
        /// True if the class has been hooked to the helper event
        /// </summary>
        private static bool _isHooked;

        /// <summary>
        /// IDs and the conditions that are listening for them to be triggered
        /// </summary>
        protected static readonly Dictionary<int, List<CustomBuffActivateCondition>> _listeners = [];


        /// <summary>
        /// Creates a condition that listens for the buff to be activated
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Buff ID to listen for</param>
        private CustomBuffActivateCondition(ConditionRequirements reqs, int id) : base(CustomName, reqs, [id]) => Listen();

        /// <summary>
        /// Creates a condition that listens for any of the buffs to be activated
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Buff IDs to listen for</param>
        private CustomBuffActivateCondition(ConditionRequirements reqs, int[] ids) : base(CustomName, reqs, ids) => Listen();

        /// <summary>
        /// Helper to create a condition that listens for the buff to be activated
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Buff ID to listen for</param>
        /// <returns>Buff activate achievement condition</returns>
        public static CustomAchievementCondition Activate(ConditionRequirements reqs, int id) => new CustomBuffActivateCondition(reqs, id);

        /// <summary>
        /// Helper to create a condition that listens for any of the buffs to be activated
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Buff IDs to listen for</param>
        /// <returns>Buff activate achievement condition</returns>
        public static CustomAchievementCondition ActivateAny(ConditionRequirements reqs, params int[] ids) => new CustomBuffActivateCondition(reqs, ids);

        /// <summary>
        /// Helper to create a condition that listens for all of the buffs to be activated
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Buff IDs to listen for</param>
        /// <returns>Buff activate achievement conditions</returns>
        public static List<CustomAchievementCondition> ActivateAll(ConditionRequirements reqs, params int[] ids)
        {
            List<CustomAchievementCondition> conditions = [];
            foreach (var id in ids)
                conditions.Add(new CustomBuffActivateCondition(reqs, id));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when a buff is activated
        /// </summary>
        /// <param name="player">Player that activated the buff</param>
        /// <param name="id">Buff ID that was activated</param>
        private static void NewAchievementsHelper_OnBuffActivation(Player player, int id)
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
                CustomAchievementsHelper.OnBuffActivation += NewAchievementsHelper_OnBuffActivation;
                _isHooked = true;
            }

            ListenForId(this, _listeners);
        }
    }
}
