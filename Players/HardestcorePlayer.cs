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
        /// True if Hardestcore is enabled
        /// </summary>
        public static bool Enabled => _enabled;
        
        public override void OnEnterWorld()
        {
            if (!Enabled || Player.difficulty != PlayerDifficultyID.Hardcore)
                return;

            if (!IsPlayerHardestcore(Player))
            {
                Main.NewText("Hardestcore is disabled (this player has died before).");
                return;
            }

            if (!IsWorldHardestcore(Main.ActiveWorldFileData))
            {
                Main.NewText("Hardestcore is disabled (a player has died in this world before).");
                return;
            }

            Player.AddBuff(ModContent.BuffType<HardestcoreBuff>(), 1);
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (!Enabled || Player.difficulty != PlayerDifficultyID.Hardcore)
                return;

            Player.ClearBuff(ModContent.BuffType<HardestcoreBuff>());
            
            TagCompound tag = [];
            if (FileUtilities.Exists(AchievementSystem.CacheFilePath, false))
            {
                TagCompound fileTag = TagIO.FromFile(AchievementSystem.CacheFilePath);
                if (fileTag != null)
                    tag = fileTag;
            }

            // Add this player to the list of banned players
            string player_name = Player.name;
            List<string> bannedPlayers = [];
            if (tag.ContainsKey("BannedHardestcorePlayers"))
                bannedPlayers.AddRange(tag.Get<List<string>>("BannedHardestcorePlayers"));
            if (!bannedPlayers.Contains(player_name))
                bannedPlayers.Add(player_name);
            tag.Set("BannedHardestcorePlayers", bannedPlayers, true);

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
        /// Check if the player can earn a Hardestcore achievement
        /// </summary>
        /// <returns>True if the player can earn a Hardestcore achievement</returns>
        public bool CanEarnAchievement() => Player.HasBuff(ModContent.BuffType<HardestcoreBuff>());

        /// <summary>
        /// Enables this player to process Hardestcore events
        /// </summary>
        public static void Enable() => _enabled = true;

        /// <summary>
        /// Checks if a Hardcore player has died before
        /// </summary>
        /// <param name="player">Player</param>
        /// <returns>True if the Hardcore player has died before</returns>
        private static bool IsPlayerHardestcore(Player player)
        {
            if (!FileUtilities.Exists(AchievementSystem.CacheFilePath, false))
                return true;

            TagCompound tag = TagIO.FromFile(AchievementSystem.CacheFilePath);
            if (tag == null)
                return true;

            if (!tag.ContainsKey("BannedHardestcorePlayers"))
                return true;

            List<string> bannedPlayers = tag.Get<List<string>>("BannedHardestcorePlayers");
            if (bannedPlayers.Contains(player.name))
                return false;

            return true;
        }

        /// <summary>
        /// Checks if a Hardcore player has died in this world before
        /// </summary>
        /// <param name="worldData">World file data</param>
        /// <returns>True if a Hardcore player has died in this world before</returns>
        private static bool IsWorldHardestcore(WorldFileData worldData)
        {
            if (!FileUtilities.Exists(AchievementSystem.CacheFilePath, false))
                return true;

            TagCompound tag = TagIO.FromFile(AchievementSystem.CacheFilePath);
            if (tag == null)
                return true;

            if (!tag.ContainsKey("BannedHardestcoreWorlds"))
                return true;

            List<string> bannedWorlds = tag.Get<List<string>>("BannedHardestcoreWorlds");
            if (bannedWorlds.Contains(worldData.UniqueId.ToString()))
                return false;

            return true;
        }
    }
}
