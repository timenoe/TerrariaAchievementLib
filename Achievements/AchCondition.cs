using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace TerrariaAchievementLib.Achievements
{
    /// <summary>
    /// Player difficulty identifier
    /// </summary>
    public enum PlayerDifficulty
    {
        Classic,
        Mediumcore,
        Hardcore,
        Journey
    }

    /// <summary>
    /// World difficulty identifier
    /// </summary>
    public enum WorldDifficulty
    {
        Classic,
        Expert,
        Master,
        Journey
    }

    /// <summary>
    /// Special world seed identifier
    /// </summary>
    public enum SpecialSeed
    {
        None = -1,
        Bees,
        Drunk,
        Starve,
        Tenth,
        Traps,
        Up,
        Worthy,
        Zenith
    }


    /// <summary>
    /// Common condition requirements<br/><br/>
    /// Notes:<br/>
    /// If PlayerDifficulty is set to Classic, only Journey Mode cannot be used<br/>
    /// If WorldDifficulty is set to Classic, only Journey Mode cannot be used<br/>
    /// If SpecialSeed is set to None, only the 10th Anniversary seed cannot be used
    /// </summary>
    /// <param name="pdiff">Player difficulty requirement</param>
    /// <param name="wdiff">World difficulty requirement</param>
    /// <param name="sseed">Special world seed requirement</param>
    public class ConditionRequirements(PlayerDifficulty pdiff, WorldDifficulty wdiff, SpecialSeed sseed)
    {
        /// <summary>
        /// Player difficulty requirement
        /// </summary>
        public readonly PlayerDifficulty PlayerDiff = pdiff;

        /// <summary>
        /// World difficulty requirement
        /// </summary>
        public readonly WorldDifficulty WorldDiff = wdiff;

        /// <summary>
        /// Special world seed requirement
        /// </summary>
        public readonly SpecialSeed SpecialSeed = sseed;


        /// <summary>
        /// Base condition identifier (used for saving to achievements.dat)
        /// </summary>
        public string Identifier { get => $"{PlayerDiff}-{WorldDiff}-{SpecialSeed}"; }


        /// <summary>
        /// Check if the requirements pass
        /// </summary>
        /// <param name="player">Player to check</param>
        /// <returns>True if the requirements pass</returns>
        public bool Pass(Player player)
        {
            if (player != Main.LocalPlayer)
                return false;

            switch (PlayerDiff)
            {
                case PlayerDifficulty.Classic:
                    if (player.difficulty == PlayerDifficultyID.Creative)
                        return false;
                    break;

                case PlayerDifficulty.Mediumcore:
                    if (player.difficulty != PlayerDifficultyID.MediumCore && player.difficulty != PlayerDifficultyID.Hardcore)
                        return false;
                    break;

                case PlayerDifficulty.Hardcore:
                    if (player.difficulty != PlayerDifficultyID.Hardcore)
                        return false;
                    break;

                case PlayerDifficulty.Journey:
                    if (player.difficulty != PlayerDifficultyID.Creative)
                        return false;
                    break;

            }

            switch (WorldDiff)
            {
                case WorldDifficulty.Classic:
                    if (Main.GameMode == GameModeID.Creative)
                        return false;
                    break;

                case WorldDifficulty.Expert:
                    if (Main.GameMode != GameModeID.Expert && Main.GameMode != GameModeID.Master)
                        return false;
                    break;

                case WorldDifficulty.Master:
                    if (Main.GameMode != GameModeID.Master)
                        return false;
                    break;

                case WorldDifficulty.Journey:
                    if (Main.GameMode != GameModeID.Creative)
                        return false;
                    break;
            }

            switch (SpecialSeed)
            {
                case SpecialSeed.None:
                    if (SpecialSeed == SpecialSeed.Tenth)
                        return false;
                    break;

                case SpecialSeed.Bees:
                    if (!Main.notTheBeesWorld)
                        return false;
                    break;

                case SpecialSeed.Drunk:
                    if (!Main.drunkWorld)
                        return false;
                    break;

                case SpecialSeed.Starve:
                    if (!Main.dontStarveWorld)
                        return false;
                    break;

                case SpecialSeed.Tenth:
                    if (!Main.tenthAnniversaryWorld)
                        return false;
                    break;

                case SpecialSeed.Traps:
                    if (!Main.noTrapsWorld)
                        return false;
                    break;

                case SpecialSeed.Up:
                    if (!Main.remixWorld)
                        return false;
                    break;

                case SpecialSeed.Worthy:
                    if (!Main.getGoodWorld)
                        return false;
                    break;

                case SpecialSeed.Zenith:
                    if (!Main.zenithWorld)
                        return false;
                    break;
            }

            return true;
        }
    }

    /// <summary>
    /// Base class for achievement conditions that have extended requirements
    /// </summary>
    /// <param name="name">Name of the condition</param>
    /// <param name="reqs">Extra base requirements</param>
    public class AchCondition(string name, ConditionRequirements reqs) : Terraria.Achievements.AchievementCondition(name)
    {
        /// <summary>
        /// Condition requirements that must be met
        /// </summary>
        public readonly ConditionRequirements Reqs = reqs;
    }

    /// <summary>
    /// Base class for achievement conditions that are triggered by ID events
    /// </summary>
    public class AchievementIdCondition : AchCondition
    {
        /// <summary>
        /// IDs that need to be triggered to satisfy the condition
        /// </summary>
        protected readonly int[] Ids;


        /// <summary>
        /// Constructor to initialize the base condition
        /// </summary>
        protected AchievementIdCondition(string name, ConditionRequirements reqs, int[] ids) : base($"{name}_{reqs.Identifier}-{string.Join(",", ids)}", reqs) => Ids = ids;


        /// <summary>
        /// Helper to check if any conditions are listening for the ID
        /// </summary>
        protected static bool IsListeningForId<T>(int id, Dictionary<int, List<T>> listeners, out List<T> conditions) => listeners.TryGetValue(id, out conditions);

        /// <summary>
        /// Registers a condition to listen to a list of IDs
        /// </summary>
        protected void ListenForId<T>(T condition, Dictionary<int, List<T>> listeners)
        {
            // Loop through all IDs in the condition
            foreach (int id in Ids)
            {
                // Create empty list of listeners for the ID if there are none
                if (!IsListeningForId(id, listeners, out var conditions))
                {
                    conditions = [];
                    listeners.Add(id, conditions);
                }

                // Add the current condition to the listeners for this ID
                conditions.Add(condition);
            }
        }
    }
}
