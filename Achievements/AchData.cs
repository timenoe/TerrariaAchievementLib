using System.Collections.Generic;
using Terraria.ID;

namespace TerrariaAchievementLib.Achievements
{
    /// <summary>
    /// Data that is commonly used in achievements
    /// </summary>
    public class AchData
    {
        /// <summary>
        /// NPC IDs that must be killed for a boss to be considered defeated<br/>
        /// Most only have 1 ID that need to be defeated<br/>
        /// If an entry has more than 1 ID, any of them can be defeated
        /// </summary>
        public static readonly Dictionary<string, int[]> DefeatBossIds = new()
        {
            { "King Slime", [NPCID.KingSlime] },
            { "Eye of Cthulhu", [NPCID.EyeofCthulhu] },
            { "Eater of Worlds", [NPCID.EaterofWorldsBody, NPCID.EaterofWorldsHead, NPCID.EaterofWorldsTail] },
            { "Brain of Cthulhu", [NPCID.BrainofCthulhu] },
            { "Queen Bee", [NPCID.QueenBee] },
            { "Skeletron", [NPCID.SkeletronHead] },
            { "Deerclops", [NPCID.Deerclops] },
            { "Wall of Flesh", [NPCID.WallofFlesh, NPCID.WallofFleshEye] },
            { "Queen Slime", [NPCID.QueenSlimeBoss] },
            { "Destroyer", [NPCID.TheDestroyer] },
            { "Twins", [NPCID.Retinazer, NPCID.Spazmatism] },
            { "Skeletron Prime", [NPCID.SkeletronPrime] },
            { "Plantera", [NPCID.Plantera] },
            { "Golem", [NPCID.Golem] },
            { "Duke Fishron", [NPCID.DukeFishron] },
            { "Empress of Light", [NPCID.HallowBoss] },
            { "Lunatic Cultist", [NPCID.CultistBoss] },
            { "Moon Lord", [NPCID.MoonLordCore] },
            { "Dark Mage (T3)", [NPCID.DD2DarkMageT3] },
            { "Ogre (T3)", [NPCID.DD2OgreT3] },
            { "Betsy", [NPCID.DD2Betsy] },
            { "Flying Dutchman", [NPCID.PirateShip] },
            { "Mourning Wood", [NPCID.MourningWood] },
            { "Pumpking", [NPCID.Pumpking] },
            { "Everscream", [NPCID.Everscream] },
            { "Santa-NK1", [NPCID.SantaNK1] },
            { "Ice Queen", [NPCID.IceQueen] },
            { "Martian Saucer", [NPCID.MartianSaucer] },
            { "Solar Pillar", [NPCID.LunarTowerSolar] },
            { "Nebula Pillar", [NPCID.LunarTowerNebula] },
            { "Vortex Pillar", [NPCID.LunarTowerVortex] },
            { "Stardust Pillar", [NPCID.LunarTowerStardust] },
        };
    }
}
