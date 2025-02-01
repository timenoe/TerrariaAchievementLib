using System.Collections.Generic;
using Terraria;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for item(s) to be opened from a grab bag
    /// </summary>
    public class ItemOpenCondition : AchIdCondition
    {
        /// <summary>
        /// Base condition identifier (used for saving to achievements.dat)
        /// </summary>
        private const string CustomName = "CUSTOM_ITEM_OPEN";


        /// <summary>
        /// True if the class has been hooked to the helper event
        /// </summary>
        private static bool _isHooked;

        /// <summary>
        /// IDs and the conditions that are listening for them to be triggered
        /// </summary>
        private static readonly Dictionary<int, List<ItemOpenCondition>> _listeners = [];

        /// <summary>
        /// Bag ID that should give the item(s)
        /// </summary>
        private int _bagId;


        /// <summary>
        /// Creates a condition that listens for an item to be opened from a grab bag
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="bagId">Grab bag ID that gifts the item</param>
        /// <param name="itemId">Item ID to listen for</param>
        private ItemOpenCondition(ConditionReqs reqs, short bagId, int itemId) : base($"{CustomName}_{string.Join(",", bagId)}", reqs, [itemId]) => Listen(this, bagId);

        /// <summary>
        /// Creates a condition that listens for any of the items to be opened from a grab bag
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="bagId">Grab bag ID that gifts the items</param>
        /// <param name="itemIds">Item IDs to listen for</param>
        private ItemOpenCondition(ConditionReqs reqs, short bagId, int[] itemIds) : base($"{CustomName}_{string.Join(",", bagId)}", reqs, itemIds) => Listen(this, bagId);


        /// <summary>
        /// Helper to create a condition that listens for an item to be opened from a grab bag
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="bagId">Grab bag ID that gifts the items</param>
        /// <param name="itemID">Item ID to listen for</param>
        /// <returns>Item open achievement condition</returns>
        public static AchCondition Open(ConditionReqs reqs, short bagId, int itemID) => new ItemOpenCondition(reqs, bagId, itemID);

        /// <summary>
        /// Helper to create a condition that listens for any of the items to be opened from a grab bag
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="bagId">Grab bag ID that gifts the items</param>
        /// <param name="itemIds">Item IDs to listen for</param>
        /// <returns>Item open achievement condition</returns>
        public static AchCondition OpenAny(ConditionReqs reqs, short bagId, params int[] itemIds) => new ItemOpenCondition(reqs, bagId, itemIds);

        /// <summary>
        /// Helper to create a condition that listens for all of the items to be opened from a grab bag
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="bagId">Grab bag ID that gifts the items</param>
        /// <param name="itemIds">Item IDs to listen for</param>
        /// <returns>Item open achievement conditions</returns>
        public static List<AchCondition> OpenAll(ConditionReqs reqs, short bagId, params int[] itemIds)
        {
            List<AchCondition> conditions = [];
            foreach (var itemId in itemIds)
                conditions.Add(new ItemOpenCondition(reqs, bagId, itemId));
            return conditions;
        }


        /// <summary>
        /// Hook that is called when an item is opened from a grab bag
        /// </summary>
        /// <param name="player">Player that opened the grab bag</param>
        /// <param name="bagId">Grab bag ID that was opened</param>
        /// <param name="itemId">Item ID that was received</param>
        private static void AchHelper_OnItemOpen(Player player, int bagId, int itemId)
        {
            if (!IsListeningForId(itemId, _listeners, out var conditions))
                return;

            foreach (var condition in conditions)
            {
                // Check Bag ID when applicable
                if (condition._bagId != 0 && bagId != condition._bagId)
                    continue;

                if (condition.Reqs.Pass(player))
                    condition.Complete();
            }
        }

        /// <summary>
        /// Listen for events so the condition can be completed
        /// </summary>
        /// <param name="bagId">Grab bag ID that should give the item</param>
        private static void Listen(ItemOpenCondition condition, int bagId)
        {
            if (!_isHooked)
            {
                AchHelper.OnItemOpen += AchHelper_OnItemOpen;
                _isHooked = true;
            }

            ListenForIds(condition, condition.Ids, _listeners);
            condition._bagId = bagId;
        }
    }
}
