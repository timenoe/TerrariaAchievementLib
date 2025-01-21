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
        /// Creates a condition that listens for an item to be bought from NPC(s)
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcs">NPCs that sell the item</param>
        /// <param name="id">Item ID to listen for</param>
        private NpcBuyCondition(ConditionReqs reqs, List<short> npcs, int id) : base($"{CustomName}_{string.Join(",", npcs)}", reqs, [id]) => Listen(npcs);

        /// <summary>
        /// Creates a condition that listens for any of the items to be bought from NPC(s)
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcs">NPCs that sell the items</param>
        /// <param name="ids">Item IDs to listen for</param>
        private NpcBuyCondition(ConditionReqs reqs, List<short> npcs, int[] ids) : base($"{CustomName}_{string.Join(",", npcs)}", reqs, ids) => Listen(npcs);


        /// <summary>
        /// Helper to create a condition that listens for an item to be bought from NPC(s)
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcs">NPCs that sell the items</param>
        /// <param name="id">Item ID to listen for</param>
        /// <returns>NPC buy achievement condition</returns>
        public static AchCondition Buy(ConditionReqs reqs, List<short> npcs, int id) => new NpcBuyCondition(reqs, npcs, id);

        /// <summary>
        /// Helper to create a condition that listens for any of the items to be bought from NPC(s)
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcs">NPCs that sell the items</param>
        /// <param name="ids">Item IDs to listen for</param>
        /// <returns>NPC buy achievement condition</returns>
        public static AchCondition BuyAny(ConditionReqs reqs, List<short> npcs, params int[] ids) => new NpcBuyCondition(reqs, npcs, ids);

        /// <summary>
        /// Helper to create a condition that listens for all of the items to be bought from NPC(s)
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcs">NPCs that sell the items</param>
        /// <param name="ids">Item IDs to listen for</param>
        /// <returns>NPC buy achievement conditions</returns>
        public static List<AchCondition> BuyAll(ConditionReqs reqs, List<short> npcs, params int[] ids)
        {
            List<AchCondition> conditions = [];
            foreach (var id in ids)
                conditions.Add(new NpcBuyCondition(reqs, npcs, id));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when an item is bought from an NPC
        /// </summary>
        /// <param name="player">Player that bought the item</param>
        /// <param name="npcId">NPC ID that sold the item</param>
        /// <param name="id">Item ID of the bought item</param>
        private void AchHelper_OnNpcBuy(Player player, short npcId, int id)
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
        /// <param name="npcId">NPC ID that should sell the item</param>
        private void Listen(short npcId)
        {
            if (!_isHooked)
            {
                AchHelper.OnNpcBuy += AchHelper_OnNpcBuy;
                _isHooked = true;
            }

            _npcId = npcId;
            ListenForId(this, _listeners);
        }
    }
}
