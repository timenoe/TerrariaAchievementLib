using System.Collections.Generic;
using Terraria;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for item(s) to be shaken from a tree
    /// </summary>
    public class ItemShakeCondition : AchIdCondition
    {
        /// <summary>
        /// Base condition identifier (used for saving to achievements.dat)
        /// </summary>
        private const string CustomName = "CUSTOM_ITEM_SHAKE";


        /// <summary>
        /// True if the class has been hooked to the helper event
        /// </summary>
        private static bool _isHooked;

        /// <summary>
        /// IDs and the conditions that are listening for them to be triggered
        /// </summary>
        private static readonly Dictionary<int, List<ItemShakeCondition>> _listeners = [];


        /// <summary>
        /// Creates a condition that listens for the item to be shaken from a tree
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Item ID to listen for</param>
        private ItemShakeCondition(ConditionReqs reqs, int id) : base(CustomName, reqs, [id]) => Listen(this);

        /// <summary>
        /// Creates a condition that listens for any of the items to be shaken from a tree
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        private ItemShakeCondition(ConditionReqs reqs, int[] ids) : base(CustomName, reqs, ids) => Listen(this);


        /// <summary>
        /// Helper to create a condition that listens for the item to be shaken from a tree
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Item ID to listen for</param>
        /// <returns>Item shake achievement condition</returns>
        public static CustomAchievementCondition Shake(ConditionReqs reqs, int id) => new ItemShakeCondition(reqs, id);

        /// <summary>
        /// Helper to create a condition that listens for any of the items to be shaken from a tree
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        /// <returns>Item shake achievement condition</returns>
        public static CustomAchievementCondition ShakeAny(ConditionReqs reqs, params int[] ids) => new ItemShakeCondition(reqs, ids);

        /// <summary>
        /// Helper to create a condition that listens for all of the items to be shaken from a tree
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        /// <returns>Item shake achievement conditions</returns>
        public static List<CustomAchievementCondition> ShakeAll(ConditionReqs reqs, params int[] ids)
        {
            List<CustomAchievementCondition> conditions = [];
            foreach (var id in ids)
                conditions.Add(new ItemShakeCondition(reqs, id));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when an item is shaken from a tree
        /// </summary>
        /// <param name="player">Player that shook the tree</param>
        /// <param name="id">Item ID that appeared</param>
        private static void AchHelper_OnItemShake(Player player, int id)
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
        private static void Listen(ItemShakeCondition condition)
        {
            if (!_isHooked)
            {
                CustomAchievementHelper.OnItemShake += AchHelper_OnItemShake;
                _isHooked = true;
            }

            ListenForIds(condition, condition.Ids, _listeners);
        }
    }
}
