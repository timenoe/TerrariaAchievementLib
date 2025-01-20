using System.Collections.Generic;
using Terraria;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for item(s) to be gifted from an NPC
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
        /// NPC ID that should gift the item(s)
        /// </summary>
        private static short _npcId;

        /// <summary>
        /// IDs and the conditions that are listening for them to be triggered
        /// </summary>
        protected static readonly Dictionary<int, List<ItemGiftCondition>> _listeners = [];


        /// <summary>
        /// Creates a condition that listens for an item to be gifted from NPC(s)
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcId">NPC ID that gifts the item</param>
        /// <param name="itemId">Item ID to listen for</param>
        private ItemGiftCondition(ConditionReqs reqs, short npcId, int itemId) : base($"{CustomName}_{string.Join(",", npcId)}", reqs, [itemId]) => Listen(npcId);

        /// <summary>
        /// Creates a condition that listens for any of the items to be gifted from NPC(s)
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcId">NPC ID that gifts the items</param>
        /// <param name="itemIds">Item IDs to listen for</param>
        private ItemGiftCondition(ConditionReqs reqs, short npcId, int[] itemIds) : base($"{CustomName}_{string.Join(",", npcId)}", reqs, itemIds) => Listen(npcId);


        /// <summary>
        /// Helper to create a condition that listens for an item to be gifted from NPC(s)
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcId">NPC ID that gifts the items</param>
        /// <param name="itemID">Item ID to listen for</param>
        /// <returns>Item gift achievement condition</returns>
        public static AchCondition Gift(ConditionReqs reqs, short npcId, int itemID) => new ItemGiftCondition(reqs, npcId, itemID);

        /// <summary>
        /// Helper to create a condition that listens for any of the items to be gifted from NPC(s)
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcId">NPC ID that gifts the items</param>
        /// <param name="itemIds">Item IDs to listen for</param>
        /// <returns>Item gift achievement condition</returns>
        public static AchCondition GiftAny(ConditionReqs reqs, short npcId, params int[] itemIds) => new ItemGiftCondition(reqs, npcId, itemIds);

        /// <summary>
        /// Helper to create a condition that listens for all of the items to be gifted from NPC(s)
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcId">NPC ID that gifts the items</param>
        /// <param name="itemIds">Item IDs to listen for</param>
        /// <returns>Item gift achievement conditions</returns>
        public static List<AchCondition> GiftAll(ConditionReqs reqs, short npcId, params int[] itemIds)
        {
            List<AchCondition> conditions = [];
            foreach (var itemId in itemIds)
                conditions.Add(new ItemGiftCondition(reqs, npcId, itemId));
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
                if (condition.Reqs.Pass(player) && npcId == _npcId)
                    condition.Complete();
            }
        }

        /// <summary>
        /// Listen for events so the condition can be completed
        /// </summary>
        /// <param name="npcId">NPC ID that should gift the item</param>
        private void Listen(short npcId)
        {
            if (!_isHooked)
            {
                AchHelper.OnItemGift += AchHelper_OnItemGift;
                _isHooked = true;
            }

            _npcId = npcId;
            ListenForId(this, _listeners);
        }
    }
}
