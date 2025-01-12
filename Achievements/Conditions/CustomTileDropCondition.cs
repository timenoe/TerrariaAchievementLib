using System.Collections.Generic;
using Terraria;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for item(s) to be dropped by tiles
    /// </summary>
    public class CustomTileDropCondition : CustomIdCondition
    {
        /// <summary>
        /// Base condition identifier (used for saving to achievements.dat)
        /// </summary>
        private const string CustomName = "CUSTOM_TILE_DROP";


        /// <summary>
        /// True if the class has been hooked to the helper event
        /// </summary>
        private static bool _isHooked;

        /// <summary>
        /// IDs and the conditions that are listening for them to be triggered
        /// </summary>
        protected static readonly Dictionary<int, List<CustomTileDropCondition>> _listeners = [];


        /// <summary>
        /// Creates a condition that listens for the item to be dropped
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Item ID to listen for</param>
        private CustomTileDropCondition(ConditionRequirements reqs, int id) : base(CustomName, reqs, [id]) => Listen();

        /// <summary>
        /// Creates a condition that listens for any of the items to be dropped
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        private CustomTileDropCondition(ConditionRequirements reqs, int[] ids) : base(CustomName, reqs, ids) => Listen();


        /// <summary>
        /// Helper to create a condition that listens for the item to be dropped
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Item ID to listen for</param>
        /// <returns>Item drop achievement condition</returns>
        public static CustomAchievementCondition Drop(ConditionRequirements reqs, int id) => new CustomTileDropCondition(reqs, id);

        /// <summary>
        /// Helper to create a condition that listens for any of the items to be dropped
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        /// <returns>Item drop achievement condition</returns>
        public static CustomAchievementCondition DropAny(ConditionRequirements reqs, params int[] ids) => new CustomTileDropCondition(reqs, ids);

        /// <summary>
        /// Helper to create a condition that listens for all of the items to be dropped
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        /// <returns>Item drop achievement conditions</returns>
        public static List<CustomAchievementCondition> DropAll(ConditionRequirements reqs, params int[] ids)
        {
            List<CustomAchievementCondition> conditions = [];
            foreach (var id in ids)
                conditions.Add(new CustomTileDropCondition(reqs, id));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when an item is dropped
        /// </summary>
        /// <param name="id">Item ID that was dropped</param>
        private void CustomAchievementsHelper_OnTileDrop(int id)
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
                CustomAchievementsHelper.OnTileDrop += CustomAchievementsHelper_OnTileDrop;
                _isHooked = true;
            }

            ListenForId(this, _listeners);
        }
    }
}
