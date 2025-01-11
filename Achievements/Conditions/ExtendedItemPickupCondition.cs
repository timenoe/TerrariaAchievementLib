using System.Collections.Generic;
using Terraria;
using Terraria.Achievements;
using Terraria.GameContent.Achievements;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Achievement condition that is satisfied when an item is picked up
    /// </summary>
    public class ExtendedItemPickupCondition : AchievementCondition
    {
        /// <summary>
        /// Base condition identifier (used for saving to achievements.dat)
        /// </summary>
        private const string Identifier = "EXTENDED_ITEM_PICKUP";


        /// <summary>
        /// Condition requirements that must be met
        /// </summary>
        public readonly ConditionRequirements Reqs;

        /// <summary>
        /// Item IDs that need to be picked up to satisfy the condition
        /// </summary>
        public readonly int[] ItemIds;

        /// <summary>
        /// Item IDs and the conditions that are listening for them to be picked up
        /// </summary>
        private static readonly Dictionary<int, List<ExtendedItemPickupCondition>> _listeners = [];

        /// <summary>
        /// True if the class has been hooked to the helper event
        /// </summary>
        private static bool _isHooked;


        /// <summary>
        /// Creates a condition that listens for the item to be picked up
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="itemId">Item ID to listen for</param>
        private ExtendedItemPickupCondition(ConditionRequirements reqs, int itemId) : base($"{Identifier}_{reqs.Identifier}-{itemId}")
        {
            Reqs = reqs;
            ItemIds = [itemId];
            ListenForItemPickup(this);
        }

        /// <summary>
        /// Creates a condition that listens for any of the items to be picked up
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="itemIds">Item IDs to listen for</param>
        private ExtendedItemPickupCondition(ConditionRequirements reqs, int[] itemIds) : base($"{Identifier}_{reqs.Identifier}-{string.Join(",", itemIds)}")
        {
            Reqs = reqs;
            ItemIds = itemIds;
            ListenForItemPickup(this);
        }


        /// <summary>
        /// Helper to create a condition that listens for the item to be picked up
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="itemId">Item ID to listen for</param>
        /// <returns>Item pickup achievement condition</returns>
        public static AchievementCondition Pickup(ConditionRequirements reqs, int itemId) => new ExtendedItemPickupCondition(reqs, itemId);

        /// <summary>
        /// Helper to create a condition that listens for any of the items to be picked up
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="itemIds">Item IDs to listen for</param>
        /// <returns>Item pickup achievement condition</returns>
        public static AchievementCondition PickupAny(ConditionRequirements reqs, params int[] itemIds) => new ExtendedItemPickupCondition(reqs, itemIds);

        /// <summary>
        /// Helper to create a condition that listens for all of the items to be picked up
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="itemIds">Item IDs to listen for</param>
        /// <returns>Item pickup achievement conditions</returns>
        public static AchievementCondition[] PickupAll(ConditionRequirements reqs, params int[] itemIds)
        {
            AchievementCondition[] array = new AchievementCondition[itemIds.Length];

            for (int i = 0; i < itemIds.Length; i++)
                array[i] = new ExtendedItemPickupCondition(reqs, itemIds[i]);

            return array;
        }

        /// <summary>
        /// Add condition to the list of item pickup listeners
        /// </summary>
        /// <param name="condition">Condition to add to the list of item pickup listeners</param>
        private static void ListenForItemPickup(ExtendedItemPickupCondition condition)
        {
            // Hook the helper event only once for the class
            if (!_isHooked)
            {
                AchievementsHelper.OnItemPickup += AchievementsHelper_OnItemPickup;
                _isHooked = true;
            }

            // Loop through all items in the condition
            foreach (int itemId in condition.ItemIds)
            {
                // Create empty list of listeners for the item if there are none
                if (!_listeners.TryGetValue(itemId, out List<ExtendedItemPickupCondition> conditions))
                {
                    conditions = [];
                    _listeners[itemId] = conditions;
                }

                // Add the current condition to the listeners for this item
                conditions.Add(condition);
            }
        }

        /// <summary>
        /// Hook that is called when an item is picked up
        /// </summary>
        /// <param name="player">Player that picked up the item</param>
        /// <param name="itemId">Item ID that was picked up</param>
        private static void AchievementsHelper_OnItemPickup(Player player, short itemId, int count)
        {
            if (!_listeners.TryGetValue(itemId, out var conditions))
                return;

            foreach (var condition in conditions)
            {
                if (condition.Reqs.Pass(player))
                    condition.Complete();
            }
        }
    }
}
