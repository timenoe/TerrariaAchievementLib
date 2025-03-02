using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using Terraria;
using Terraria.Achievements;
using Terraria.ModLoader;
using Terraria.Utilities;
using TerrariaAchievementLib.Achievements;
using TerrariaAchievementLib.Players;
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
        /// File to backup achievement information
        /// </summary>
        private static string _backupFilePath;

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

        /// <summary>
        /// Unique achievement name header
        /// </summary>
        protected abstract string Identifier { get; }

        /// <summary>
        /// Achievement icon texture path
        /// </summary>
        protected abstract List<string> TexturePaths { get; }


        public override void OnModLoad()
        {
            if (Main.dedServ)
                return;

            _backupFilePath = $"{ModLoader.ModPath}/{Mod.Name}Lib.dat";
            _cacheFilePath = $"{ModLoader.ModPath}/{Mod.Name}Lib.nbt";
            _instance = this;

            LogTool.SetDefaults(Mod);
            ModContent.GetInstance<IconSystem>().LoadAchTextures(TexturePaths);

            RegisterAchievements();
            LoadMainSaveData();
            LoadCustomSaveData();

            On_AchievementManager.Save += On_AchievementManager_Save;
        }

        public override void OnModUnload()
        {
            if (Main.dedServ)
                return;

            On_AchievementManager.Save -= On_AchievementManager_Save;

            BackupCustomSaveData();
            UnregisterAchievements();
        }

        /// <summary>
        /// Checks if an achievement was added from this system
        /// </summary>
        /// <param name="internalName">Internal achievement name</param>
        /// <returns>True if the achievement was added from this system</returns>
        public bool IsCustomAchievement(string internalName) => internalName.StartsWith(Identifier);

        /// <summary>
        /// Unlock a manual achievement using its internal name
        /// </summary>
        /// <param name="internalName">Achievement name</param>
        public void UnlockManualAchievement(string internalName)
        {
            if (!IsCustomAchievement(internalName))
                return;
            
            AchievementTool.UnlockAchievementInternal($"{Identifier}_{internalName}", out _);
        }

        /// <summary>
        /// Register new achievements to the in-game list<br/>
        /// Involves consecutive calls to RegisterNewAchievement
        /// </summary>
        protected abstract void RegisterAchievements();

        /// <summary>
        /// Register a new achievement with one condition to the in-game list
        /// </summary>
        /// <param name="name">Achievement name</param>
        /// <param name="cond">Achievement condition</param>
        /// <param name="cat">Achievement category</param>
        protected void RegisterAchievement(string name, CustomAchievementCondition cond, AchievementCategory cat)
        {
            // Add unique achievement header to the name if needed
            if (!name.StartsWith(Identifier))
                name = $"{Identifier}_{name}";

            // Enable Hardestcore if needed
            if (cond.Reqs.PlayerDiff == PlayerDiff.Hardestcore)
                HardestcorePlayer.Enable();

            Achievement ach = new(name);
            ach.AddCondition(cond);

            Main.Achievements.Register(ach);
            Main.Achievements.RegisterAchievementCategory(name, cat);
            ModContent.GetInstance<IconSystem>().RegisterAchievementIcon(name);
        }

        /// <summary>
        /// Register a new achievement with multiple conditions to the in-game list
        /// </summary>
        /// <param name="name">Achievement name</param>
        /// <param name="conds">Achievement conditions</param>
        /// <param name="track">True if tracking completed conditions total in the in-game menu</param>
        /// <param name="cat">Achievement category</param>
        protected void RegisterAchievement(string name, List<CustomAchievementCondition> conds, bool track, AchievementCategory cat)
        {
            // Add unique achievement header to the name if needed
            if (!name.StartsWith(Identifier))
                name = $"{Identifier}_{name}";

            Achievement ach = new(name);
            List<string> condNames = [];
            foreach (var cond in conds)
            {
                if (condNames.Contains(cond.Name))
                    LogTool.ModLog($"Blocked a duplicate condition from being added: {name} - {cond.Name}", ModLogType.Warn);
                else
                {
                    // Enable Hardestcore if needed
                    if (cond.Reqs.PlayerDiff == PlayerDiff.Hardestcore)
                        HardestcorePlayer.Enable();

                    ach.AddCondition(cond);
                    condNames.Add(cond.Name);
                }
            }

            if (track)
                ach.UseConditionsCompletedTracker();

            Main.Achievements.Register(ach);
            Main.Achievements.RegisterAchievementCategory(name, cat);
            ModContent.GetInstance<IconSystem>().RegisterAchievementIcon(name);
        }

        /// <summary>
        /// Register a new achievement to manually unlock to the in-game list
        /// </summary>
        /// <param name="name">Achievement name</param>
        /// <param name="cat">Achievement category</param>
        protected void RegisterManualAchievement(string name, AchievementCategory cat)
        {
            // Add unique achievement header to the name if needed
            if (!name.StartsWith(Identifier))
                name = $"{Identifier}_{name}";

            Achievement ach = new(name);
            ManualAchievementCondition cond = new("UNLOCKED");
            ach.AddCondition(cond);

            Main.Achievements.Register(ach);
            Main.Achievements.RegisterAchievementCategory(name, cat);
            ModContent.GetInstance<IconSystem>().RegisterAchievementIcon(name);
        }

        /// <summary>
        /// Saves a backup of achievements.dat with custom achievements
        /// </summary>
        private static void BackupCustomSaveData() => File.Copy($"{Main.SavePath}/achievements.dat", _backupFilePath, overwrite: true);

        /// <summary>
        /// Load any save data from achievements.dat if applicable
        /// </summary>
        private static void LoadMainSaveData()
        {
            Dictionary<string, Achievement> achs = (Dictionary<string, Achievement>)typeof(AchievementManager).GetField("_achievements", ReflectionFlags)?.GetValue(Main.Achievements);
            if (achs == null)
                return;

            // Clear existing achievement progress before loading again
            // Bug in the vanilla code causes issues during two consecutive loads
            foreach (Achievement ach in achs.Values)
                ach.ClearProgress();

            Main.Achievements.Load();
        }

        /// <summary>
        /// Load custom achievement save data from a backed up achievements.dat<br/>
        /// The main achievements.dat is not guaranteed to have save data for this system<br/>
        /// It could have been overwritten without this system's achievements
        /// </summary>
        private void LoadCustomSaveData()
        {
            if (!FileUtilities.Exists(_backupFilePath, false))
                return;

            byte[] buffer = FileUtilities.ReadAllBytes(_backupFilePath, false);
            Dictionary<string, StoredAchievement> achs = null;
            try
            {
                using MemoryStream memStream = new(buffer);

                byte[] cryptoKey = Encoding.ASCII.GetBytes("RELOGIC-TERRARIA");
                using CryptoStream cryptoStream = new(memStream, Aes.Create().CreateDecryptor(cryptoKey, cryptoKey), CryptoStreamMode.Read);

                using BsonReader bsonReader = new(cryptoStream);
                achs = JsonSerializer.Create(new JsonSerializerSettings()).Deserialize<Dictionary<string, StoredAchievement>>(bsonReader);
            }
            catch (Exception)
            {
                return;
            }

            if (achs == null)
                return;

            foreach (KeyValuePair<string, StoredAchievement> ach in achs)
            {
                // Only load custom achievements
                if (!IsCustomAchievement(ach.Key))
                    continue;

                Achievement mainAch = Main.Achievements.GetAchievement(ach.Key);
                if (mainAch == null)
                    continue;

                mainAch.ClearProgress();
                mainAch.Load(ach.Value.Conditions);
            }

            // Must save here or else other mods may clear the progress that was just loaded
            Main.Achievements.Save();
        }

        /// <summary>
        /// Unregister all achievements that were added from this system
        /// </summary>
        private void UnregisterAchievements()
        {
            Dictionary<string, Achievement> achs = (Dictionary<string, Achievement>)typeof(AchievementManager).GetField("_achievements", ReflectionFlags)?.GetValue(Main.Achievements);
            Dictionary<string, int> icons = (Dictionary<string, int>)typeof(AchievementManager).GetField("_achievementIconIndexes", ReflectionFlags)?.GetValue(Main.Achievements);

            if (achs == null || icons == null)
                return;

            Dictionary<string, Achievement> achs_copy = new(achs);
            foreach (KeyValuePair<string, Achievement> ach in achs_copy)
            {
                // Only remove custom achievements
                if (!IsCustomAchievement(ach.Value.Name))
                    continue;

                achs.Remove(ach.Key);
                icons.Remove(ach.Key);

                Delegate handler = typeof(AchievementManager).GetMethod("AchievementCompleted", ReflectionFlags)?.CreateDelegate(typeof(Achievement.AchievementCompleted), Main.Achievements);
                if (handler == null)
                    continue;

                // Prevents double achievement unlocks during mod reload
                ach.Value.OnCompleted -= (Achievement.AchievementCompleted)handler;
            }
        }

        /// <summary>
        /// Detour to make a copy of achievements.dat every time it's saved
        /// </summary>
        /// <param name="orig">Original Save method</param>
        /// <param name="self">Achievement manager that is saving</param>
        private void On_AchievementManager_Save(On_AchievementManager.orig_Save orig, AchievementManager self)
        {
            orig.Invoke(self);

            BackupCustomSaveData();
        }


        /// <summary>
        /// Format of a stored achievement in achievements.dat
        /// </summary>
        private class StoredAchievement
        {
            [JsonProperty]
            public Dictionary<string, JObject> Conditions;
        }
    }
}
