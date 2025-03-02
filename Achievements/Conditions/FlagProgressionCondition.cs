using System.Collections.Generic;
using Terraria;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for progression flag(s) to be set
    /// </summary>
    public class FlagProgressionCondition : AchIdCondition
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
        private static readonly Dictionary<int, List<FlagProgressionCondition>> _listeners = [];


        /// <summary>
        /// Creates a condition that listens for the progression flag to be set
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Progression flag ID to listen for</param>
        private FlagProgressionCondition(ConditionReqs reqs, int id) : base(CustomName, reqs, [id]) => Listen(this);

        /// <summary>
        /// Creates a condition that listens for any of the progression flags to be set
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Progression flag IDs to listen for</param>
        private FlagProgressionCondition(ConditionReqs reqs, int[] ids) : base(CustomName, reqs, ids) => Listen(this);


        /// <summary>
        /// Helper to create a condition that listens for the progression flag to be set
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Progression flag ID to listen for</param>
        /// <returns>Progression flag set achievement condition</returns>
        public static CustomAchievementCondition Set(ConditionReqs reqs, int id) => new FlagProgressionCondition(reqs, id);

        /// <summary>
        /// Helper to create a condition that listens for any of the progression flags to be set
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Progression flag IDs to listen for</param>
        /// <returns>Progression flag set achievement condition</returns>
        public static CustomAchievementCondition SetAny(ConditionReqs reqs, params int[] ids) => new FlagProgressionCondition(reqs, ids);

        /// <summary>
        /// Helper to create a condition that listens for all of the progression flags to be set
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Progression flag IDs to listen for</param>
        /// <returns>Progression flag set achievement conditions</returns>
        public static List<CustomAchievementCondition> SetAll(ConditionReqs reqs, params int[] ids)
        {
            List<CustomAchievementCondition> conditions = [];
            foreach (var id in ids)
                conditions.Add(new FlagProgressionCondition(reqs, id));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when a progression flag is set
        /// </summary>
        /// <param name="player">Player that raised the progression flag</param>
        /// <param name="id">Flag ID that was set</param>
        private static void CustomAchievementsHelper_OnFlagProgression(Player player, int id)
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
        /// <param name="condition">Achievement condition</param>
        /// </summary>
        private static void Listen(FlagProgressionCondition condition)
        {
            if (!_isHooked)
            {
                CustomAchievementsHelper.OnFlagProgression += CustomAchievementsHelper_OnFlagProgression;
                _isHooked = true;
            }

            ListenForIds(condition, condition.Ids, _listeners);
        }
    }
}
