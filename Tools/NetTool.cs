using Terraria;
using Terraria.ID;

namespace TerrariaAchievementLib.Tools
{
    /// <summary>
    /// Tool to help with network checks
    /// </summary>
    public class NetTool
    {
        /// <summary>
        /// Checks if the game is singleplayer
        /// </summary>
        /// <returns>True if the game is singleplayer</returns>
        public static bool Singleplayer() => Main.netMode == NetmodeID.SinglePlayer;

        /// <summary>
        /// Checks if the game is a multiplayer client
        /// </summary>
        /// <returns>True if the game is a multiplayer client</returns>
        public static bool MultiplayerClient() => Main.netMode == NetmodeID.MultiplayerClient;

        /// <summary>
        /// Checks if the game is a multiplayer server
        /// </summary>
        /// <returns>True if the game is a multiplayer server</returns>
        public static bool MultiplayerServer() => Main.dedServ;
    }
}
