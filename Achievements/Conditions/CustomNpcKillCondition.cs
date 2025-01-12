using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Achievements;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for NPC(s) to be killed
    /// </summary>
    public class CustomNpcKillCondition : CustomIdCondition
    {
        /// <summary>
        /// Base condition identifier (used for saving to achievements.dat)
        /// </summary>
        private const string CustomName = "CUSTOM_NPC_KILL";


        /// <summary>
        /// True if the class has been hooked to the helper event
        /// </summary>
        private static bool _isHooked;


        /// <summary>
        /// Creates a condition that listens for the NPC to be killed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">NPC ID to listen for</param>
        private CustomNpcKillCondition(ConditionRequirements reqs, int id) : base(CustomName, reqs, [id]) { }

        /// <summary>
        /// Creates a condition that listens for any of the NPCs to be killed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">NPC IDs to listen for</param>
        private CustomNpcKillCondition(ConditionRequirements reqs, int[] ids) : base(CustomName, reqs, ids) { }


        protected override void HookIdEvent()
        {
            if (!_isHooked)
            {
                AchievementsHelper.OnNPCKilled += AchievementsHelper_OnNPCKilled;
                _isHooked = true;
            }
        }


        /// <summary>
        /// Helper to create a condition that listens for the NPC to be killed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">NPC ID to listen for</param>
        /// <returns>NPC kill achievement condition</returns>
        public static CustomAchievementCondition Kill(ConditionRequirements reqs, int id) => new CustomNpcKillCondition(reqs, id);

        /// <summary>
        /// Helper to create a condition that listens for any of the NPCs to be killed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">NPC IDs to listen for</param>
        /// <returns>NPC kill achievement condition</returns>
        public static CustomAchievementCondition KillAny(ConditionRequirements reqs, params int[] ids) => new CustomNpcKillCondition(reqs, ids);

        /// <summary>
        /// Helper to create a condition that listens for all of the NPCs to be killed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">NPC IDs to listen for</param>
        /// <returns>NPC kill achievement conditions</returns>
        public static List<CustomAchievementCondition> KillAll(ConditionRequirements reqs, params int[] ids)
        {
            List<CustomAchievementCondition> conditions = [];
            foreach (var id in ids)
                conditions.Add(new CustomNpcKillCondition(reqs, id));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when an NPC is killed
        /// </summary>
        /// <param name="player">Player that killed the NPC</param>
        /// <param name="id">NPC ID that was killed</param>
        private static void AchievementsHelper_OnNPCKilled(Player player, short id)
        {
            if (!IsListeningForId(id, out var conditions))
                return;

            foreach (var condition in conditions)
            {
                if (condition.Reqs.Pass(player))
                    condition.Complete();
            }
        }
    }
}
