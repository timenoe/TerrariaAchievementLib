using System.Collections.Generic;
using Terraria;

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
        /// NPC IDs of the NPCs that drop the loot
        /// </summary>
        private static List<short> _npcs;

        /// <summary>
        /// IDs and the conditions that are listening for them to be triggered
        /// </summary>
        protected static readonly Dictionary<int, List<CustomNpcLootCondition>> _listeners = [];


        /// <summary>
        /// Creates a condition that listens for the NPC to drop loot
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcs">NPCs that drop the loot</param>
        /// <param name="id">Loot ID to listen for</param>
        private CustomNpcLootCondition(ConditionRequirements reqs, List<short> npcs, int id) : base($"{CustomName}_{string.Join(",", npcs)}", reqs, [id]) => Listen(npcs);

        /// <summary>
        /// Creates a condition that listens for the NPC to drop any of the loot
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcs">NPCs that drop the loot</param>
        /// <param name="ids">Loot IDs to listen for</param>
        private CustomNpcLootCondition(ConditionRequirements reqs, List<short> npcs, int[] ids) : base($"{CustomName}_{string.Join(",", npcs)}", reqs, ids) => Listen(npcs);


        /// <summary>
        /// Helper to create a condition that listens for the NPC to drop loot
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcs">NPCs that drop the loot</param>
        /// <param name="id">Loot ID to listen for</param>
        /// <returns>NPC loot achievement condition</returns>
        public static CustomAchievementCondition Drop(ConditionRequirements reqs, List<short> npcs, int id) => new CustomNpcLootCondition(reqs, npcs, id);

        /// <summary>
        /// Helper to create a condition that listens for the NPC to drop any of the loot
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcs">NPCs that drop the loot</param>
        /// <param name="ids">Loot IDs to listen for</param>
        /// <returns>NPC loot achievement condition</returns>
        public static CustomAchievementCondition DropAny(ConditionRequirements reqs, List<short> npcs, params int[] ids) => new CustomNpcLootCondition(reqs, npcs, ids);

        /// <summary>
        /// Helper to create a condition that listens for the NPC to drop all of the loot
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="npcs">NPCs that drop the loot</param>
        /// <param name="ids">Loot IDs to listen for</param>
        /// <returns>NPC loot achievement conditions</returns>
        public static List<CustomAchievementCondition> DropAll(ConditionRequirements reqs, List<short> npcs, params int[] ids)
        {
            List<CustomAchievementCondition> conditions = [];
            foreach (var id in ids)
                conditions.Add(new CustomNpcLootCondition(reqs, npcs, id));
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
                if (condition.Reqs.Pass(Main.LocalPlayer) && _npcs.Contains((short)npc.type))
                    condition.Complete();
            }
        }

        /// <summary>
        /// Listen for events so the condition can be completed
        /// </summary>
        private void Listen(List<short> npcs)
        {
            if (!_isHooked)
            {
                CustomAchievementsHelper.OnNpcLoot += CustomAchievementsHelper_OnNpcLoot;
                _isHooked = true;
            }

            _npcs = npcs;
            ListenForId(this, _listeners);
        }
    }
}
