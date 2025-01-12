using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.ID;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for an NPC to drop loot
    /// </summary>
    public class CustomNpcLootCondition : CustomIdCondition
    {
        /// <summary>
        /// Base condition identifier (used for saving to achievements.dat)
        /// </summary>
        private const string CustomName = "CUSTOM_NPC_LOOT";


        /// <summary>
        /// True if the class has been hooked to the helper event
        /// </summary>
        private static bool _isHooked;

        /// <summary>
        /// NPC ID of the NPC to drop loot
        /// </summary>
        private static short _npc;

        /// <summary>
        /// IDs and the conditions that are listening for them to be triggered
        /// </summary>
        protected static readonly Dictionary<int, List<CustomNpcLootCondition>> _listeners = [];


        /// <summary>
        /// Creates a condition that listens for the NPC to drop loot
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npc">NPC that drops the loot</param>
        /// <param name="id">Loot ID to listen for</param>
        private CustomNpcLootCondition(ConditionRequirements reqs, short npc, int id) : base($"{CustomName}_{npc}", reqs, [id]) => Listen(npc);

        /// <summary>
        /// Creates a condition that listens for the NPC to drop any of the loot
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npc">NPC that drops the loot</param>
        /// <param name="ids">Loot IDs to listen for</param>
        private CustomNpcLootCondition(ConditionRequirements reqs, short npc, int[] ids) : base($"{CustomName}_{npc}", reqs, ids) => Listen(npc);


        /// <summary>
        /// Helper to create a condition that listens for the NPC to drop loot
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npc">NPC that drops the loot</param>
        /// <param name="id">Loot ID to listen for</param>
        /// <returns>NPC loot achievement condition</returns>
        public static CustomAchievementCondition Drop(ConditionRequirements reqs, short npc, int id) => new CustomNpcLootCondition(reqs, npc, id);

        /// <summary>
        /// Helper to create a condition that listens for the NPC to drop any of the loot
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npc">NPC that drops the loot</param>
        /// <param name="ids">Loot IDs to listen for</param>
        /// <returns>NPC loot achievement condition</returns>
        public static CustomAchievementCondition DropAny(ConditionRequirements reqs, short npc, params int[] ids) => new CustomNpcLootCondition(reqs, npc, ids);

        /// <summary>
        /// Helper to create a condition that listens for the NPC to drop all of the loot
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npc">NPC that drops the loot</param>
        /// <param name="ids">Loot IDs to listen for</param>
        /// <returns>NPC loot achievement conditions</returns>
        public static List<CustomAchievementCondition> DropAll(ConditionRequirements reqs, short npc, params int[] ids)
        {
            List<CustomAchievementCondition> conditions = [];
            foreach (var id in ids)
                conditions.Add(new CustomNpcLootCondition(reqs, npc, id));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when an NPC drops loot
        /// </summary>
        /// <param name="npc">NPC that dropped the loot</param>
        /// <param name="id">Item ID of the loot</param>
        private void CustomAchievementsHelper_OnNpcLoot(NPC npc, int id)
        {
            if (!IsListeningForId(id, _listeners, out var conditions))
                return;

            foreach (var condition in conditions)
            {
                if (condition.Reqs.Pass(Main.LocalPlayer) && _npc == npc.type)
                    condition.Complete();
            }
        }

        /// <summary>
        /// Listen for events so the condition can be completed
        /// </summary>
        private void Listen(short npc)
        {
            if (!_isHooked)
            {
                CustomAchievementsHelper.OnNpcLoot += CustomAchievementsHelper_OnNpcLoot;
                _isHooked = true;
            }

            _npc = npc;
            ListenForId(this, _listeners);
        }
    }
}
