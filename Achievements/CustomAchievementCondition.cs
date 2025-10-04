using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using TerrariaAchievementLib.Achievements.Conditions;
using TerrariaAchievementLib.Players;

namespace TerrariaAchievementLib.Achievements
{
    /// <summary>
    /// Player difficulty identifier
    /// </summary>
    public enum PlayerDiff
    {
        Classic,
        Mediumcore,
        Hardcore,
        Journey,
        Hardestcore
    }

    /// <summary>
    /// World difficulty identifier
    /// </summary>
    public enum WorldDiff
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

        // Main
        Bees,
        Drunk,
        Starve,
        Tenth,
        Traps,
        Up,
        Worthy,
        Zenith,

        // Extras
        Unworthy
    }

    public class ConditionHelper
    {
        /// <summary>
        /// Helper to add multiple conditions to a ModAchievement
        /// </summary>
        /// <param name="ach">ModAchievement</param>
        /// <param name="conds">Conditions to add to the mod achievement</param>
        public static void AddConditions(ModAchievement ach, List<CustomAchievementCondition> conds)
        {
            foreach (var condition in conds)
                ach.AddCondition(condition);
        }

        /// <summary>
        /// Returns achievement conditions to replace a vanilla achievement
        /// </summary>
        /// <param name="key">Vanilla achievement key</param>
        /// <param name="reqs">Condition requirements</param>
        /// <returns></returns>
        public static List<CustomAchievementCondition> GetVanillaAchievementConditions(string key, ConditionReqs reqs)
        {
            List<CustomAchievementCondition> conds = [];

            switch (key)
            {
                case "TIMBER":
                    int[] wood = [9, 619, 2504, 620, 2503, 2260, 621, 911, 1729, 5215];
                    return [ItemGrabCondition.GrabAny(reqs, wood)];

                case "BENCHED":
                    int[] workbenches = ItemID.Sets.Workbenches.Select(s => (int)s).ToArray();
                    return [ItemCraftCondition.CraftAny(reqs, workbenches)];

                case "NO_HOBO":
                    return [FlagProgressionCondition.Set(reqs, AchievementHelperID.Events.NPCMovedIn)];

                case "OBTAIN_HAMMER":
                    int[] hammers = [2775, 2746, 5283, 3505, 654, 3517, 7, 3493, 2780, 1513, 2516, 660, 3481, 657, 922, 3511, 2785, 3499, 3487, 196, 367, 104, 797, 2320, 787, 1234, 1262, 3465, 204, 217, 1507, 3524, 3522, 3525, 3523, 4317, 1305];
                    return [ItemGrabCondition.GrabAny(reqs, hammers)];

                case "OOO_SHINY":
                    int[] ore = [7, 6, 9, 8, 166, 167, 168, 169, 22, 204, 58, 107, 108, 111, 221, 222, 223, 211];
                    return [TileDestroyCondition.DestroyAny(reqs, ore)];

                case "HEART_BREAKER":
                    return [TileDestroyCondition.Destroy(reqs, TileID.Heart)];

                case "HEAVY_METAL":
                    return [ItemGrabCondition.GrabAny(reqs, [ItemID.IronAnvil, ItemID.LeadAnvil])];

                case "I_AM_LOOT":
                    return [FlagSpecialCondition.Set(reqs, AchievementHelperID.Special.PeekInGoldenChest)];

                case "STAR_POWER":
                    return [FlagSpecialCondition.Set(reqs, AchievementHelperID.Special.ConsumeStar)];

                case "HOLD_ON_TIGHT":
                    return [ItemEquipCondition.Equip(reqs, ItemSlot.Context.EquipGrapple, ItemID.None)];

                case "EYE_ON_YOU":
                    return [NpcKillCondition.Kill(reqs, true, NPCID.EyeofCthulhu)];

                case "SMASHING_POPPET":
                    return [FlagProgressionCondition.Set(reqs, AchievementHelperID.Events.SmashShadowOrb)];

                case "WHERES_MY_HONEY":
                    return [FlagSpecialCondition.Set(reqs, AchievementHelperID.Special.FoundBeeHive)];

                case "STING_OPERATION":
                    return [NpcKillCondition.Kill(reqs, true, NPCID.QueenBee)];

                case "BONED":
                    return [NpcKillCondition.Kill(reqs, true, NPCID.SkeletronHead)];

                case "DUNGEON_HEIST":
                    conds.Add(ItemGrabCondition.Grab(reqs, ItemID.GoldenKey));
                    conds.Add(FlagProgressionCondition.Set(reqs, AchievementHelperID.Events.UnlockedGoldenChest));
                    return conds;

                case "ITS_GETTING_HOT_IN_HERE":
                    return [FlagSpecialCondition.Set(reqs, AchievementHelperID.Special.FoundHell)];

                case "MINER_FOR_FIRE":
                    return [ItemCraftCondition.Craft(reqs, ItemID.MoltenPickaxe)];

                case "STILL_HUNGRY":
                    return [NpcKillCondition.KillAny(reqs, true, [NPCID.WallofFlesh, NPCID.WallofFleshEye])];

                case "ITS_HARD":
                    return [FlagProgressionCondition.Set(reqs, AchievementHelperID.Events.StartHardmode)];

                case "BEGONE_EVIL":
                    return [FlagProgressionCondition.Set(reqs, AchievementHelperID.Events.SmashDemonAltar)];

                case "EXTRA_SHINY":
                    int[] hardmodeOre = [107, 108, 111, 221, 222, 223];
                    return [TileDestroyCondition.DestroyAny(reqs, hardmodeOre)];

                case "HEAD_IN_THE_CLOUDS":
                    return [ItemEquipCondition.Equip(reqs, AchievementData.CustomItemSlotContextID.EquipWings, ItemID.None)];

                case "BUCKET_OF_BOLTS":
                    conds.Add(NpcKillCondition.KillAny(reqs, false, [NPCID.Retinazer, NPCID.Spazmatism]));
                    conds.AddRange(NpcKillCondition.KillAll(reqs, false, [NPCID.TheDestroyer, NPCID.SkeletronPrime]));
                    return conds;

                case "DRAX_ATTAX":
                    return [ItemCraftCondition.CraftAny(reqs, [ItemID.Drax, ItemID.PickaxeAxe])];

                case "PHOTOSYNTHESIS":
                    return [TileDestroyCondition.Destroy(reqs, TileID.Chlorophyte)];

                case "GET_A_LIFE":
                    return [FlagSpecialCondition.Set(reqs, AchievementHelperID.Special.ConsumeFruit)];

                case "THE_GREAT_SOUTHERN_PLANTKILL":
                    return [NpcKillCondition.Kill(reqs, true, NPCID.Plantera)];

                case "TEMPLE_RAIDER":
                    return [FlagProgressionCondition.Set(reqs, AchievementHelperID.Events.TempleRaider)];

                case "LIHZAHRDIAN_IDOL":
                    return [NpcKillCondition.Kill(reqs, true, NPCID.Golem)];

                case "ROBBING_THE_GRAVE":
                    int[] dungeonItems = [1513, 938, 963, 977, 1300, 1254, 1514, 679, 759, 1446, 1445, 1444, 1183, 1266, 671, 3291, 4679];
                    return [ItemGrabCondition.GrabAny(reqs, dungeonItems)];

                case "OBSESSIVE_DEVOTION":
                    return [NpcKillCondition.Kill(reqs, true, NPCID.CultistBoss)];

                case "STAR_DESTROYER":
                    return NpcKillCondition.KillAll(reqs, true, [NPCID.LunarTowerNebula, NPCID.LunarTowerSolar, NPCID.LunarTowerStardust, NPCID.LunarTowerVortex]);

                case "CHAMPION_OF_TERRARIA":
                    return [NpcKillCondition.Kill(reqs, true, NPCID.MoonLordCore)];

                default:
                    return [];
            }
        }
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
    public class ConditionReqs(PlayerDiff pdiff, WorldDiff wdiff, SpecialSeed sseed)
    {
        /// <summary>
        /// Player difficulty requirement
        /// </summary>
        public readonly PlayerDiff PlayerDiff = pdiff;

        /// <summary>
        /// World difficulty requirement
        /// </summary>
        public readonly WorldDiff WorldDiff = wdiff;

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
                case PlayerDiff.Classic:
                    if (player.difficulty == PlayerDifficultyID.Creative)
                        return false;
                    break;

                case PlayerDiff.Mediumcore:
                    if (player.difficulty != PlayerDifficultyID.MediumCore && player.difficulty != PlayerDifficultyID.Hardcore)
                        return false;
                    break;

                case PlayerDiff.Hardcore:
                    if (player.difficulty != PlayerDifficultyID.Hardcore)
                        return false;
                    break;

                case PlayerDiff.Journey:
                    if (player.difficulty != PlayerDifficultyID.Creative)
                        return false;
                    break;

                case PlayerDiff.Hardestcore:
                    if (player.difficulty != PlayerDifficultyID.Hardcore || !player.GetModPlayer<HardestcorePlayer>().CanEarnAchievement())
                        return false;
                    break;
            }

            switch (WorldDiff)
            {
                case WorldDiff.Classic:
                    if (Main.GameMode == GameModeID.Creative)
                        return false;
                    break;

                case WorldDiff.Expert:
                    if (Main.GameMode != GameModeID.Expert && Main.GameMode != GameModeID.Master)
                        return false;
                    break;

                case WorldDiff.Master:
                    if (Main.GameMode != GameModeID.Master)
                        return false;
                    break;

                case WorldDiff.Journey:
                    if (Main.GameMode != GameModeID.Creative)
                        return false;
                    break;
            }

            switch (SpecialSeed)
            {
                case SpecialSeed.None:
                    if (Main.tenthAnniversaryWorld && !Main.zenithWorld)
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

                case SpecialSeed.Unworthy:
                    if (Main.getGoodWorld)
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
    public class CustomAchievementCondition(string name, ConditionReqs reqs) : Terraria.Achievements.AchievementCondition(name)
    {
        /// <summary>
        /// Condition requirements that must be met
        /// </summary>
        public readonly ConditionReqs Reqs = reqs;
    }

    /// <summary>
    /// Base class for achievement conditions that are triggered by ID events
    /// </summary>
    public class AchIdCondition : CustomAchievementCondition
    {
        /// <summary>
        /// Conditions that track buffs
        /// </summary>
        public static readonly string[] BuffConditions = ["BuffAddCondition"];

        /// <summary>
        /// Conditions that track items
        /// </summary>
        public static readonly string[] ItemConditions = ["ItemCatchCondition", "ItemCraftCondition", "ItemEquipCondition", "ItemExtractCondition", "ItemGrabCondition", "ItemOpenCondition", "ItemShakeCondition", "ItemUseCondition", "NpcBuyCondition", "NpcDropCondition", "NpcGiftCondition"];

        /// <summary>
        /// Conditions that track NPCs
        /// </summary>
        public static readonly string[] NpcConditions = ["NpcCatchCondition", "NpcHappyCondition", "NpcKillCondition", "NpcShimmerCondition"];

        /// <summary>
        /// IDs that need to be triggered to satisfy the condition
        /// </summary>
        protected readonly int[] Ids;


        /// <summary>
        /// Constructor to initialize the base condition
        /// </summary>
        protected AchIdCondition(string name, ConditionReqs reqs, int[] ids) : base($"{name}_{reqs.Identifier}-{string.Join(",", ids)}", reqs) => Ids = ids;


        /// <summary>
        /// Helper to check if any conditions are listening for the ID
        /// </summary>
        protected static bool IsListeningForId<T>(int id, Dictionary<int, List<T>> listeners, out List<T> conditions) => listeners.TryGetValue(id, out conditions);

        /// <summary>
        /// Registers a condition to listen to a list of IDs
        /// </summary>
        protected static void ListenForIds<T>(T condition, int[] ids, Dictionary<int, List<T>> listeners)
        {
            // Loop through all IDs in the condition
            foreach (int id in ids)
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
