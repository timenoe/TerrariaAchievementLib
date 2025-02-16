using System.Collections.Generic;
using Terraria;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for item(s) to be crafted
    /// </summary>
    public class ItemCraftCondition : AchIdCondition
    {
        /// <summary>
        /// Base condition identifier (used for saving to achievements.dat)
        /// </summary>
        private const string CustomName = "CUSTOM_ITEM_CRAFT";


        /// <summary>
        /// True if the class has been hooked to the helper event
        /// </summary>
        private static bool _isHooked;

        /// <summary>
        /// IDs and the conditions that are listening for them to be triggered
        /// </summary>
        private static readonly Dictionary<int, List<ItemCraftCondition>> _listeners = [];


        /// <summary>
        /// Creates a condition that listens for the item to be crafted
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Item ID to listen for</param>
        private ItemCraftCondition(ConditionReqs reqs, int id) : base(CustomName, reqs, [id]) => Listen(this);

        /// <summary>
        /// Creates a condition that listens for any of the items to be crafted
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        private ItemCraftCondition(ConditionReqs reqs, int[] ids) : base(CustomName, reqs, ids) => Listen(this);


        /// <summary>
        /// Helper to create a condition that listens for the item to be crafted
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Item ID to listen for</param>
        /// <returns>Item craft achievement condition</returns>
        public static CustomAchievementCondition Craft(ConditionReqs reqs, int id) => new ItemCraftCondition(reqs, id);

        /// <summary>
        /// Helper to create a condition that listens for any of the items to be crafted
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        /// <returns>Item craft achievement condition</returns>
        public static CustomAchievementCondition CraftAny(ConditionReqs reqs, params int[] ids) => new ItemCraftCondition(reqs, ids);

        /// <summary>
        /// Helper to create a condition that listens for all of the items to be crafted
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        /// <returns>Item craft achievement conditions</returns>
        public static List<CustomAchievementCondition> CraftAll(ConditionReqs reqs, params int[] ids)
        {
            List<CustomAchievementCondition> conditions = [];
            foreach (var id in ids)
                conditions.Add(new ItemCraftCondition(reqs, id));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when an item is crafted
        /// </summary>
        /// <param name="player">Player that crafted the item</param>
        /// <param name="id">Item ID that was crafted</param>
        /// <param name="count">Count of the crafted item</param>
        private static void CustomAchievementsHelper_OnItemCraft(Player player, int id, int count)
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
        private static void Listen(ItemCraftCondition condition)
        {
            if (!_isHooked)
            {
                CustomAchievementsHelper.OnItemCraft += CustomAchievementsHelper_OnItemCraft;
                _isHooked = true;
            }

            ListenForIds(condition, condition.Ids, _listeners);
        }
    }
}
