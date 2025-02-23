using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;
using TerrariaAchievementLib.Buffs;
using TerrariaAchievementLib.Systems;
using TerrariaAchievementLib.Tools;

namespace TerrariaAchievementLib.Players
{
    /// <summary>
    /// Player that complies with Hardestcore rules
    /// </summary>
    public class HardestcorePlayer : ModPlayer
    {
        /// <summary>
        /// True if Hardestcore is enabled
        /// </summary>
        private static bool _enabled;

        /// <summary>
        /// String GUID of the visited world
        /// </summary>
        private string _recentWorldId = "";

        /// <summary>
        /// String GUID of the player
        /// </summary>
        private string _uniqueId = "";


        public override void OnEnterWorld()
        {
            if (!_enabled)
                return;

            if (Player.difficulty == PlayerDifficultyID.Hardcore)
            {
                string world_id = Main.ActiveWorldFileData.UniqueId.ToString();

                if (!WasPlayerGeneratedWithMod())
                {
                    LogTool.ChatLog("Hardestcore is disabled: This Hardcore player was not created with this mod.", ChatLogType.Warn);
                    return;
                }

                if (!WasWorldGeneratedWithMod())
                {
                    LogTool.ChatLog("Hardestcore is disabled: This world was not created with this mod.", ChatLogType.Warn);
                    return;
                }

                // Ban player if they enter more than 1 world 
                if (!string.IsNullOrEmpty(_recentWorldId) && world_id != _recentWorldId)
                    BanPlayer();

                if (!IsPlayerHardestcore())
                {
                    LogTool.ChatLog("Hardestcore is disabled: Either this Hardcore player has visited other worlds, or they have died before.", ChatLogType.Warn);
                    return;
                }

                if (!IsWorldHardestcore())
                {
                    LogTool.ChatLog("Hardestcore is disabled: Either a non-Hardcore player has visited this world, or a Hardcore player has died in this world.", ChatLogType.Warn);
                    return;
                }

                _recentWorldId = world_id;
                Player.AddBuff(ModContent.BuffType<HardestcoreBuff>(), 1);
            }

            // Ban world if a non-hardcore player enters it
            else
                BanWorld();
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (!_enabled || Player.difficulty != PlayerDifficultyID.Hardcore)
                return;

            Player.ClearBuff(ModContent.BuffType<HardestcoreBuff>());

            BanPlayer();
            BanWorld();
        }

        public override void SaveData(TagCompound tag) => tag["HardestcoreUniqueId"] = _uniqueId;

        public override void LoadData(TagCompound tag) => _uniqueId = tag.GetString("HardestcoreUniqueId");

        /// <summary>
        /// Enables this player to process Hardestcore events
        /// </summary>
        public static void Enable() => _enabled = true;

        /// <summary>
        /// Checks if a Hardcore player has died in the current world before
        /// </summary>
        /// <returns>True if a Hardcore player has died in the current world before</returns>
        private static bool IsWorldHardestcore()
        {
            if (!FileUtilities.Exists(AchievementSystem.CacheFilePath, false))
                return true;

            TagCompound tag = TagIO.FromFile(AchievementSystem.CacheFilePath);
            if (tag == null)
                return true;

            if (!tag.ContainsKey("BannedHardestcoreWorlds"))
                return true;

            List<string> bannedWorlds = tag.Get<List<string>>("BannedHardestcoreWorlds");
            if (bannedWorlds.Contains(Main.ActiveWorldFileData.UniqueId.ToString()))
                return false;

            return true;
        }

        /// <summary>
        /// Check if the world was generated with this mod enabled
        /// </summary>
        /// <returns>True if the world was generated with this mod enabled</returns>
        public static bool WasWorldGeneratedWithMod()
        {
            WorldFileData world = Main.ActiveWorldFileData;

            if (!world.WorldGenModsRecorded)
                return false;

            if (!world.TryGetModVersionGeneratedWith("PlayerAchievements", out _))
                return false;

            return true;
        }

        /// <summary>
        /// Ban the current world from Hardestcore
        /// </summary>
        private static void BanWorld()
        {
            TagCompound tag = [];
            if (FileUtilities.Exists(AchievementSystem.CacheFilePath, false))
            {
                TagCompound fileTag = TagIO.FromFile(AchievementSystem.CacheFilePath);
                if (fileTag != null)
                    tag = fileTag;
            }

            // Add this world to the list of banned worlds
            string world_id = Main.ActiveWorldFileData.UniqueId.ToString();
            List<string> bannedWorlds = [];
            if (tag.ContainsKey("BannedHardestcoreWorlds"))
                bannedWorlds.AddRange(tag.Get<List<string>>("BannedHardestcoreWorlds"));
            if (!bannedWorlds.Contains(world_id))
                bannedWorlds.Add(world_id);
            tag.Set("BannedHardestcoreWorlds", bannedWorlds, true);

            TagIO.ToFile(tag, AchievementSystem.CacheFilePath);
        }

        /// <summary>
        /// Assign a unique GUID during creation
        /// </summary>
        public void Create() => _uniqueId = Guid.NewGuid().ToString();

        /// <summary>
        /// Check if the player can earn a Hardestcore achievement
        /// </summary>
        /// <returns>True if the player can earn a Hardestcore achievement</returns>
        public bool CanEarnAchievement()
        {
            if (Player == null)
                return false;

            return Player.HasBuff(ModContent.BuffType<HardestcoreBuff>());
        }

        /// <summary>
        /// Checks if a Hardcore player has died before
        /// </summary>
        /// <returns>True if the Hardcore player has died before</returns>
        private bool IsPlayerHardestcore()
        {
            if (!FileUtilities.Exists(AchievementSystem.CacheFilePath, false))
                return true;

            TagCompound tag = TagIO.FromFile(AchievementSystem.CacheFilePath);
            if (tag == null)
                return true;

            if (!tag.ContainsKey("BannedHardestcorePlayers"))
                return true;

            List<string> bannedPlayers = tag.Get<List<string>>("BannedHardestcorePlayers");
            if (bannedPlayers.Contains(_uniqueId))
                return false;

            return true;
        }

        /// <summary>
        /// Check if the player was generated with this mod enabled
        /// </summary>
        /// <returns>True if the player was generated with this mod enabled</returns>
        private bool WasPlayerGeneratedWithMod() => !string.IsNullOrEmpty(_uniqueId);

        /// <summary>
        /// Ban player from Hardestcore
        /// </summary>
        private void BanPlayer()
        {
            TagCompound tag = [];
            if (FileUtilities.Exists(AchievementSystem.CacheFilePath, false))
            {
                TagCompound fileTag = TagIO.FromFile(AchievementSystem.CacheFilePath);
                if (fileTag != null)
                    tag = fileTag;
            }

            // Add this player to the list of banned players
            List<string> bannedPlayers = [];
            if (tag.ContainsKey("BannedHardestcorePlayers"))
                bannedPlayers.AddRange(tag.Get<List<string>>("BannedHardestcorePlayers"));
            if (!bannedPlayers.Contains(_uniqueId))
                bannedPlayers.Add(_uniqueId);
            tag.Set("BannedHardestcorePlayers", bannedPlayers, true);

            TagIO.ToFile(tag, AchievementSystem.CacheFilePath);
        }
    }
}
