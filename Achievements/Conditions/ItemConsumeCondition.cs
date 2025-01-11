﻿using System.Collections.Generic;
using Terraria;
using Terraria.Achievements;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Achievement condition that is satisfied when an item is consumed
    /// </summary>
    public class ItemConsumeCondition : AchievementCondition
    {
        /// <summary>
        /// Base condition identifier (used for saving to achievements.dat)
        /// </summary>
        private const string Identifier = "ITEM_CONSUME";


        /// <summary>
        /// Condition requirements that must be met
        /// </summary>
        public readonly ConditionRequirements Reqs;

        /// <summary>
        /// Item IDs that need to be consumed to satisfy the condition
        /// </summary>
        public readonly int[] ItemIds;

        /// <summary>
        /// Item IDs and the conditions that are listening for them to be consumed
        /// </summary>
        private static readonly Dictionary<int, List<ItemConsumeCondition>> _listeners = [];

        /// <summary>
        /// True if the class has been hooked to the helper event
        /// </summary>
        private static bool _isHooked;


        /// <summary>
        /// Creates a condition that listens for the item to be consumed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="itemId">Item ID to listen for</param>
        private ItemConsumeCondition(ConditionRequirements reqs, int itemId) : base($"{Identifier}_{reqs.Identifier}-{itemId}")
        {
            Reqs = reqs;
            ItemIds = [itemId];
            ListenForItemConsume(this);
        }

        /// <summary>
        /// Creates a condition that listens for any of the items to be consumed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="itemIds">Item IDs to listen for</param>
        private ItemConsumeCondition(ConditionRequirements reqs, int[] itemIds) : base($"{Identifier}_{reqs.Identifier}-{string.Join(",", itemIds)}")
        {
            Reqs = reqs;
            ItemIds = itemIds;
            ListenForItemConsume(this);
        }


        /// <summary>
        /// Helper to create a condition that listens for the item to be consumed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="itemId">Item ID to listen for</param>
        /// <returns>Item consume achievement condition</returns>
        public static AchievementCondition Consume(ConditionRequirements reqs, int itemId) => new ItemConsumeCondition(reqs, itemId);

        /// <summary>
        /// Helper to create a condition that listens for any of the items to be consumed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="itemIds">Item IDs to listen for</param>
        /// <returns>Item consume achievement condition</returns>
        public static AchievementCondition ConsumeAny(ConditionRequirements reqs, params int[] itemIds) => new ItemConsumeCondition(reqs, itemIds);

        /// <summary>
        /// Helper to create a condition that listens for all of the items to be consumed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="itemIds">Item IDs to listen for</param>
        /// <returns>Item consume achievement conditions</returns>
        public static AchievementCondition[] ConsumeAll(ConditionRequirements reqs, params int[] itemIds)
        {
            AchievementCondition[] array = new AchievementCondition[itemIds.Length];

            for (int i = 0; i < itemIds.Length; i++)
                array[i] = new ItemConsumeCondition(reqs, itemIds[i]);

            return array;
        }

        /// <summary>
        /// Add condition to the list of item consume listeners
        /// </summary>
        /// <param name="condition">Condition to add to the list of item consume listeners</param>
        private static void ListenForItemConsume(ItemConsumeCondition condition)
        {
            // Hook the helper event only once for the class
            if (!_isHooked)
            {
                NewAchievementsHelper.OnItemConsume += NewAchievementsHelper_OnItemConsume;
                _isHooked = true;
            }

            // Loop through all items in the condition
            foreach (int itemId in condition.ItemIds)
            {
                // Create empty list of listeners for the item if there are none
                if (!_listeners.TryGetValue(itemId, out List<ItemConsumeCondition> conditions))
                {
                    conditions = [];
                    _listeners[itemId] = conditions;
                }

                // Add the current condition to the listeners for this item
                conditions.Add(condition);
            }
        }

        /// <summary>
        /// Hook that is called when an item is consumed
        /// </summary>
        /// <param name="player">Player that consumed the item</param>
        /// <param name="itemId">Item ID that was consumed</param>
        private static void NewAchievementsHelper_OnItemConsume(Player player, int itemId)
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
