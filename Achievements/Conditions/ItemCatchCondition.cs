using System.Collections.Generic;
using Terraria;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for item(s) to be caught
    /// </summary>
    public class ItemCatchCondition : AchIdCondition
    {
        /// <summary>
        /// Base condition identifier (used for saving to achievements.dat)
        /// </summary>
        private const string CustomName = "CUSTOM_ITEM_CATCH";


        /// <summary>
        /// True if the class has been hooked to the helper event
        /// </summary>
        private static bool _isHooked;

        /// <summary>
        /// IDs and the conditions that are listening for them to be triggered
        /// </summary>
        private static readonly Dictionary<int, List<ItemCatchCondition>> _listeners = [];


        /// <summary>
        /// Creates a condition that listens for the item to be caught
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Item ID to listen for</param>
        private ItemCatchCondition(ConditionReqs reqs, int id) : base(CustomName, reqs, [id]) => Listen(this);

        /// <summary>
        /// Creates a condition that listens for any of the item to be caught
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        private ItemCatchCondition(ConditionReqs reqs, int[] ids) : base(CustomName, reqs, ids) => Listen(this);


        /// <summary>
        /// Helper to create a condition that listens for the item to be caught
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Item ID to listen for</param>
        /// <returns>Item catch achievement condition</returns>
        public static CustomAchievementCondition Catch(ConditionReqs reqs, int id) => new ItemCatchCondition(reqs, id);

        /// <summary>
        /// Helper to create a condition that listens for any of the items to be caught
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        /// <returns>Item catch achievement condition</returns>
        public static CustomAchievementCondition CatchAny(ConditionReqs reqs, params int[] ids) => new ItemCatchCondition(reqs, ids);

        /// <summary>
        /// Helper to create a condition that listens for all of the items to be caught
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        /// <returns>Item catch achievement conditions</returns>
        public static List<CustomAchievementCondition> CatchAll(ConditionReqs reqs, params int[] ids)
        {
            List<CustomAchievementCondition> conditions = [];
            foreach (var id in ids)
                conditions.Add(new ItemCatchCondition(reqs, id));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when an item is caught
        /// </summary>
        /// <param name="player">Player that caught the item</param>
        /// <param name="id">Item ID that was caught</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private static void CustomAchievementsHelper_OnItemCatch(Player player, int id)
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
        private static void Listen(ItemCatchCondition condition)
        {
            if (!_isHooked)
            {
                CustomAchievementsHelper.OnItemCatch += CustomAchievementsHelper_OnItemCatch;
                _isHooked = true;
            }

            ListenForIds(condition, condition.Ids, _listeners);
        }
    }
}
