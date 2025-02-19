using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace TerrariaAchievementLib.Achievements
{
    /// <summary>
    /// Identifies progression item
    /// </summary>
    public enum ProgressionElement
    {
        Usable,
        Equippable,
        Ammo,
        Buff
    }

    /// <summary>
    /// Identifies progression state
    /// </summary>
    public enum ProgressionState
    {
        PreHardmode,
        Hardmode,
        PostMechanicalTrio,
        PostPlantera,
        PostGolem,
        PostLunaticCultist,
        PostMoonLord
    }

    /// <summary>
    /// Identifies a specific progression restriction
    /// </summary>
    public enum ProgressionRestriction
    {
        All,
        Player,
        World
    }

    /// <summary>
    /// Checks if certain items should be blocked based on current world progression
    /// </summary>
    public class AchievementProgression
    {
        /// <summary>
        /// True if items should be blocked based on the current world progression
        /// </summary>
        private static bool _enabled;

        /// <summary>
        /// Determines when items should be blocked
        /// </summary>
        private static ProgressionRestriction _restriction;
        
        /// <summary>
        /// Defines progression consumables
        /// </summary>
        private static readonly Dictionary<ProgressionState, int[]> ProgressionConsumable = new()
        {
            { ProgressionState.Hardmode, [ItemID.LifeFruit, ItemID.DemonHeart, ItemID.AegisFruit, ItemID.CombatBookVolumeTwo, ItemID.MinecartPowerup, ItemID.GreaterHealingPotion, ItemID.GreaterManaPotion, ItemID.SuperManaPotion, ItemID.FlaskofCursedFlames, ItemID.FlaskofIchor] },
            { ProgressionState.PostMechanicalTrio, [] },
            { ProgressionState.PostPlantera, [ItemID.FlaskofNanites, ItemID.FlaskofVenom] },
            { ProgressionState.PostGolem, [] },
            { ProgressionState.PostLunaticCultist, [ItemID.SuperHealingPotion] },
            { ProgressionState.PostMoonLord, [] },
        };

        /// <summary>
        /// Defines progression weapons
        /// </summary>
        private static readonly Dictionary<ProgressionState, int[]> ProgressionWeapons = new()
        {
            { ProgressionState.Hardmode, [ItemID.PearlwoodSword, ItemID.TaxCollectorsStickOfDoom, ItemID.SlapHand, ItemID.CobaltSword, ItemID.PalladiumSword, ItemID.BluePhasesaber, ItemID.GreenPhasesaber, ItemID.OrangePhasesaber, ItemID.PurplePhasesaber, ItemID.RedPhasesaber, ItemID.WhitePhasesaber, ItemID.YellowPhasesaber, ItemID.IceSickle, ItemID.DD2SquireDemonSword, ItemID.MythrilSword, ItemID.OrichalcumSword, ItemID.BreakerBlade, ItemID.Cutlass, ItemID.Frostbrand, ItemID.AdamantiteSword, ItemID.BeamSword, ItemID.TitaniumSword, ItemID.FetidBaghnakhs, ItemID.Bladetongue, ItemID.HamBat, ItemID.Excalibur, ItemID.WaffleIron, ItemID.DeathSickle, ItemID.TrueNightsEdge, ItemID.FormatC, ItemID.Gradient, ItemID.Chik, ItemID.HelFire, ItemID.Amarok, ItemID.Code2, ItemID.Yelets, ItemID.RedsYoyo, ItemID.ValkyrieYoyo, ItemID.CobaltNaginata, ItemID.PalladiumPike, ItemID.MythrilHalberd, ItemID.OrichalcumHalberd, ItemID.AdamantiteGlaive, ItemID.TitaniumTrident, ItemID.Gungnir, ItemID.MonkStaffT2, ItemID.MushroomSpear, ItemID.ObsidianSwordfish, ItemID.FlyingKnife, ItemID.BouncingShield, ItemID.LightDisc, ItemID.Bananarang, ItemID.DripplerFlail, ItemID.DaoofPow, ItemID.Anchor, ItemID.ChainGuillotines, ItemID.KOCannon, ItemID.Flairon, ItemID.Arkhalis, ItemID.JoustingLance, ItemID.ShadowFlameKnife, ItemID.HallowJoustingLance, ItemID.MonkStaffT1, ItemID.PearlwoodBow, ItemID.Marrow, ItemID.IceBow, ItemID.DaedalusStormbow, ItemID.ShadowFlameBow, ItemID.DD2PhoenixBow, ItemID.PulseBow, ItemID.Tsunami, ItemID.CobaltRepeater, ItemID.PalladiumRepeater, ItemID.MythrilRepeater, ItemID.OrichalcumRepeater, ItemID.AdamantiteRepeater, ItemID.TitaniumRepeater, ItemID.HallowedRepeater, ItemID.ClockworkAssaultRifle, ItemID.Gatligator, ItemID.Shotgun, ItemID.OnyxBlaster, ItemID.Uzi, ItemID.Megashark, ItemID.Toxikarp, ItemID.DartPistol, ItemID.DartRifle, ItemID.CoinGun, ItemID.Flamethrower, ItemID.SuperStarCannon, ItemID.SkyFracture, ItemID.CrystalSerpent, ItemID.FlowerofFrost, ItemID.FrostStaff, ItemID.CrystalVileShard, ItemID.SoulDrain, ItemID.MeteorStaff, ItemID.PoisonStaff, ItemID.RainbowRod, ItemID.UnholyTrident, ItemID.BookStaff, ItemID.LaserRifle, ItemID.ZapinatorOrange, ItemID.BubbleGun, ItemID.CursedFlames, ItemID.GoldenShower, ItemID.CrystalStorm, ItemID.RazorbladeTyphoon, ItemID.IceRod, ItemID.ClingerStaff, ItemID.NimbusRod, ItemID.MagicDagger, ItemID.MedusaHead, ItemID.SpiritFlame, ItemID.ShadowFlameHexDoll, ItemID.SharpTears, ItemID.MagicalHarp, ItemID.Smolstar, ItemID.SpiderStaff, ItemID.PirateStaff, ItemID.SanguineStaff, ItemID.OpticStaff, ItemID.TempestStaff, ItemID.QueenSpiderStaff, ItemID.DD2LightningAuraT2Popper, ItemID.DD2FlameburstTowerT2Popper, ItemID.DD2ExplosiveTrapT2Popper, ItemID.DD2BallistraTowerT2Popper, ItemID.FireWhip, ItemID.CoolWhip, ItemID.SwordWhip, ItemID.RedRocket, ItemID.GreenRocket, ItemID.BlueRocket, ItemID.YellowRocket, ItemID.Cannon, ItemID.BunnyCannon, ItemID.HolyWater, ItemID.WaffleIron] },
            { ProgressionState.PostMechanicalTrio, [ItemID.TrueExcalibur, ItemID.ChlorophyteSaber, ItemID.ChlorophyteClaymore, ItemID.ChlorophytePartisan, ItemID.ChlorophyteShotbow, ItemID.VenomStaff] },
            { ProgressionState.PostPlantera, [ItemID.PsychoKnife, ItemID.Keybrand, ItemID.TheHorsemansBlade, ItemID.ChristmasTreeSword, ItemID.Seedler, ItemID.TerraBlade, ItemID.Kraken, ItemID.TheEyeOfCthulhu, ItemID.NorthPole, ItemID.PaladinsHammer, ItemID.FlowerPow, ItemID.ScourgeoftheCorruptor, ItemID.VampireKnives, ItemID.ShadowJoustingLance, ItemID.PiercingStarlight, ItemID.FairyQueenRangedItem, ItemID.StakeLauncher, ItemID.VenusMagnum, ItemID.TacticalShotgun, ItemID.SniperRifle, ItemID.CandyCornRifle, ItemID.ChainGun, ItemID.GrenadeLauncher, ItemID.ProximityMineLauncher, ItemID.RocketLauncher, ItemID.NailGun, ItemID.JackOLanternLauncher, ItemID.PiranhaGun, ItemID.ElfMelter, ItemID.NettleBurst, ItemID.BatScepter, ItemID.BlizzardStaff, ItemID.InfernoFork, ItemID.ShadowbeamStaff, ItemID.SpectreStaff, ItemID.PrincessWeapon, ItemID.Razorpine, ItemID.WaspGun, ItemID.LeafBlower, ItemID.RainbowGun, ItemID.MagnetSphere, ItemID.ToxicFlask, ItemID.FairyQueenMagicItem, ItemID.SparkleGuitar, ItemID.DeadlySphereStaff, ItemID.PygmyStaff, ItemID.RavenStaff, ItemID.StormTigerStaff, ItemID.EmpressBlade, ItemID.StaffoftheFrostHydra, ItemID.ScytheWhip, ItemID.MaceWhip, ItemID.RainbowWhip, ItemID.LandMine] },
            { ProgressionState.PostGolem, [ItemID.DD2SquireBetsySword, ItemID.InfluxWaver, ItemID.PossessedHatchet, ItemID.GolemFist, ItemID.MonkStaffT3, ItemID.DD2BetsyBow, ItemID.Xenopopper, ItemID.Stynger, ItemID.FireworksLauncher, ItemID.ElectrosphereLauncher, ItemID.StaffofEarth, ItemID.ApprenticeStaffT3, ItemID.HeatRay, ItemID.LaserMachinegun, ItemID.ChargedBlasterCannon, ItemID.XenoStaff, ItemID.DD2LightningAuraT3Popper, ItemID.DD2FlameburstTowerT3Popper, ItemID.DD2ExplosiveTrapT3Popper, ItemID.DD2BallistraTowerT3Popper] },
            { ProgressionState.PostLunaticCultist, [ItemID.DayBreak, ItemID.SolarEruption, ItemID.Phantasm, ItemID.VortexBeater, ItemID.NebulaArcanum, ItemID.NebulaBlaze, ItemID.StardustCellStaff, ItemID.StardustDragonStaff] },
            { ProgressionState.PostMoonLord, [ItemID.StarWrath, ItemID.Meowmere, ItemID.Terrarian, ItemID.Zenith, ItemID.SDMG, ItemID.Celeb2, ItemID.LunarFlareBook, ItemID.LastPrism, ItemID.MoonlordTurretStaff, ItemID.RainbowCrystalStaff] },
        };

        /// <summary>
        /// Defines progression tools
        /// </summary>
        private static readonly Dictionary<ProgressionState, int[]> ProgressionTools = new()
        {
            { ProgressionState.Hardmode, [ItemID.CobaltPickaxe, ItemID.CobaltDrill, ItemID.PalladiumPickaxe, ItemID.PalladiumDrill, ItemID.MythrilPickaxe, ItemID.MythrilDrill, ItemID.OrichalcumPickaxe, ItemID.OrichalcumDrill, ItemID.AdamantitePickaxe, ItemID.AdamantiteDrill, ItemID.TitaniumPickaxe, ItemID.TitaniumDrill, ItemID.PickaxeAxe, ItemID.Drax, ItemID.PalladiumWaraxe, ItemID.PalladiumChainsaw, ItemID.MythrilWaraxe, ItemID.MythrilChainsaw, ItemID.OrichalcumWaraxe, ItemID.OrichalcumChainsaw, ItemID.AdamantiteWaraxe, ItemID.AdamantiteChainsaw, ItemID.TitaniumWaraxe, ItemID.TitaniumChainsaw, ItemID.PickaxeAxe, ItemID.BloodHamaxe, ItemID.Hammush, ItemID.RodofDiscord] },
            { ProgressionState.PostMechanicalTrio, [ItemID.ChlorophytePickaxe, ItemID.ChlorophyteDrill, ItemID.ChlorophyteGreataxe, ItemID.ChlorophyteChainsaw, ItemID.ChlorophyteWarhammer, ItemID.ChlorophyteJackhammer] },
            { ProgressionState.PostPlantera, [ItemID.SpectrePickaxe, ItemID.ShroomiteDiggingClaw, ItemID.ButchersChainsaw, ItemID.SpectreHamaxe, ItemID.TheAxe] },
            { ProgressionState.PostGolem, [ItemID.Picksaw, ItemID.LaserDrill, ItemID.Picksaw] },
            { ProgressionState.PostLunaticCultist, [] },
            { ProgressionState.PostMoonLord, [ItemID.VortexPickaxe, ItemID.NebulaPickaxe, ItemID.SolarFlarePickaxe, ItemID.StardustPickaxe, ItemID.VortexDrill, ItemID.NebulaDrill, ItemID.SolarFlareDrill, ItemID.StardustDrill, ItemID.LunarHamaxeSolar, ItemID.LunarHamaxeVortex, ItemID.LunarHamaxeNebula, ItemID.LunarHamaxeStardust, ItemID.RodOfHarmony, ItemID.PortalGun] },
        };

        /// <summary>
        /// Defines progression ammo
        /// </summary>
        private static readonly Dictionary<ProgressionState, int[]> ProgressionAmmo = new()
        {
            { ProgressionState.Hardmode, [ItemID.CrystalBullet, ItemID.CursedBullet, ItemID.HighVelocityBullet, ItemID.IchorBullet, ItemID.ExplodingBullet, ItemID.GoldenBullet, ItemID.EndlessMusketPouch, ItemID.CursedArrow, ItemID.IchorArrow, ItemID.EndlessQuiver, ItemID.CrystalDart, ItemID.CursedDart, ItemID.IchorDart] },
            { ProgressionState.PostMechanicalTrio, [ItemID.ChlorophyteBullet, ItemID.ChlorophyteArrow] },
            { ProgressionState.PostPlantera, [ItemID.VenomBullet, ItemID.NanoBullet, ItemID.VenomArrow, ItemID.RocketI, ItemID.RocketII, ItemID.RocketIII, ItemID.RocketIV, ItemID.ClusterRocketI, ItemID.ClusterRocketII, ItemID.DryRocket, ItemID.WetRocket, ItemID.LavaRocket, ItemID.HoneyRocket, ItemID.MiniNukeI, ItemID.MiniNukeII] },
            { ProgressionState.PostGolem, [] },
            { ProgressionState.PostLunaticCultist, [] },
            { ProgressionState.PostMoonLord, [ItemID.MoonlordBullet, ItemID.MoonlordArrow] },
        };

        /// <summary>
        /// Defines progression armor
        /// </summary>
        private static readonly Dictionary<ProgressionState, int[]> ProgressionArmor = new()
        {
            { ProgressionState.Hardmode, [ItemID.DjinnsCurse, ItemID.PearlwoodHelmet, ItemID.PearlwoodBreastplate, ItemID.PearlwoodGreaves, ItemID.SpiderMask, ItemID.SpiderBreastplate, ItemID.SpiderGreaves, ItemID.CobaltHat, ItemID.CobaltHelmet, ItemID.CobaltMask, ItemID.CobaltBreastplate, ItemID.CobaltLeggings, ItemID.PalladiumHeadgear, ItemID.PalladiumMask, ItemID.PalladiumHelmet, ItemID.PalladiumBreastplate, ItemID.PalladiumLeggings, ItemID.MythrilHood, ItemID.MythrilHelmet, ItemID.MythrilHat, ItemID.MythrilChainmail, ItemID.MythrilGreaves, ItemID.OrichalcumHeadgear, ItemID.OrichalcumMask, ItemID.OrichalcumHelmet, ItemID.OrichalcumBreastplate, ItemID.OrichalcumLeggings, ItemID.AdamantiteHeadgear, ItemID.AdamantiteHelmet, ItemID.AdamantiteMask, ItemID.AdamantiteBreastplate, ItemID.AdamantiteLeggings, ItemID.TitaniumHeadgear, ItemID.TitaniumMask, ItemID.TitaniumHelmet, ItemID.TitaniumBreastplate, ItemID.TitaniumLeggings, ItemID.CrystalNinjaHelmet, ItemID.CrystalNinjaChestplate, ItemID.CrystalNinjaLeggings, ItemID.FrostHelmet, ItemID.FrostBreastplate, ItemID.FrostLeggings, ItemID.AncientBattleArmorHat, ItemID.AncientBattleArmorShirt, ItemID.AncientBattleArmorPants, ItemID.SquireGreatHelm, ItemID.SquirePlating, ItemID.SquireGreaves, ItemID.MonkBrows, ItemID.MonkShirt, ItemID.MonkPants, ItemID.HuntressWig, ItemID.HuntressJerkin, ItemID.HuntressPants, ItemID.ApprenticeHat, ItemID.ApprenticeRobe, ItemID.ApprenticeTrousers, ItemID.HallowedMask, ItemID.HallowedHelmet, ItemID.HallowedHood, ItemID.HallowedHood, ItemID.HallowedPlateMail, ItemID.HallowedGreaves, ItemID.AncientHallowedMask, ItemID.AncientHallowedHelmet, ItemID.AncientHallowedHeadgear, ItemID.AncientHallowedHood, ItemID.AncientHallowedPlateMail, ItemID.AncientHallowedGreaves] },
            { ProgressionState.PostMechanicalTrio, [ItemID.ChlorophyteMask, ItemID.ChlorophyteHelmet, ItemID.ChlorophyteHeadgear, ItemID.ChlorophytePlateMail, ItemID.ChlorophyteGreaves, ItemID.TurtleHelmet, ItemID.TurtleScaleMail, ItemID.TurtleLeggings] },
            { ProgressionState.PostPlantera, [ItemID.TikiMask, ItemID.TikiShirt, ItemID.TikiPants, ItemID.ShroomiteHeadgear, ItemID.ShroomiteHelmet, ItemID.ShroomiteMask, ItemID.ShroomiteBreastplate, ItemID.ShroomiteLeggings, ItemID.SpectreMask, ItemID.SpectreHood, ItemID.SpectreRobe, ItemID.SpectrePants, ItemID.SpookyHelmet, ItemID.SpookyBreastplate, ItemID.SpookyLeggings] },
            { ProgressionState.PostGolem, [ItemID.BeetleHelmet, ItemID.BeetleScaleMail, ItemID.BeetleShell, ItemID.BeetleLeggings, ItemID.SquireAltHead, ItemID.SquireAltShirt, ItemID.SquireAltPants, ItemID.MonkAltHead, ItemID.MonkAltShirt, ItemID.MonkAltPants, ItemID.HuntressAltHead, ItemID.HuntressAltShirt, ItemID.HuntressAltPants, ItemID.ApprenticeAltHead, ItemID.ApprenticeAltShirt, ItemID.ApprenticeAltPants] },
            { ProgressionState.PostLunaticCultist, [] },
            { ProgressionState.PostMoonLord, [ItemID.SolarFlareHelmet, ItemID.SolarFlareBreastplate, ItemID.SolarFlareLeggings, ItemID.VortexHelmet, ItemID.VortexBreastplate, ItemID.VortexLeggings, ItemID.NebulaHelmet, ItemID.NebulaBreastplate, ItemID.NebulaLeggings, ItemID.StardustHelmet, ItemID.StardustBreastplate, ItemID.StardustLeggings] },
        };

        /// <summary>
        /// Defines progression accessories
        /// </summary>
        private static readonly Dictionary<ProgressionState, int[]> ProgressionAccessories = new()
        {
            { ProgressionState.Hardmode, [ItemID.NeptunesShell, ItemID.MoonCharm, ItemID.MoonShell, ItemID.WarriorEmblem, ItemID.RangerEmblem, ItemID.SorcererEmblem, ItemID.SummonerEmblem, ItemID.AvengerEmblem, ItemID.CelestialEmblem, ItemID.MoonStone, ItemID.TitanGlove, ItemID.PowerGlove, ItemID.BerserkerGlove, ItemID.MechanicalGlove, ItemID.FireGauntlet, ItemID.PhilosophersStone, ItemID.CharmofMyths, ItemID.PutridScent, ItemID.MagicQuiver, ItemID.StalkersQuiver, ItemID.MoltenQuiver, ItemID.ArcaneFlower, ItemID.StarCloak, ItemID.BeeCloak, ItemID.ShimmerCloak, ItemID.ManaCloak, ItemID.ArmorPolish, ItemID.Blindfold, ItemID.FastClock, ItemID.PocketMirror, ItemID.TrifoldMap, ItemID.ArmorBracing, ItemID.ReflectiveShades, ItemID.ThePlan, ItemID.Vitamins, ItemID.AnkhCharm, ItemID.AnkhShield, ItemID.CrossNecklace, ItemID.StarVeil, ItemID.FleshKnuckles, ItemID.FrozenTurtleShell, ItemID.DiscountCard, ItemID.LuckyCoin, ItemID.GoldRing, ItemID.CoinRing, ItemID.GreedyRing, ItemID.AngelWings, ItemID.DemonWings, ItemID.FairyWings, ItemID.FinWings, ItemID.FrozenWings, ItemID.HarpyWings, ItemID.Jetpack, ItemID.BatWings, ItemID.BeeWings, ItemID.ButterflyWings, ItemID.FlameWings, ItemID.SteampunkWings, ItemID.FishronWings, ItemID.RedsWings, ItemID.DTownsWings, ItemID.WillsWings, ItemID.CrownosWings, ItemID.CenxsWings, ItemID.BejeweledValkyrieWing, ItemID.Yoraiz0rWings, ItemID.JimsWings, ItemID.SkiphsWings, ItemID.LokisWings, ItemID.ArkhalisWings, ItemID.LeinforsWings, ItemID.GhostarsWings, ItemID.SafemanWings, ItemID.FoodBarbarianWings, ItemID.GroxTheGreatWings] },
            { ProgressionState.PostMechanicalTrio, [] },
            { ProgressionState.PostPlantera, [ItemID.BlackBelt, ItemID.Tabi, ItemID.MasterNinjaGear, ItemID.RifleScope, ItemID.HerculesBeetle, ItemID.PapyrusScarab, ItemID.NecromanticScroll, ItemID.PaladinsShield, ItemID.HeroShield, ItemID.FrozenShield, ItemID.LeafWings, ItemID.Hoverboard, ItemID.BoneWings, ItemID.MothronWings, ItemID.GhostWings, ItemID.FestiveWings, ItemID.SpookyWings, ItemID.TatteredFairyWings, ItemID.RainbowWings] },
            { ProgressionState.PostGolem, [ItemID.DestroyerEmblem, ItemID.SunStone, ItemID.CelestialStone, ItemID.CelestialShell, ItemID.EyeoftheGolem, ItemID.SniperScope, ItemID.ReconScope, ItemID.BeetleWings, ItemID.BetsyWings] },
            { ProgressionState.PostLunaticCultist, [] },
            { ProgressionState.PostMoonLord, [ItemID.WingsNebula, ItemID.WingsVortex, ItemID.WingsSolar, ItemID.WingsStardust, ItemID.LongRainbowTrailWings] },
        };

        /// <summary>
        /// Defines progression mounts
        /// </summary>
        private static readonly Dictionary<ProgressionState, int[]> ProgressionMounts = new()
        {
            { ProgressionState.Hardmode, [ItemID.AncientHorn, ItemID.WolfMountItem, ItemID.BlessedApple, ItemID.ScalyTruffle, ItemID.QueenSlimeMountSaddle, ItemID.WallOfFleshGoatMountItem, ItemID.PirateShipMountItem, ItemID.SpookyWoodMountItem, ItemID.SantankMountItem] },
            { ProgressionState.PostMechanicalTrio, [] },
            { ProgressionState.PostPlantera, [ItemID.ReindeerBells] },
            { ProgressionState.PostGolem, [ItemID.BrainScrambler, ItemID.CosmicCarKey] },
            { ProgressionState.PostLunaticCultist, [] },
            { ProgressionState.PostMoonLord, [ItemID.DrillContainmentUnit] },
        };

        /// <summary>
        /// Defines progression hooks
        /// </summary>
        private static readonly Dictionary<ProgressionState, int[]> ProgressionHooks = new()
        {
            { ProgressionState.Hardmode, [ItemID.DualHook, ItemID.TendonHook, ItemID.IlluminantHook, ItemID.WormHook, ItemID.StaticHook, ItemID.QueenSlimeHook] },
            { ProgressionState.PostMechanicalTrio, [] },
            { ProgressionState.PostPlantera, [ItemID.SpookyHook, ItemID.ChristmasHook, ItemID.ThornHook] },
            { ProgressionState.PostGolem, [ItemID.AntiGravityHook] },
            { ProgressionState.PostLunaticCultist, [ItemID.LunarHook] },
            { ProgressionState.PostMoonLord, [] },
        };

        /// <summary>
        /// Defines progression buffs
        /// </summary>
        private static readonly Dictionary<ProgressionState, int[]> ProgressionBuffs = new()
        {
            { ProgressionState.Hardmode, [BuffID.BasiliskMount, BuffID.WolfMount, BuffID.UnicornMount, BuffID.PigronMount, BuffID.QueenSlimeMount, BuffID.WallOfFleshGoatMount, BuffID.PirateShipMount, BuffID.SpookyWoodMount, BuffID.SantankMount] },
            { ProgressionState.PostMechanicalTrio, [] },
            { ProgressionState.PostPlantera, [BuffID.Rudolph] },
            { ProgressionState.PostGolem, [BuffID.ScutlixMount] },
            { ProgressionState.PostLunaticCultist, [BuffID.UFOMount] },
            { ProgressionState.PostMoonLord, [BuffID.DrillMount] },
        };

        /// <summary>
        /// Pre-Hardmode items in Remix worlds, that are normally Hardmode items
        /// </summary>
        private static readonly int[] remixPreHardmode = [ItemID.MagicDagger, ItemID.Keybrand, ItemID.KOCannon, ItemID.IceBow, ItemID.BubbleGun, ItemID.UnholyTrident];

        /// <summary>
        /// Hardmode items in Remix worlds, that are normally Pre-Hardmode items
        /// </summary>
        private static readonly int[] remixHardmode = [ItemID.WandofSparking, ItemID.WandofFrosting, ItemID.Katana, ItemID.ChainKnife, ItemID.SnowballCannon, ItemID.AquaScepter, ItemID.FlowerofFire];


        /// <summary>
        /// Enable the blocking of items based on the current world progression
        /// </summary>
        /// <param name="restriction">Determines when items should be blocked</param>
        public static void Enable(ProgressionRestriction restriction = ProgressionRestriction.All)
        {
            _enabled = true;
            _restriction = restriction;
        }

        /// <summary>
        /// Checks if progression item blocking is enabled
        /// </summary>
        /// <param name="player">Player to base the check on</param>
        /// <returns>True if progression item blocking is enabled</returns>
        public static bool IsEnabled(Player player)
        {
            if (!_enabled)
                return false;

            switch (_restriction)
            {
                case ProgressionRestriction.Player:
                    if (player.difficulty != PlayerDifficultyID.MediumCore && player.difficulty != PlayerDifficultyID.Hardcore)
                        return false;
                    break;

                case ProgressionRestriction.World:
                    if (!Main.expertMode && !Main.specialSeedWorld)
                        return false;
                    break;
            }

            return true;
        }

        /// <summary>
        /// Checks if an item is allowed based on the current progression
        /// </summary>
        /// <param name="type">Progression item type</param>
        /// <param name="item">Progression item ID</param>
        /// <param name="playe">Player to base the check on</param>
        /// <returns>True if the item is allowed</returns>
        public static bool IsElementAllowed(ProgressionElement type, int item, Player player)
        {
            if (!IsEnabled(player))
                return true;

            List<Dictionary<ProgressionState, int[]>> allItems = [];
            switch (type)
            {
                case ProgressionElement.Usable:
                    allItems.Add(ProgressionConsumable);
                    allItems.Add(ProgressionWeapons);
                    allItems.Add(ProgressionTools);
                    allItems.Add(ProgressionHooks);
                    allItems.Add(ProgressionMounts);
                    break;

                case ProgressionElement.Equippable:
                    allItems.Add(ProgressionArmor);
                    allItems.Add(ProgressionAccessories);
                    allItems.Add(ProgressionHooks);
                    allItems.Add(ProgressionMounts);
                    break;

                case ProgressionElement.Ammo:
                    allItems.Add(ProgressionAmmo);
                    break;

                case ProgressionElement.Buff:
                    allItems.Add(ProgressionBuffs);
                    break;

                default:
                    return false;
            }

            var start_state = GetProgressionState();
            foreach (var items in allItems)
            {
                for (ProgressionState state = start_state + 1; state <= ProgressionState.PostMoonLord; state++)
                {
                    if (Main.remixWorld && state > ProgressionState.PreHardmode && remixHardmode.Contains(item))
                        return false;

                    if (items[state].Contains(item))
                    {
                        if (Main.remixWorld && remixPreHardmode.Contains(item))
                            return true;

                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the current progression state
        /// </summary>
        /// <returns>Current progression state</returns>
        private static ProgressionState GetProgressionState()
        {
            if (NPC.downedMoonlord)
                return ProgressionState.PostMoonLord;

            if (NPC.downedAncientCultist)
                return ProgressionState.PostLunaticCultist;

            if (NPC.downedGolemBoss)
                return ProgressionState.PostGolem;

            if (NPC.downedPlantBoss)
                return ProgressionState.PostPlantera;

            if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
                return ProgressionState.PostMechanicalTrio;

            if (Main.hardMode)
                return ProgressionState.Hardmode;

            return ProgressionState.PreHardmode;
        }
    }
}
