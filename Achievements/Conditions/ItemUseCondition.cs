using System.Collections.Generic;
using Terraria;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for item(s) to be used
    /// </summary>
    public class ItemUseCondition : AchIdCondition
    {
        /// <summary>
        /// Base condition identifier (used for saving to achievements.dat)
        /// </summary>
        private const string CustomName = "CUSTOM_ITEM_USE";


        /// <summary>
        /// True if the class has been hooked to the helper event
        /// </summary>
        private static bool _isHooked;

        /// <summary>
        /// IDs and the conditions that are listening for them to be triggered
        /// </summary>
        private static readonly Dictionary<int, List<ItemUseCondition>> _listeners = [];


        /// <summary>
        /// Creates a condition that listens for the item to be used
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Item ID to listen for</param>
        private ItemUseCondition(ConditionReqs reqs, int id) : base(CustomName, reqs, [id]) => Listen(this);

        /// <summary>
        /// Creates a condition that listens for any of the items to be used
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        private ItemUseCondition(ConditionReqs reqs, int[] ids) : base(CustomName, reqs, ids) => Listen(this);


        /// <summary>
        /// Helper to create a condition that listens for the item to be used
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Item ID to listen for</param>
        /// <returns>Item use achievement condition</returns>
        public static AchCondition Use(ConditionReqs reqs, int id) => new ItemUseCondition(reqs, id);

        /// <summary>
        /// Helper to create a condition that listens for any of the items to be used
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        /// <returns>Item use achievement condition</returns>
        public static AchCondition UseAny(ConditionReqs reqs, params int[] ids) => new ItemUseCondition(reqs, ids);

        /// <summary>
        /// Helper to create a condition that listens for all of the items to be used
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        /// <returns>Item use achievement conditions</returns>
        public static List<AchCondition> UseAll(ConditionReqs reqs, params int[] ids)
        {
            List<AchCondition> conditions = [];
            foreach (var id in ids)
                conditions.Add(new ItemUseCondition(reqs, id));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when an item is used
        /// </summary>
        /// <param name="player">Player that used the item</param>
        /// <param name="id">Item ID that was used</param>
        private static void AchHelper_OnItemUse(Player player, int id)
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
        private static void Listen(ItemUseCondition condition)
        {
            if (!_isHooked)
            {
                AchHelper.OnItemUse += AchHelper_OnItemUse;
                _isHooked = true;
            }

            ListenForIds(condition, condition.Ids, _listeners);
        }
    }
}
