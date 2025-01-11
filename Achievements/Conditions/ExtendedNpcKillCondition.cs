using System.Collections.Generic;
using Terraria;
using Terraria.Achievements;
using Terraria.GameContent.Achievements;
using Terraria.ID;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Achievement condition that is satisfied when an NPC is killed<br/><br/>
    /// Extended Optional Requirements:<br/>
    /// Game Mode<br/>
    /// Zenith Secret World Seed
    /// </summary>
    public class ExtendedNpcKillCondition : AchievementCondition
    {
        /// <summary>
        /// Base condition identifier (used for saving to achievements.dat)
        /// </summary>
        private const string Identifier = "EXTENDED_NPC_KILL";


        /// <summary>
        /// Extended game mode requirement
        /// </summary>
        public readonly int GameMode;

        /// <summary>
        /// NPC IDs that need to be killed to satisfy the condition
        /// </summary>
        public readonly short[] NpcIds;

        public readonly bool Zenith;

        /// <summary>
        /// True if the class has been hooked to the helper event
        /// </summary>
        private static bool _isHooked;

        /// <summary>
        /// NPC IDs and the conditions that are listening for them to be killed
        /// </summary>
        private static readonly Dictionary<short, List<ExtendedNpcKillCondition>> _listeners = [];


        /// <summary>
        /// Creates a condition that listens for the NPC to be killed
        /// </summary>
        /// <param name="gameMode">Game mode requirement; -1 to ignore</param>
        /// <param name="zenith">True if the world should have the Zenith secret seed</param>
        /// <param name="npcId">NPC ID to listen for</param>
        private ExtendedNpcKillCondition(int gameMode, bool zenith, short npcId) : base($"{Identifier}_{gameMode}-{zenith}-{npcId}")
        {
            GameMode = gameMode;
            Zenith = zenith;
            NpcIds = [npcId];
            ListenForNpcKill(this);
        }

        /// <summary>
        /// Creates a condition that listens for any of the NPCs to be killed
        /// </summary>
        /// <param name="gameMode">Game mode requirement; -1 to ignore</param>
        /// <param name="zenith">True if the world should have the Zenith secret seed</param>
        /// <param name="npcIds">NPC IDs to listen for</param>
        private ExtendedNpcKillCondition(int gameMode, bool zenith, short[] npcIds) : base($"{Identifier}_{gameMode}-{zenith}-{string.Join(",", npcIds)}")
        {
            GameMode = gameMode;
            Zenith = zenith;
            NpcIds = npcIds;
            ListenForNpcKill(this);
        }


        /// <summary>
        /// Helper to create a condition that listens for the NPC to be killed
        /// </summary>
        /// <param name="gameMode">Game mode requirement; -1 to ignore</param>
        /// <param name="zenith">True if the world should have the Zenith secret seed</param>
        /// <param name="npcId">NPC ID to listen for</param>
        /// <returns>NPC kill achievement condition</returns>
        public static AchievementCondition Kill(int gameMode, bool zenith, short npcId) => new ExtendedNpcKillCondition(gameMode, zenith, npcId);

        /// <summary>
        /// Helper to create a condition that listens for any of the NPCs to be killed
        /// </summary>
        /// <param name="gameMode">Game mode requirement; -1 to ignore</param>
        /// <param name="zenith">True if the world should have the Zenith secret seed</param>
        /// <param name="npcIds">NPC IDs to listen for</param>
        /// <returns>NPC kill achievement condition</returns>
        public static AchievementCondition KillAny(int gameMode, bool zenith, params short[] npcIds) => new ExtendedNpcKillCondition(gameMode, zenith, npcIds);

        /// <summary>
        /// Helper to create conditions that listens for all of the NPCs to be killed
        /// </summary>
        /// <param name="gameMode">Game mode requirement; -1 to ignore</param>
        /// <param name="zenith">True if the world should have the Zenith secret seed</param>
        /// <param name="npcIds">NPC IDs</param>
        /// <returns>NPC kill achievement conditions</returns>
        public static List<AchievementCondition> KillAll(int gameMode, bool zenith, params short[] npcIds)
        {
            List<AchievementCondition> conditions = [];
            foreach (short npcId in npcIds)
                conditions.Add(new ExtendedNpcKillCondition(gameMode, zenith, npcId));
            return conditions;
        }

        /// <summary>
        /// Checks if any conditions are listening for a NPC to be killed
        /// </summary>
        /// <param name="npcId">NPC ID to check</param>
        /// <param name="conditions">Conditions that are waiting for the NPC to be killed</param>
        /// <returns>True if there are conditions waiting for the NPC to be killed</returns>
        private static bool IsListeningForNpcKill(short npcId, out List<ExtendedNpcKillCondition> conditions) => _listeners.TryGetValue(npcId, out conditions);

        /// <summary>
        /// Add condition to the list of NPC death listeners
        /// </summary>
        /// <param name="condition">Condition to add to the list of NPC death listeners</param>
        private static void ListenForNpcKill(ExtendedNpcKillCondition condition)
        {
            // Hook the helper event only once for the class
            if (!_isHooked)
            {
                AchievementsHelper.OnNPCKilled += AchievementsHelper_OnNPCKilled;
                _isHooked = true;
            }

            // Loop through all NPCs in the condition
            foreach (short npcId in condition.NpcIds)
            {
                // Create empty list of listeners for the NPC if there are none
                if (!IsListeningForNpcKill(npcId, out var conditions))
                {
                    conditions = [];
                    _listeners[npcId] = conditions;
                }

                // Add the current condition to the listeners for this NPC
                conditions.Add(condition);
            }
        }

        /// <summary>
        /// Hook that is called when an NPC is killed
        /// </summary>
        /// <param name="player">Player that killed the NPC</param>
        /// <param name="npcId">NPC ID that was killed</param>
        private static void AchievementsHelper_OnNPCKilled(Player player, short npcId)
        {
            if (player != Main.LocalPlayer)
                return;

            if (!IsListeningForNpcKill(npcId, out var conditions))
                return;

            foreach (var condition in conditions)
            {
                if (condition.CheckGameMode() && condition.CheckZenith())
                    condition.Complete();
            }
        }

        /// <summary>
        /// Checks if the current game mode satisfies the requirement
        /// </summary>
        /// <returns>True if the current game mode satisfies the requirement</returns>
        private bool CheckGameMode() => GameMode < GameModeID.Normal || GameMode == Main.GameMode;

        /// <summary>
        /// Checks for the Zenith secret world seed if applicable
        /// </summary>
        /// <returns>True if the world seed is Zenith if applicable</returns>
        private bool CheckZenith() => !Zenith || Main.zenithWorld;
    }
}
