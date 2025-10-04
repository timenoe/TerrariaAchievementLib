using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using TerrariaAchievementLib.Achievements;
using TerrariaAchievementLib.Tools;

namespace TerrariaAchievementLib.Systems
{
    /// <summary>
    /// Used to add new achievements to the in-game list
    /// </summary>
    public abstract class AchievementSystem : ModSystem
    {
        /// <summary>
        /// Flags to use during reflection
        /// </summary>
        public const BindingFlags ReflectionFlags = BindingFlags.NonPublic | BindingFlags.Instance;


        /// <summary>
        /// File to cache general information
        /// </summary>
        private static string _cacheFilePath;

        /// <summary>
        /// Reference to the instance of this class
        /// </summary>
        private static AchievementSystem _instance;


        /// <summary>
        /// File to cache general information
        /// </summary>
        public static string CacheFilePath => _cacheFilePath;

        /// <summary>
        /// Reference to the instance of this class
        /// </summary>
        public static AchievementSystem Instance => _instance;


        public override void OnModLoad()
        {
            if (Main.dedServ)
                return;

            _cacheFilePath = $"{ModLoader.ModPath}/{Mod.Name}Lib.nbt";
            _instance = this;

            LogTool.SetDefaults(Mod);
        }

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
    }
}
