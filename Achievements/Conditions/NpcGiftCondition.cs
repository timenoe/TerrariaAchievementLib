using System.Collections.Generic;
using Terraria;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for item(s) to be gifted from an NPC
    /// </summary>
    public class NpcGiftCondition : AchIdCondition
    {
        /// <summary>
        /// Base condition identifier (used for saving to achievements.dat)
        /// </summary>
        private const string CustomName = "CUSTOM_NPC_GIFT";


        /// <summary>
        /// True if the class has been hooked to the helper event
        /// </summary>
        private static bool _isHooked;

        /// <summary>
        /// IDs and the conditions that are listening for them to be triggered
        /// </summary>
        private static readonly Dictionary<int, List<NpcGiftCondition>> _listeners = [];

        /// <summary>
        /// NPC ID that should gift the item(s)
        /// </summary>
        private short _npcId;


        /// <summary>
        /// Creates a condition that listens for an item to be gifted from an NPC
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcId">NPC ID that gifts the item</param>
        /// <param name="itemId">Item ID to listen for</param>
        private NpcGiftCondition(ConditionReqs reqs, short npcId, int itemId) : base($"{CustomName}_{string.Join(",", npcId)}", reqs, [itemId]) => Listen(this, npcId);

        /// <summary>
        /// Creates a condition that listens for any of the items to be gifted from an NPC
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcId">NPC ID that gifts the items</param>
        /// <param name="itemIds">Item IDs to listen for</param>
        private NpcGiftCondition(ConditionReqs reqs, short npcId, int[] itemIds) : base($"{CustomName}_{string.Join(",", npcId)}", reqs, itemIds) => Listen(this, npcId);


        /// <summary>
        /// Helper to create a condition that listens for an item to be gifted from an NPC
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcId">NPC ID that gifts the items</param>
        /// <param name="itemID">Item ID to listen for</param>
        /// <returns>Item gift achievement condition</returns>
        public static CustomAchievementCondition Gift(ConditionReqs reqs, short npcId, int itemID) => new NpcGiftCondition(reqs, npcId, itemID);

        /// <summary>
        /// Helper to create a condition that listens for any of the items to be gifted from an NPC
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcId">NPC ID that gifts the items</param>
        /// <param name="itemIds">Item IDs to listen for</param>
        /// <returns>Item gift achievement condition</returns>
        public static CustomAchievementCondition GiftAny(ConditionReqs reqs, short npcId, params int[] itemIds) => new NpcGiftCondition(reqs, npcId, itemIds);

        /// <summary>
        /// Helper to create a condition that listens for all of the items to be gifted from an NPC
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcId">NPC ID that gifts the items</param>
        /// <param name="itemIds">Item IDs to listen for</param>
        /// <returns>Item gift achievement conditions</returns>
        public static List<CustomAchievementCondition> GiftAll(ConditionReqs reqs, short npcId, params int[] itemIds)
        {
            List<CustomAchievementCondition> conditions = [];
            foreach (var itemId in itemIds)
                conditions.Add(new NpcGiftCondition(reqs, npcId, itemId));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when an item is gifted from an NPC
        /// </summary>
        /// <param name="player">Player that received the item</param>
        /// <param name="npcId">NPC ID that gifted the item</param>
        /// <param name="itemId">Item ID of the gifted item</param>
        private static void CustomAchievementsHelper_OnNpcGift(Player player, int npcId, int itemId)
        {
            if (!IsListeningForId(itemId, _listeners, out var conditions))
                return;

            foreach (var condition in conditions)
            {
                if (npcId != condition._npcId)
                    continue;
                
                if (condition.Reqs.Pass(player))
                    condition.Complete();
            }
        }

        /// <summary>
        /// Listen for events so the condition can be completed
        /// </summary>
        /// <param name="npcId">NPC ID that should gift the item</param>
        private static void Listen(NpcGiftCondition condition, short npcId)
        {
            if (!_isHooked)
            {
                CustomAchievementsHelper.OnNpcGift += CustomAchievementsHelper_OnNpcGift;
                _isHooked = true;
            }

            ListenForIds(condition, condition.Ids, _listeners);
            condition._npcId = npcId;
        }
    }
}
