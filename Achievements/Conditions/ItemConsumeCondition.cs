using System.Collections.Generic;
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
        /// <param name="itemId">Item ID to listen for</param>
        private ItemConsumeCondition(int itemId) : base($"{Identifier}_{itemId}")
        {
            ItemIds = [itemId];
            ListenForItemConsume(this);
        }

        /// <summary>
        /// Creates a condition that listens for any of the items to be consumed
        /// </summary>
        /// <param name="itemIds">Item IDs to listen for</param>
        private ItemConsumeCondition(int[] itemIds) : base($"{Identifier}_{string.Join(",", itemIds)}")
        {
            ItemIds = itemIds;
            ListenForItemConsume(this);
        }


        /// <summary>
        /// Helper to create a condition that listens for the item to be consumed
        /// </summary>
        /// <param name="itemId">Item ID to listen for</param>
        /// <returns>Item consume achievement condition</returns>
        public static AchievementCondition Activate(int itemId) => new ItemConsumeCondition(itemId);

        /// <summary>
        /// Helper to create a condition that listens for any of the items to be consumed
        /// </summary>
        /// <param name="itemIds">Item IDs to listen for</param>
        /// <returns>Item consume achievement condition</returns>
        public static AchievementCondition ActivateAny(params int[] itemIds) => new ItemConsumeCondition(itemIds);

        /// <summary>
        /// Helper to create a condition that listens for all of the items to be consumed
        /// </summary>
        /// <param name="itemIds">Item IDs to listen for</param>
        /// <returns>Item consume achievement conditions</returns>
        public static AchievementCondition[] ActivateAll(params int[] itemIds)
        {
            AchievementCondition[] array = new AchievementCondition[itemIds.Length];

            for (int i = 0; i < itemIds.Length; i++)
                array[i] = new ItemConsumeCondition(itemIds[i]);

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
            if (player.whoAmI != Main.myPlayer)
                return;

            if (!_listeners.TryGetValue(itemId, out List<ItemConsumeCondition> conditions))
                return;

            foreach (ItemConsumeCondition condition in conditions)
                condition.Complete();
        }
    }
}
