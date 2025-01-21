using System.Collections.Generic;
using Terraria;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for item(s) to be dropped by an NPC
    /// </summary>
    public class NpcDropCondition : AchIdCondition
    {
        /// <summary>
        /// Base condition identifier (used for saving to achievements.dat)
        /// </summary>
        private const string CustomName = "CUSTOM_NPC_DROP";


        /// <summary>
        /// True if the class has been hooked to the helper event
        /// </summary>
        private static bool _isHooked;

        /// <summary>
        /// IDs and the conditions that are listening for them to be triggered
        /// </summary>
        private static readonly Dictionary<int, List<NpcDropCondition>> _listeners = [];

        /// <summary>
        /// NPC ID of the NPC that should drop the item(s)
        /// </summary>
        private short _npcId;


        /// <summary>
        /// Creates a condition that listens for the NPC to drop an item
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcId">NPCs that drop the loot</param>
        /// <param name="itemId">Item ID to listen for</param>
        private NpcDropCondition(ConditionReqs reqs, short npcId, int itemId) : base($"{CustomName}_{string.Join(",", npcId)}", reqs, [itemId]) => Listen(npcId);

        /// <summary>
        /// Creates a condition that listens for the NPC to drop any of the items
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcId">NPCs that drop the loot</param>
        /// <param name="itemIds">Item IDs to listen for</param>
        private NpcDropCondition(ConditionReqs reqs, short npcId, int[] itemIds) : base($"{CustomName}_{string.Join(",", npcId)}", reqs, itemIds) => Listen(npcId);


        /// <summary>
        /// Helper to create a condition that listens for the NPC to drop an item
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcId">NPC ID that drops the item</param>
        /// <param name="itemId">Item ID to listen for</param>
        /// <returns>NPC item drop achievement condition</returns>
        public static AchCondition Drop(ConditionReqs reqs, short npcId, int itemId) => new NpcDropCondition(reqs, npcId, itemId);

        /// <summary>
        /// Helper to create a condition that listens for the NPC to drop any of the items
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcId">NPC ID that drops the item</param>
        /// <param name="itemIds">Item IDs to listen for</param>
        /// <returns>NPC item drop achievement condition</returns>
        public static AchCondition DropAny(ConditionReqs reqs, short npcId, params int[] itemIds) => new NpcDropCondition(reqs, npcId, itemIds);

        /// <summary>
        /// Helper to create a condition that listens for the NPC to drop all of the items
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcId">NPC ID that drops the item</param>
        /// <param name="itemIds">Item IDs to listen for</param>
        /// <returns>NPC item drop achievement conditions</returns>
        public static List<AchCondition> DropAll(ConditionReqs reqs, short npcId, params int[] itemIds)
        {
            List<AchCondition> conditions = [];
            foreach (var itemId in itemIds)
                conditions.Add(new NpcDropCondition(reqs, npcId, itemId));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when an NPC drops an item
        /// </summary>
        /// <param name="player">Player that damaged the NPC</param>
        /// <param name="npcId">NPC ID that dropped the item</param>
        /// <param name="itemId">Item ID that was dropped</param>
        private void AchHelper_OnNpcDrop(Player player, short npcId, int itemId)
        {
            if (!IsListeningForId(itemId, _listeners, out var conditions))
                return;

            foreach (var condition in conditions)
            {
                bool validNpc = _npcId == 0 || npcId == _npcId;
                if (condition.Reqs.Pass(player) && validNpc)
                    condition.Complete();
            }
        }

        /// <summary>
        /// Listen for events so the condition can be completed
        /// </summary>
        /// <param name="npcId">NPC ID that should drop the loot</param>
        private void Listen(short npcId)
        {
            if (!_isHooked)
            {
                AchHelper.OnNpcDrop += AchHelper_OnNpcDrop;
                _isHooked = true;
            }

            _npcId = npcId;
            ListenForId(this, _listeners);
        }
    }
}
