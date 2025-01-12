using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Achievements;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for NPC(s) to be killed
    /// </summary>
    public class NpcKillCondition : AchievementIdCondition
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
        /// IDs and the conditions that are listening for them to be triggered
        /// </summary>
        protected static readonly Dictionary<int, List<NpcKillCondition>> _listeners = [];


        /// <summary>
        /// Creates a condition that listens for the NPC to be killed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">NPC ID to listen for</param>
        private NpcKillCondition(ConditionRequirements reqs, int id) : base(CustomName, reqs, [id]) => Listen();

        /// <summary>
        /// Creates a condition that listens for any of the NPCs to be killed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">NPC IDs to listen for</param>
        private NpcKillCondition(ConditionRequirements reqs, int[] ids) : base(CustomName, reqs, ids) => Listen();


        /// <summary>
        /// Helper to create a condition that listens for the NPC to be killed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">NPC ID to listen for</param>
        /// <returns>NPC kill achievement condition</returns>
        public static AchCondition Kill(ConditionRequirements reqs, int id) => new NpcKillCondition(reqs, id);

        /// <summary>
        /// Helper to create a condition that listens for any of the NPCs to be killed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">NPC IDs to listen for</param>
        /// <returns>NPC kill achievement condition</returns>
        public static AchCondition KillAny(ConditionRequirements reqs, params int[] ids) => new NpcKillCondition(reqs, ids);

        /// <summary>
        /// Helper to create a condition that listens for all of the NPCs to be killed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">NPC IDs to listen for</param>
        /// <returns>NPC kill achievement conditions</returns>
        public static List<AchCondition> KillAll(ConditionRequirements reqs, params int[] ids)
        {
            List<AchCondition> conditions = [];
            foreach (var id in ids)
                conditions.Add(new NpcKillCondition(reqs, id));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when an NPC is killed
        /// </summary>
        /// <param name="player">Player that killed the NPC</param>
        /// <param name="id">NPC ID that was killed</param>
        private static void AchievementsHelper_OnNPCKilled(Player player, short id)
        {
            if (!IsListeningForId(id, _listeners, out var conditions))
                return;

            foreach (var condition in conditions)
            {
                if (condition.Reqs.Pass(player))
                    condition.Complete();
            }
        }

        /// <summary>
        /// Listen for events so the condition can be completed
        /// </summary>
        private void Listen()
        {
            if (!_isHooked)
            {
                AchievementsHelper.OnNPCKilled += AchievementsHelper_OnNPCKilled;
                _isHooked = true;
            }

            ListenForId(this, _listeners);
        }
    }
}
