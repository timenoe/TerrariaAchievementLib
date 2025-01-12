using System.Collections.Generic;
using Terraria;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for fish to be caught
    /// </summary>
    public class FishCatchCondition : AchievementIdCondition
    {
        /// <summary>
        /// Base condition identifier (used for saving to achievements.dat)
        /// </summary>
        private const string CustomName = "CUSTOM_FISH_CATCH";


        /// <summary>
        /// True if the class has been hooked to the helper event
        /// </summary>
        private static bool _isHooked;

        /// <summary>
        /// IDs and the conditions that are listening for them to be triggered
        /// </summary>
        protected static readonly Dictionary<int, List<FishCatchCondition>> _listeners = [];


        /// <summary>
        /// Creates a condition that listens for the fish to be caught
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Item ID to listen for</param>
        private FishCatchCondition(ConditionRequirements reqs, int id) : base(CustomName, reqs, [id]) => Listen();

        /// <summary>
        /// Creates a condition that listens for any of the fish to be caught
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        private FishCatchCondition(ConditionRequirements reqs, int[] ids) : base(CustomName, reqs, ids) => Listen();


        /// <summary>
        /// Helper to create a condition that listens for the fish to be caught
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Item ID to listen for</param>
        /// <returns>Fish catch achievement condition</returns>
        public static AchCondition Catch(ConditionRequirements reqs, int id) => new FishCatchCondition(reqs, id);

        /// <summary>
        /// Helper to create a condition that listens for any of the fish to be caught
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        /// <returns>Fish catch achievement condition</returns>
        public static AchCondition CatchAny(ConditionRequirements reqs, params int[] ids) => new FishCatchCondition(reqs, ids);

        /// <summary>
        /// Helper to create a condition that listens for all of the fish to be caught
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        /// <returns>Fish catch achievement conditions</returns>
        public static List<AchCondition> CatchAll(ConditionRequirements reqs, params int[] ids)
        {
            List<AchCondition> conditions = [];
            foreach (var id in ids)
                conditions.Add(new FishCatchCondition(reqs, id));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when a fish is caught
        /// </summary>
        /// <param name="player">Player that caught the fish</param>
        /// <param name="id">Item ID that was caught</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void CustomAchievementsHelper_OnFishCatch(Player player, int id)
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
                AchHelper.OnFishCatch += CustomAchievementsHelper_OnFishCatch;
                _isHooked = true;
            }

            ListenForId(this, _listeners);
        }
    }
}
