using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
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
            if (source is EntitySource_TileBreak)
            {
                // Check that the local player is the one closest to the destroyed tile
                if (Player.FindClosest(item.position, 1, 1) != Main.myPlayer)
                    return;

                AchHelper.NotifyTileDrop(Main.LocalPlayer, item.type);
            }

            else if (source is EntitySource_Loot loot)
            {
                // Check that the local player has damaged the NPC
                if (loot.Entity is NPC npc && npc.playerInteraction[Main.myPlayer])
                    AchHelper.NotifyNpcDrop(Main.LocalPlayer, npc, item.type);
            }

            else if (source is EntitySource_Gift gift)
            {
                // Check that the local player is the one closest to the gift
                if (Player.FindClosest(item.Center, 1, 1) != Main.myPlayer)
                    return;

                if (gift.Entity is NPC npc)
                    AchHelper.NotifyItemGift(Main.LocalPlayer, npc.type, item.type);
                else
                    AchHelper.NotifyItemGift(Main.LocalPlayer, NPCID.None, item.type);
            }   
        }
    }
}
