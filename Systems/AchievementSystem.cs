using System.Reflection;
using Terraria;
using Terraria.ModLoader;
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
        /// Reference to the instance of this class
        /// </summary>
        private static AchievementSystem _instance;

        /// <summary>
        /// File to cache general information
        /// </summary>
        private static string _cacheFilePath;


        /// <summary>
        /// Reference to the instance of this class
        /// </summary>
        public static AchievementSystem Instance => _instance;

        /// <summary>
        /// File to cache general information
        /// </summary>
        public static string CacheFilePath => _cacheFilePath;


        public override void OnModLoad()
        {
            if (Main.dedServ)
                return;

            _instance = this;
            _cacheFilePath = $"{ModLoader.ModPath}/{Mod.Name}Lib.nbt";

            LogTool.SetDefaults(Mod);
        }
    }
}
