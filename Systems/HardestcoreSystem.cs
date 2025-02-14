using Terraria;
using Terraria.IO;
using Terraria.ModLoader;
using TerrariaAchievementLib.Players;

namespace TerrariaAchievementLib.Systems
{
    /// <summary>
    /// Assigns a Hardestcore GUID to the player during creation
    /// </summary>
    public class HardestcoreSystem : ModSystem
    {
        public override void Load()
        {
            if (Main.dedServ)
                return;
            
            On_PlayerFileData.CreateAndSave += On_PlayerFileData_CreateAndSave;
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            On_PlayerFileData.CreateAndSave -= On_PlayerFileData_CreateAndSave;
        }

        private PlayerFileData On_PlayerFileData_CreateAndSave(On_PlayerFileData.orig_CreateAndSave orig, Player player)
        {
            player.GetModPlayer<HardestcorePlayer>()?.Create();

            return orig.Invoke(player);
        }
    }
}
