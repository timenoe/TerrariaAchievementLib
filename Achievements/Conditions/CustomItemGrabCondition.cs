using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Achievements;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for item(s) to be grabbed
    /// </summary>
    public class CustomItemGrabCondition : CustomIdCondition
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
        /// Creates a condition that listens for the item to be grabbed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Item ID to listen for</param>
        private CustomItemGrabCondition(ConditionRequirements reqs, int id) : base(CustomName, reqs, [id]) { }

        /// <summary>
        /// Creates a condition that listens for any of the items to be grabbed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        private CustomItemGrabCondition(ConditionRequirements reqs, int[] ids) : base(CustomName, reqs, ids) { }


        protected override void HookIdEvent()
        {
            if (!_isHooked)
            {
                AchievementsHelper.OnItemPickup += AchievementsHelper_OnItemPickup;
                _isHooked = true;
            }
        }

        /// <summary>
        /// Helper to create a condition that listens for the item to be grabbed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Item ID to listen for</param>
        /// <returns>Item pickup achievement condition</returns>
        public static CustomAchievementCondition Grab(ConditionRequirements reqs, int id) => new CustomItemGrabCondition(reqs, id);

        /// <summary>
        /// Helper to create a condition that listens for any of the items to be grabbed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        /// <returns>Item pickup achievement condition</returns>
        public static CustomAchievementCondition GrabAny(ConditionRequirements reqs, params int[] ids) => new CustomItemGrabCondition(reqs, ids);

        /// <summary>
        /// Helper to create a condition that listens for all of the items to be grabbed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        /// <returns>Item pickup achievement conditions</returns>
        public static List<CustomAchievementCondition> GrabAll(ConditionRequirements reqs, params int[] ids)
        {
            List<CustomAchievementCondition> conditions = [];
            foreach (var id in ids)
                conditions.Add(new CustomItemGrabCondition(reqs, id));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when an item is picked up
        /// </summary>
        /// <param name="player">Player that grabbed the item</param>
        /// <param name="id">Item ID that was grabbed</param>
        /// <param name="id">Count of the grabbed item(s)</param>
        private static void AchievementsHelper_OnItemPickup(Player player, short id, int count)
        {
            if (!IsListeningForId(id, out var conditions))
                return;

            foreach (var condition in conditions)
            {
                if (condition.Reqs.Pass(player))
                    condition.Complete();
            }
        }
    }
}
