using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TerrariaAchievementLib.Achievements;

namespace TerrariaAchievementLib.Items
{
    /// <summary>
    /// Sends notifications to achievement conditions when an item is dropped
    /// </summary>
    public class AchItem : GlobalItem
    {
        public override void OnSpawn(Item item, IEntitySource source)
        {
            if (source is EntitySource_Loot loot)
            {
                // Check that the local player has damaged the NPC
                if (loot.Entity is NPC npc && npc.playerInteraction[Main.myPlayer])
                    AchHelper.NotifyNpcDrop(Main.LocalPlayer, npc, item.type);
            }

            else if (source is EntitySource_TileBreak tile)
            {
                // Check that the local player is the one closest to the destroyed tile
                // Pretty sure this is similar to what vanilla does in similar situations
                if (Player.FindClosest(tile.TileCoords.ToVector2(), 1, 1) == Main.myPlayer)
                    AchHelper.NotifyTileDrop(Main.LocalPlayer, item.type);
            }  
        }
    }
}
