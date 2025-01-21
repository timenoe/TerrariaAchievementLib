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
        /// Creates a condition that listens for the NPC to drop loot
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcs">NPCs that drop the loot</param>
        /// <param name="id">Loot ID to listen for</param>
        private NpcDropCondition(ConditionReqs reqs, List<short> npcs, int id) : base($"{CustomName}_{string.Join(",", npcs)}", reqs, [id]) => Listen(npcs);

        /// <summary>
        /// Creates a condition that listens for the NPC to drop any of the loot
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcs">NPCs that drop the loot</param>
        /// <param name="ids">Loot IDs to listen for</param>
        private NpcDropCondition(ConditionReqs reqs, List<short> npcs, int[] ids) : base($"{CustomName}_{string.Join(",", npcs)}", reqs, ids) => Listen(npcs);


        /// <summary>
        /// Helper to create a condition that listens for the NPC to drop loot
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcs">NPCs that drop the loot</param>
        /// <param name="id">Loot ID to listen for</param>
        /// <returns>NPC loot achievement condition</returns>
        public static AchCondition Drop(ConditionReqs reqs, List<short> npcs, int id) => new NpcDropCondition(reqs, npcs, id);

        /// <summary>
        /// Helper to create a condition that listens for the NPC to drop any of the loot
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcs">NPCs that drop the loot</param>
        /// <param name="ids">Loot IDs to listen for</param>
        /// <returns>NPC loot achievement condition</returns>
        public static AchCondition DropAny(ConditionReqs reqs, List<short> npcs, params int[] ids) => new NpcDropCondition(reqs, npcs, ids);

        /// <summary>
        /// Helper to create a condition that listens for the NPC to drop all of the loot
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcs">NPCs that drop the loot</param>
        /// <param name="ids">Loot IDs to listen for</param>
        /// <returns>NPC loot achievement conditions</returns>
        public static List<AchCondition> DropAll(ConditionReqs reqs, List<short> npcs, params int[] ids)
        {
            List<AchCondition> conditions = [];
            foreach (var id in ids)
                conditions.Add(new NpcDropCondition(reqs, npcs, id));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when an NPC drops loot
        /// </summary>
        /// <param name="player">Player that damaged the NPC</param>
        /// <param name="npcId">NPC ID that dropped the loot</param>
        /// <param name="id">Item ID of the loot</param>
        private void AchHelper_OnNpcDrop(Player player, short npcId, int id)
        {
            if (!IsListeningForId(id, _listeners, out var conditions))
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
