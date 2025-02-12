using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;
using System.Collections.Generic;

namespace TerrariaAchievementLib.Systems
{
    /// <summary>
    /// Updates Hardestcore banned player cache<br/>
    /// When a banned player is renamed, update their cache entry<br/>
    /// When a banned player is created again, remove them from the cache<br/>
    /// Player name is used because players don't have a unique ID like worlds do
    /// </summary>
    public class HardestcoreSystem : ModSystem
    {
        public override void Load()
        {
            On_PlayerFileData.Rename += On_PlayerFileData_Rename;
            On_PlayerFileData.CreateAndSave += On_PlayerFileData_CreateAndSave;
        }

        public override void Unload()
        {
            On_PlayerFileData.Rename -= On_PlayerFileData_Rename;
            On_PlayerFileData.CreateAndSave -= On_PlayerFileData_CreateAndSave;
        }

        private void On_PlayerFileData_Rename(On_PlayerFileData.orig_Rename orig, PlayerFileData self, string newName)
        {
            string oldName = self.Name;
            orig.Invoke(self, newName);

            if (!FileUtilities.Exists(AchievementSystem.CacheFilePath, false))
                return;

            TagCompound tag = TagIO.FromFile(AchievementSystem.CacheFilePath);
            if (tag == null)
                return;

            if (!tag.ContainsKey("BannedHardestcorePlayers"))
                return;

            // Replace old player name with new one
            List<string> bannedPlayers = tag.Get<List<string>>("BannedHardestcorePlayers");
            bannedPlayers.Remove(oldName);
            bannedPlayers.Add(newName);
            tag.Set("BannedHardestcorePlayers", bannedPlayers, true);
            TagIO.ToFile(tag, AchievementSystem.CacheFilePath);
        }

        private PlayerFileData On_PlayerFileData_CreateAndSave(On_PlayerFileData.orig_CreateAndSave orig, Terraria.Player player)
        {
            if (!FileUtilities.Exists(AchievementSystem.CacheFilePath, false))
                return orig.Invoke(player);

            TagCompound tag = TagIO.FromFile(AchievementSystem.CacheFilePath);
            if (tag == null)
                return orig.Invoke(player);

            if (!tag.ContainsKey("BannedHardestcorePlayers"))
                return orig.Invoke(player);

            // Remove new player from banned list if applicable
            List<string> bannedPlayers = tag.Get<List<string>>("BannedHardestcorePlayers");
            bannedPlayers.Remove(player.name);
            tag.Set("BannedHardestcorePlayers", bannedPlayers, true);
            TagIO.ToFile(tag, AchievementSystem.CacheFilePath);
            
            return orig.Invoke(player);
        }
    }
}
