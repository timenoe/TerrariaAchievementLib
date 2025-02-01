using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.Bestiary;
using Terraria.ID;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for NPC(s) to be killed
    /// </summary>
    public class NpcKillCondition : AchIdCondition
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
        private static readonly Dictionary<int, List<NpcKillCondition>> _listeners = [];

        /// <summary>
        /// True if killing the NPC for the first time
        /// </summary>
        private bool _first;

        /// <summary>
        /// Creates a condition that listens for the NPC to be killed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="first">True if killing the NPC for the first time</param>
        /// <param name="id">NPC ID to listen for</param>
        private NpcKillCondition(ConditionReqs reqs, bool first, int id) : base($"{CustomName}_{first}", reqs, [id]) => Listen(first);

        /// <summary>
        /// Creates a condition that listens for any of the NPCs to be killed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="first">True if killing the NPC for the first time</param>
        /// <param name="ids">NPC IDs to listen for</param>
        private NpcKillCondition(ConditionReqs reqs, bool first, int[] ids) : base($"{CustomName}_{first}", reqs, ids) => Listen(first);


        /// <summary>
        /// Helper to create a condition that listens for the NPC to be killed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="first">True if killing the NPC for the first time</param>
        /// <param name="id">NPC ID to listen for</param>
        /// <returns>NPC kill achievement condition</returns>
        public static AchCondition Kill(ConditionReqs reqs, bool first, int id) => new NpcKillCondition(reqs, first, id);

        /// <summary>
        /// Helper to create a condition that listens for any of the NPCs to be killed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="first">True if killing the NPC for the first time</param>
        /// <param name="ids">NPC IDs to listen for</param>
        /// <returns>NPC kill achievement condition</returns>
        public static AchCondition KillAny(ConditionReqs reqs, bool first, params int[] ids) => new NpcKillCondition(reqs, first, ids);

        /// <summary>
        /// Helper to create a condition that listens for all of the NPCs to be killed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="first">True if killing the NPC for the first time</param>
        /// <param name="ids">NPC IDs to listen for</param>
        /// <returns>NPC kill achievement conditions</returns>
        public static List<AchCondition> KillAll(ConditionReqs reqs, bool first, params int[] ids)
        {
            List<AchCondition> conditions = [];
            foreach (var id in ids)
                conditions.Add(new NpcKillCondition(reqs, first, id));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when an NPC is killed
        /// </summary>
        /// <param name="player">Player that killed the NPC</param>
        /// <param name="id">NPC ID that was killed</param>
        private void AchievementsHelper_OnNPCKilled(Player player, short id)
        {
            if (!IsListeningForId(id, _listeners, out var conditions))
                return;

            foreach (var condition in conditions)
            {
                if (_first && Main.BestiaryTracker.Kills.GetKillCount(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[id]) != 0)
                    continue;
                
                if (condition.Reqs.Pass(player))
                    condition.Complete();
            }
        }

        /// <summary>
        /// Listen for events so the condition can be completed
        /// <param name="first">True if killing the NPC for the first time</param>
        /// </summary>
        private void Listen(bool first)
        {
            if (!_isHooked)
            {
                AchievementsHelper.OnNPCKilled += AchievementsHelper_OnNPCKilled;
                _isHooked = true;
            }

            ListenForId(this, _listeners);
            _first = first;
        }
    }
}
