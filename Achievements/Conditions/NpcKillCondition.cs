using System.Collections.Generic;
using System.Linq;
using Terraria;
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
        private NpcKillCondition(ConditionReqs reqs, bool first, int id) : base($"{CustomName}_{first}", reqs, [id]) => Listen(this, first);

        /// <summary>
        /// Creates a condition that listens for any of the NPCs to be killed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="first">True if killing the NPC for the first time</param>
        /// <param name="ids">NPC IDs to listen for</param>
        private NpcKillCondition(ConditionReqs reqs, bool first, int[] ids) : base($"{CustomName}_{first}", reqs, ids) => Listen(this, first);


        /// <summary>
        /// Helper to create a condition that listens for the NPC to be killed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="first">True if killing the NPC for the first time</param>
        /// <param name="id">NPC ID to listen for</param>
        /// <returns>NPC kill achievement condition</returns>
        public static CustomAchievementCondition Kill(ConditionReqs reqs, bool first, int id) => new NpcKillCondition(reqs, first, id);

        /// <summary>
        /// Helper to create a condition that listens for any of the NPCs to be killed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="first">True if killing the NPC for the first time</param>
        /// <param name="ids">NPC IDs to listen for</param>
        /// <returns>NPC kill achievement condition</returns>
        public static CustomAchievementCondition KillAny(ConditionReqs reqs, bool first, params int[] ids) => new NpcKillCondition(reqs, first, ids);

        /// <summary>
        /// Helper to create a condition that listens for all of the NPCs to be killed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="first">True if killing the NPC for the first time</param>
        /// <param name="ids">NPC IDs to listen for</param>
        /// <returns>NPC kill achievement conditions</returns>
        public static List<CustomAchievementCondition> KillAll(ConditionReqs reqs, bool first, params int[] ids)
        {
            List<CustomAchievementCondition> conditions = [];
            foreach (var id in ids)
                conditions.Add(new NpcKillCondition(reqs, first, id));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when an NPC is killed
        /// </summary>
        /// <param name="player">Player that killed the NPC</param>
        /// <param name="id">NPC ID that was killed</param>
        private static void CustomAchievementsHelper_OnNpcKill(Player player, int id)
        {
            if (!IsListeningForId(id, _listeners, out var conditions))
                return;

            foreach (var condition in conditions)
            {
                if (condition._first)
                {
                    // Add better check for the Twins specifically
                    if (condition.Ids.SequenceEqual(AchievementData.DefeatBoss["TWINS"]))
                    {
                        if (NPC.downedMechBoss2)
                            continue;
                    }

                    else if (Main.BestiaryTracker.Kills.GetKillCount(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[id]) > 0)
                        continue;
                }

                if (condition.Reqs.Pass(player))
                    condition.Complete();
            }
        }

        /// <summary>
        /// Listen for events so the condition can be completed
        /// <param name="condition">Achievement condition</param>
        /// <param name="first">True if killing the NPC for the first time</param>
        /// </summary>
        private static void Listen(NpcKillCondition condition, bool first)
        {
            if (!_isHooked)
            {
                CustomAchievementsHelper.OnNpcKill += CustomAchievementsHelper_OnNpcKill;
                _isHooked = true;
            }

            ListenForIds(condition, condition.Ids, _listeners);
            condition._first = first;
        }
    }
}
