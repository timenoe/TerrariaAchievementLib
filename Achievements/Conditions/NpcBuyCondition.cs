using System.Collections.Generic;
using Terraria;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for item(s) to be bought from an NPC
    /// </summary>
    public class NpcBuyCondition : AchIdCondition
    {
        /// <summary>
        /// Base condition identifier (used for saving to achievements.dat)
        /// </summary>
        private const string CustomName = "CUSTOM_NPC_BUY";


        /// <summary>
        /// True if the class has been hooked to the helper event
        /// </summary>
        private static bool _isHooked;

        /// <summary>
        /// IDs and the conditions that are listening for them to be triggered
        /// </summary>
        private static readonly Dictionary<int, List<NpcBuyCondition>> _listeners = [];

        /// <summary>
        /// NPC ID of the NPC that should sell the item(s)
        /// </summary>
        private short _npcId;


        /// <summary>
        /// Creates a condition that listens for an item to be bought from an NPC
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcId">NPC ID that sells the item</param>
        /// <param name="itemId">Item ID to listen for</param>
        private NpcBuyCondition(ConditionReqs reqs, short npcId, int itemId) : base($"{CustomName}_{string.Join(",", npcId)}", reqs, [itemId]) => Listen(this, npcId);

        /// <summary>
        /// Creates a condition that listens for any of the items to be bought from an NPC
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcId">NPC ID that sells the items</param>
        /// <param name="itemIds">Item IDs to listen for</param>
        private NpcBuyCondition(ConditionReqs reqs, short npcId, int[] itemIds) : base($"{CustomName}_{string.Join(",", npcId)}", reqs, itemIds) => Listen(this, npcId);


        /// <summary>
        /// Helper to create a condition that listens for an item to be bought from an NPC
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcId">NPC ID that sells the item</param>
        /// <param name="itemId">Item ID to listen for</param>
        /// <returns>NPC item buy achievement condition</returns>
        public static AchCondition Buy(ConditionReqs reqs, short npcId, int itemId) => new NpcBuyCondition(reqs, npcId, itemId);

        /// <summary>
        /// Helper to create a condition that listens for any of the items to be bought from an NPC
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcId">NPC ID that sell the items</param>
        /// <param name="itemIds">Item IDs to listen for</param>
        /// <returns>NPC item buy achievement condition</returns>
        public static AchCondition BuyAny(ConditionReqs reqs, short npcId, params int[] itemIds) => new NpcBuyCondition(reqs, npcId, itemIds);

        /// <summary>
        /// Helper to create a condition that listens for all of the items to be bought from an NPC
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcId">NPC ID that sells the items</param>
        /// <param name="itemIds">Item IDs to listen for</param>
        /// <returns>NPC item buy achievement conditions</returns>
        public static List<AchCondition> BuyAll(ConditionReqs reqs, short npcId, params int[] itemIds)
        {
            List<AchCondition> conditions = [];
            foreach (var itemId in itemIds)
                conditions.Add(new NpcBuyCondition(reqs, npcId, itemId));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when an item is bought from an NPC
        /// </summary>
        /// <param name="player">Player that bought the item</param>
        /// <param name="npcId">NPC ID that sold the item</param>
        /// <param name="itemId">Item ID that was bought</param>
        private static void AchHelper_OnNpcBuy(Player player, short npcId, int itemId)
        {
            if (!IsListeningForId(itemId, _listeners, out var conditions))
                return;

            foreach (var condition in conditions)
            {
                // Check NPC ID when applicable
                if (condition._npcId != 0 && npcId != condition._npcId)
                    continue;

                if (condition.Reqs.Pass(player))
                    condition.Complete();
            }
        }

        /// <summary>
        /// Listen for events so the condition can be completed
        /// </summary>
        /// <param name="npcId">NPC ID that should sell the item</param>
        private static void Listen(NpcBuyCondition condition, short npcId)
        {
            if (!_isHooked)
            {
                AchHelper.OnNpcBuy += AchHelper_OnNpcBuy;
                _isHooked = true;
            }

            ListenForIds(condition, condition.Ids, _listeners);
            condition._npcId = npcId;
        }
    }
}
