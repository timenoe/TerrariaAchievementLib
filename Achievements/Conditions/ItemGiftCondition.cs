using System.Collections.Generic;
using Terraria;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for item(s) to be gifted from NPC(s)
    /// </summary>
    public class ItemGiftCondition : AchIdCondition
    {
        /// <summary>
        /// Base condition identifier (used for saving to achievements.dat)
        /// </summary>
        private const string CustomName = "CUSTOM_ITEM_GIFT";


        /// <summary>
        /// True if the class has been hooked to the helper event
        /// </summary>
        private static bool _isHooked;

        /// <summary>
        /// NPC IDs of the NPCs that gift the items
        /// </summary>
        private static List<short> _npcs;

        /// <summary>
        /// IDs and the conditions that are listening for them to be triggered
        /// </summary>
        protected static readonly Dictionary<int, List<ItemGiftCondition>> _listeners = [];


        /// <summary>
        /// Creates a condition that listens for an item to be gifted from NPC(s)
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcs">NPCs that gift the item</param>
        /// <param name="id">Item ID to listen for</param>
        private ItemGiftCondition(ConditionReqs reqs, List<short> npcs, int id) : base($"{CustomName}_{string.Join(",", npcs)}", reqs, [id]) => Listen(npcs);

        /// <summary>
        /// Creates a condition that listens for any of the items to be gifted from NPC(s)
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcs">NPCs that gift the items</param>
        /// <param name="ids">Item IDs to listen for</param>
        private ItemGiftCondition(ConditionReqs reqs, List<short> npcs, int[] ids) : base($"{CustomName}_{string.Join(",", npcs)}", reqs, ids) => Listen(npcs);


        /// <summary>
        /// Helper to create a condition that listens for an item to be gifted from NPC(s)
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcs">NPCs that gift the items</param>
        /// <param name="id">Item ID to listen for</param>
        /// <returns>Item gift achievement condition</returns>
        public static AchCondition Buy(ConditionReqs reqs, List<short> npcs, int id) => new ItemGiftCondition(reqs, npcs, id);

        /// <summary>
        /// Helper to create a condition that listens for any of the items to be gifted from NPC(s)
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcs">NPCs that gift the items</param>
        /// <param name="ids">Item IDs to listen for</param>
        /// <returns>Item gift achievement condition</returns>
        public static AchCondition BuyAny(ConditionReqs reqs, List<short> npcs, params int[] ids) => new ItemGiftCondition(reqs, npcs, ids);

        /// <summary>
        /// Helper to create a condition that listens for all of the items to be gifted from NPC(s)
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcs">NPCs that gift the items</param>
        /// <param name="ids">Item IDs to listen for</param>
        /// <returns>Item gift achievement conditions</returns>
        public static List<AchCondition> BuyAll(ConditionReqs reqs, List<short> npcs, params int[] ids)
        {
            List<AchCondition> conditions = [];
            foreach (var id in ids)
                conditions.Add(new ItemGiftCondition(reqs, npcs, id));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when an item is gifted from an NPC
        /// </summary>
        /// <param name="player">Player that received the item</param>
        /// <param name="npcId">NPC ID that gifted the item</param>
        /// <param name="itemId">Item ID of the gifted item</param>
        private void AchHelper_OnItemGift(Player player, int npcId, int itemId)
        {
            if (!IsListeningForId(itemId, _listeners, out var conditions))
                return;

            foreach (var condition in conditions)
            {
                if (condition.Reqs.Pass(player) && IsNpcValid(npcId))
                    condition.Complete();
            }
        }

        /// <summary>
        /// Check if the NPC that gifted the item is valid
        /// </summary>
        /// <param name="npcId">NPC ID that gifted the item</param>
        /// <returns>True if the NPC is valid</returns>
        private static bool IsNpcValid(int npcId) => _npcs.Count == 0 || _npcs.Contains((short)npcId);

        /// <summary>
        /// Listen for events so the condition can be completed
        /// </summary>
        private void Listen(List<short> npcs)
        {
            if (!_isHooked)
            {
                AchHelper.OnItemGift += AchHelper_OnItemGift;
                _isHooked = true;
            }

            _npcs = npcs;
            ListenForId(this, _listeners);
        }
    }
}
