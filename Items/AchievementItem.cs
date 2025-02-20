using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaAchievementLib.Achievements;
using TerrariaAchievementLib.Tools;

namespace TerrariaAchievementLib.Items
{
    /// <summary>
    /// Sends notifications to achievement conditions when an item is dropped
    /// </summary>
    public class AchievementItem : GlobalItem
    {
        // This allows for Magic Storage crafts to be recognized
        public override void OnCreated(Item item, ItemCreationContext context)
        {
            if (context is RecipeItemCreationContext recipeContext)
                AchievementsHelper.NotifyItemCraft(recipeContext.Recipe);
        }

        public override void OnSpawn(Item item, IEntitySource source)
        {
            if (source is EntitySource_TileBreak tile)
            {
                // Check that the local player is the one closest to the destroyed tile
                if (Player.FindClosest(item.Center, 1, 1) != Main.myPlayer)
                    return;

                // Check that the local player is within range of the tile
                if (TileTool.IsTileOnScreen(tile.TileCoords))
                    CustomAchievementsHelper.NotifyTileDrop(Main.LocalPlayer, item.type);
            }

            else if (source is EntitySource_ShakeTree)
            {
                // Check that the local player is the one closest to the item
                if (Player.FindClosest(item.Center, 1, 1) == Main.myPlayer)
                    CustomAchievementsHelper.NotifyItemShake(Main.LocalPlayer, item.type);
            }

            else if (source is EntitySource_Loot loot)
            {
                // Check that the local player has damaged the NPC
                if (loot.Entity is NPC npc && npc.playerInteraction[Main.myPlayer])
                    CustomAchievementsHelper.NotifyNpcDrop(Main.LocalPlayer, npc.type, item.type);
            }

            else if (source is EntitySource_ItemOpen bag)
            {
                // Check that the local player is the one closest to the item
                if (Player.FindClosest(item.Center, 1, 1) == Main.myPlayer)
                    CustomAchievementsHelper.NotifyItemOpen(Main.LocalPlayer, bag.ItemType, item.type);
            }
            
            else if (source is EntitySource_Gift gift)
            {
                // Check that the local player is the one closest to the gift
                if (Player.FindClosest(item.Center, 1, 1) != Main.myPlayer)
                    return;

                if (gift.Entity is NPC npc)
                    CustomAchievementsHelper.NotifyNpcGift(Main.LocalPlayer, npc.type, item.type);
                else
                    CustomAchievementsHelper.NotifyNpcGift(Main.LocalPlayer, NPCID.None, item.type);
            }
        }
    }
}
