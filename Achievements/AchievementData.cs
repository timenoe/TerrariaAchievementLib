using System.Collections.Generic;
using Terraria.ID;

namespace TerrariaAchievementLib.Achievements
{
    /// <summary>
    /// Data that is commonly used in achievements
    /// </summary>
    public class AchievementData
    {
        /// <summary>
        /// NPC IDs that must be killed for a boss to be considered defeated<br/>
        /// If an entry has more than 1 ID, any of them can be defeated
        /// </summary>
        public static readonly Dictionary<string, int[]> DefeatBoss = new()
        {
            { "KING_SLIME",       [NPCID.KingSlime] },
            { "EYE_OF_CTHULHU",   [NPCID.EyeofCthulhu] },
            { "EATER_OF_WORLDS",  [NPCID.EaterofWorldsBody, NPCID.EaterofWorldsHead, NPCID.EaterofWorldsTail] },
            { "BRAIN_OF_CTHULHU", [NPCID.BrainofCthulhu] },
            { "QUEEN_BEE",        [NPCID.QueenBee] },
            { "SKELETRON",        [NPCID.SkeletronHead] },
            { "DEERCLOPS",        [NPCID.Deerclops] },
            { "WALL_OF_FLESH",    [NPCID.WallofFlesh, NPCID.WallofFleshEye] },
            { "QUEEN_SLIME",      [NPCID.QueenSlimeBoss] },
            { "TWINS",            [NPCID.Retinazer, NPCID.Spazmatism] },
            { "DESTROYER",        [NPCID.TheDestroyer] },
            { "SKELETRON_PRIME",  [NPCID.SkeletronPrime] },
            { "PLANTERA",         [NPCID.Plantera] },
            { "GOLEM",            [NPCID.Golem] },
            { "EMPRESS_OF_LIGHT", [NPCID.HallowBoss] },
            { "DUKE_FISHRON",     [NPCID.DukeFishron] },
            { "LUNATIC_CULTIST",  [NPCID.CultistBoss] },
            { "SOLAR_PILLAR",     [NPCID.LunarTowerSolar] },
            { "NEBULA_PILLAR",    [NPCID.LunarTowerNebula] },
            { "VORTEX_PILLAR",    [NPCID.LunarTowerVortex] },
            { "STARDUST_PILLAR",  [NPCID.LunarTowerStardust] },
            { "MOON_LORD",        [NPCID.MoonLordCore] },
            { "MOURNING_WOOD",    [NPCID.MourningWood] },
            { "PUMPKING",         [NPCID.Pumpking] },
            { "EVERSCREAM",       [NPCID.Everscream] },
            { "SANTA-NK1",        [NPCID.SantaNK1] },
            { "ICE_QUEEN",        [NPCID.IceQueen] },
            { "DARK_MAGE_T3",     [NPCID.DD2DarkMageT3] },
            { "OGRE_T3",          [NPCID.DD2OgreT3] },
            { "BETSY",            [NPCID.DD2Betsy] },
            { "FLYING_DUTCHMAN",  [NPCID.PirateShip] },
            { "MARTIAN_SAUCER",   [NPCID.MartianSaucer, NPCID.MartianSaucerCore] }
        };

        /// <summary>
        /// All treasure bags obtained from specific bosses
        /// </summary>
        public static readonly Dictionary<string, int> TreasureBags = new()
        {
            { "KING_SLIME",       ItemID.KingSlimeBossBag },
            { "EYE_OF_CTHULHU",   ItemID.EyeOfCthulhuBossBag },
            { "EATER_OF_WORLDS",  ItemID.EaterOfWorldsBossBag },
            { "BRAIN_OF_CTHULHU", ItemID.BrainOfCthulhuBossBag },
            { "QUEEN_BEE",        ItemID.QueenBeeBossBag },
            { "SKELETRON",        ItemID.SkeletronBossBag },
            { "DEERCLOPS",        ItemID.DeerclopsBossBag },
            { "WALL_OF_FLESH",    ItemID.WallOfFleshBossBag },
            { "QUEEN_SLIME",      ItemID.QueenSlimeBossBag },
            { "TWINS",            ItemID.TwinsBossBag },
            { "DESTROYER",        ItemID.DestroyerBossBag },
            { "SKELETRON_PRIME",  ItemID.SkeletronPrimeBossBag },
            { "PLANTERA",         ItemID.PlanteraBossBag },
            { "GOLEM",            ItemID.GolemBossBag },
            { "EMPRESS_OF_LIGHT", ItemID.FairyQueenBossBag },
            { "DUKE_FISHRON",     ItemID.FishronBossBag },
            { "MOON_LORD",        ItemID.MoonLordBossBag },
            { "BETSY",            ItemID.BossBagBetsy }
        };

        /// <summary>
        /// Extended item slot context IDs
        /// </summary>
        public class CustomItemSlotContextID
        {
            public const int EquipWings = 1337;
        }
    }
}