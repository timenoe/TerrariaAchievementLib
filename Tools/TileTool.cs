using Microsoft.Xna.Framework;
using Terraria;

namespace TerrariaAchievementLib.Tools
{
    /// <summary>
    /// Tool to help with tile checks
    /// </summary>
    public class TileTool
    {
        /// <summary>
        /// Checks if a tile is on screen
        /// </summary>
        /// <param name="tile">Tile coordinates</param>
        /// <returns>True if the tile is on screen</returns>
        public static bool IsTileOnScreen(Point tile)
        {
            bool inX = tile.X > Main.screenPosition.X / 16 && tile.X < Main.screenPosition.X / 16 + Main.screenWidth / 16;
            bool inY = tile.Y > Main.screenPosition.Y / 16 && tile.Y < Main.screenPosition.Y / 16 + Main.screenHeight / 16;
            return inX && inY;
        }
    }
}
