using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Achievements;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for item(s) to be grabbed
    /// </summary>
    public class ItemGrabCondition : AchievementIdCondition
    {
        /// <summary>
        /// Base condition identifier (used for saving to achievements.dat)
        /// </summary>
        private const string CustomName = "CUSTOM_ITEM_GRAB";


        /// <summary>
        /// True if the class has been hooked to the helper event
        /// </summary>
        private static bool _isHooked;

        /// <summary>
        /// IDs and the conditions that are listening for them to be triggered
        /// </summary>
        protected static readonly Dictionary<int, List<ItemGrabCondition>> _listeners = [];


        /// <summary>
        /// Creates a condition that listens for the item to be grabbed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Item ID to listen for</param>
        private ItemGrabCondition(ConditionRequirements reqs, int id) : base(CustomName, reqs, [id]) => Listen();

        /// <summary>
        /// Creates a condition that listens for any of the items to be grabbed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        private ItemGrabCondition(ConditionRequirements reqs, int[] ids) : base(CustomName, reqs, ids) => Listen();


        /// <summary>
        /// Helper to create a condition that listens for the item to be grabbed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Item ID to listen for</param>
        /// <returns>Item grab achievement condition</returns>
        public static AchCondition Grab(ConditionRequirements reqs, int id) => new ItemGrabCondition(reqs, id);

        /// <summary>
        /// Helper to create a condition that listens for any of the items to be grabbed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        /// <returns>Item grab achievement condition</returns>
        public static AchCondition GrabAny(ConditionRequirements reqs, params int[] ids) => new ItemGrabCondition(reqs, ids);

        /// <summary>
        /// Helper to create a condition that listens for all of the items to be grabbed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        /// <returns>Item grab achievement conditions</returns>
        public static List<AchCondition> GrabAll(ConditionRequirements reqs, params int[] ids)
        {
            List<AchCondition> conditions = [];
            foreach (var id in ids)
                conditions.Add(new ItemGrabCondition(reqs, id));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when an item is grabbed
        /// </summary>
        /// <param name="player">Player that grabbed the item</param>
        /// <param name="id">Item ID that was grabbed</param>
        /// <param name="id">Count of the grabbed item(s)</param>
        private static void AchievementsHelper_OnItemPickup(Player player, short id, int count)
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
        private void Listen()
        {
            if (!_isHooked)
            {
                AchievementsHelper.OnItemPickup += AchievementsHelper_OnItemPickup;
                _isHooked = true;
            }

            ListenForId(this, _listeners);
        }
    }
}
