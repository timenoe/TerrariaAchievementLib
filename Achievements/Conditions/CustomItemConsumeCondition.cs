using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TerrariaAchievementLib.Items;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for item(s) to be consumed
    /// </summary>
    public class CustomItemConsumeCondition : CustomIdCondition
    {
        /// <summary>
        /// Base condition identifier (used for saving to achievements.dat)
        /// </summary>
        private const string CustomName = "CUSTOM_ITEM_CONSUME";


        /// <summary>
        /// True if the class has been hooked to the helper event
        /// </summary>
        private static bool _isHooked;

        /// <summary>
        /// IDs and the conditions that are listening for them to be triggered
        /// </summary>
        protected static readonly Dictionary<int, List<CustomItemConsumeCondition>> _listeners = [];


        /// <summary>
        /// Creates a condition that listens for the item to be consumed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Item ID to listen for</param>
        private CustomItemConsumeCondition(ConditionRequirements reqs, int id) : base(CustomName, reqs, [id]) => Listen();

        /// <summary>
        /// Creates a condition that listens for any of the items to be consumed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        private CustomItemConsumeCondition(ConditionRequirements reqs, int[] ids) : base(CustomName, reqs, ids) => Listen();


        /// <summary>
        /// Helper to create a condition that listens for the item to be consumed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Item ID to listen for</param>
        /// <returns>Item consume achievement condition</returns>
        public static CustomAchievementCondition Consume(ConditionRequirements reqs, int id) => new CustomItemConsumeCondition(reqs, id);

        /// <summary>
        /// Helper to create a condition that listens for any of the items to be consumed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        /// <returns>Item consume achievement condition</returns>
        public static CustomAchievementCondition ConsumeAny(ConditionRequirements reqs, params int[] ids) => new CustomItemConsumeCondition(reqs, ids);

        /// <summary>
        /// Helper to create a condition that listens for all of the items to be consumed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        /// <returns>Item consume achievement conditions</returns>
        public static List<CustomAchievementCondition> ConsumeAll(ConditionRequirements reqs, params int[] ids)
        {
            List<CustomAchievementCondition> conditions = [];
            foreach (var id in ids)
                conditions.Add(new CustomItemConsumeCondition(reqs, id));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when an item is consumed
        /// </summary>
        /// <param name="player">Player that consumed the item</param>
        /// <param name="id">Item ID that was consumed</param>
        private static void NewAchievementsHelper_OnItemConsume(Player player, int id)
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
        /// Register item IDs to send notifications when they are consumed
        /// </summary>
        /// <param name="itemIds">Item IDs to send notifications when they are consumed</param>
        //private static void RegisterConsumeItem(List<int> itemIds)
        //{
        //    ConsumeItem manager = ModContent.GetInstance<ConsumeItem>();
        //    manager.RegisterItems(itemIds);
        //}

        /// <summary>
        /// Listen for events so the condition can be completed
        /// </summary>
        private void Listen()
        {
            if (!_isHooked)
            {
                CustomAchievementsHelper.OnItemConsume += NewAchievementsHelper_OnItemConsume;
                _isHooked = true;
            }

            ListenForId(this, _listeners);
        }
    }
}
