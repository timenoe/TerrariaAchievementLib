using Microsoft.Xna.Framework;
using Terraria;

namespace TerrariaAchievementLib.Tools
{
    public class TileTool
    {
        public static bool IsTileOnScreen(Point tile)
        {
            bool inX = tile.X > Main.screenPosition.X / 16 && tile.X < Main.screenPosition.X / 16 + Main.screenWidth / 16;
            bool inY = tile.Y > Main.screenPosition.Y / 16 && tile.Y < Main.screenPosition.Y / 16 + Main.screenHeight / 16;
            return inX && inY;
        }
    }
}
