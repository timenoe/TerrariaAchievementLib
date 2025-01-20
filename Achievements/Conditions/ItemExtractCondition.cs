using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for item(s) to be extracted
    /// </summary>
    public class ItemExtractCondition : AchIdCondition
    {
        /// <summary>
        /// Base condition identifier (used for saving to achievements.dat)
        /// </summary>
        private const string CustomName = "CUSTOM_ITEM_EXTRACT";


        /// <summary>
        /// True if the class has been hooked to the helper event
        /// </summary>
        private static bool _isHooked;

        /// <summary>
        /// IDs and the conditions that are listening for them to be triggered
        /// </summary>
        protected static readonly Dictionary<int, List<ItemExtractCondition>> _listeners = [];


        /// <summary>
        /// Creates a condition that listens for the item to be extracted
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Item ID to listen for</param>
        private ItemExtractCondition(ConditionReqs reqs, int id) : base(CustomName, reqs, [id]) => Listen();

        /// <summary>
        /// Creates a condition that listens for any of the items to be extracted
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        private ItemExtractCondition(ConditionReqs reqs, int[] ids) : base(CustomName, reqs, ids) => Listen();


        /// <summary>
        /// Helper to create a condition that listens for the item to be extracted
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Item ID to listen for</param>
        /// <returns>Item extract achievement condition</returns>
        public static AchCondition Extract(ConditionReqs reqs, int id) => new ItemExtractCondition(reqs, id);

        /// <summary>
        /// Helper to create a condition that listens for any of the items to be extracted
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        /// <returns>Item extract achievement condition</returns>
        public static AchCondition ExtractAny(ConditionReqs reqs, params int[] ids) => new ItemExtractCondition(reqs, ids);

        /// <summary>
        /// Helper to create a condition that listens for all of the items to be extracted
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        /// <returns>Item extract achievement conditions</returns>
        public static List<AchCondition> ExtractAll(ConditionReqs reqs, params int[] ids)
        {
            List<AchCondition> conditions = [];
            foreach (var id in ids)
                conditions.Add(new ItemExtractCondition(reqs, id));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when an item is extracted
        /// </summary>
        /// <param name="player">Player that extracted the item</param>
        /// <param name="id">Item ID that was extracted</param>
        private void AchHelper_OnItemExtract(Player player, int id)
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
                AchHelper.OnItemExtract += AchHelper_OnItemExtract;
                _isHooked = true;
            }

            ListenForId(this, _listeners);
        }
    }
}
