using Terraria;
using Terraria.Achievements;
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

    public class CustomAchievementCondition(string name, ConditionRequirements reqs) : AchievementCondition(name)
    {
        /// <summary>
        /// Condition requirements that must be met
        /// </summary>
        public readonly ConditionRequirements Reqs = reqs;
    }
}
