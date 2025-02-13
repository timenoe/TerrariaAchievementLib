using System.Collections.Generic;
using Terraria;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for item(s) to be equipped
    /// </summary>
    public class ItemEquipCondition : AchIdCondition
    {
        /// <summary>
        /// Base condition identifier (used for saving to achievements.dat)
        /// </summary>
        private const string CustomName = "CUSTOM_ITEM_EQUIP";


        /// <summary>
        /// True if the class has been hooked to the helper event
        /// </summary>
        private static bool _isHooked;

        /// <summary>
        /// IDs and the conditions that are listening for them to be triggered
        /// </summary>
        private static readonly Dictionary<int, List<ItemEquipCondition>> _listeners = [];

        /// <summary>
        /// Item slot context ID
        /// </summary>
        private int _contextId;


        /// <summary>
        /// Creates a condition that listens for an item to be equipped
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="contextId">Item slot context ID</param>
        /// <param name="itemId">Item ID to listen for</param>
        private ItemEquipCondition(ConditionReqs reqs, int contextId, int itemId) : base($"{CustomName}_{string.Join(",", contextId)}", reqs, [itemId]) => Listen(this, contextId);

        /// <summary>
        /// Creates a condition that listens for any of the items to be equipped
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="contextId">Item slot context ID</param>
        /// <param name="itemIds">Item IDs to listen for</param>
        private ItemEquipCondition(ConditionReqs reqs, int contextId, int[] itemIds) : base($"{CustomName}_{string.Join(",", contextId)}", reqs, itemIds) => Listen(this, contextId);


        /// <summary>
        /// Helper to create a condition that listens for an item to be equipped
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="contextId">Item slot context ID</param>
        /// <param name="itemID">Item ID to listen for</param>
        /// <returns>Item equip achievement condition</returns>
        public static CustomAchievementCondition Equip(ConditionReqs reqs, int contextId, int itemID) => new ItemEquipCondition(reqs, contextId, itemID);

        /// <summary>
        /// Helper to create a condition that listens for any of the items to be equipped
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="contextId">Item slot context ID</param>
        /// <param name="itemIds">Item IDs to listen for</param>
        /// <returns>Item equip achievement condition</returns>
        public static CustomAchievementCondition EquipAny(ConditionReqs reqs, int contextId, params int[] itemIds) => new ItemEquipCondition(reqs, contextId, itemIds);

        /// <summary>
        /// Helper to create a condition that listens for all of the items to be equipped
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="contextId">Item slot context ID</param>
        /// <param name="itemIds">Item IDs to listen for</param>
        /// <returns>Item equip achievement conditions</returns>
        public static List<CustomAchievementCondition> EquipAll(ConditionReqs reqs, int contextId, params int[] itemIds)
        {
            List<CustomAchievementCondition> conditions = [];
            foreach (var itemId in itemIds)
                conditions.Add(new ItemEquipCondition(reqs, contextId, itemId));
            return conditions;
        }


        /// <summary>
        /// Hook that is called when an item is equipped
        /// </summary>
        /// <param name="player">Player that equipped the item</param>
        /// <param name="contextId">Item slot context ID</param>
        /// <param name="itemId">Item ID that was equipped</param>
        private static void AchHelper_OnItemEquip(Player player, int contextId, int itemId)
        {
            if (!IsListeningForId(itemId, _listeners, out var conditions))
                return;

            foreach (var condition in conditions)
            {
                // Check item slot context ID when applicable
                if (condition._contextId != 0 && contextId != condition._contextId)
                    continue;

                if (condition.Reqs.Pass(player))
                    condition.Complete();
            }
        }

        /// <summary>
        /// Listen for events so the condition can be completed
        /// </summary>
        /// <param name="contextId">Item slot context ID</param>
        private static void Listen(ItemEquipCondition condition, int contextId)
        {
            if (!_isHooked)
            {
                CustomAchievementHelper.OnItemEquip += AchHelper_OnItemEquip;
                _isHooked = true;
            }

            ListenForIds(condition, condition.Ids, _listeners);
            condition._contextId = contextId;
        }
    }
}
