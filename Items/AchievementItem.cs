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
        public override void Load()
        {
            On_Item.ChangeItemType += On_Item_ChangeItemType;
            On_Item.SetDefaults_int += On_Item_SetDefaults_int;
        }

        public override void Unload()
        {
            On_Item.ChangeItemType -= On_Item_ChangeItemType;
            On_Item.SetDefaults_int -= On_Item_SetDefaults_int;
        }

        // This allows for Magic Storage crafts to be recognized
        public override void OnCreated(Item item, ItemCreationContext context)
        {
            if (context is RecipeItemCreationContext recipeContext)
            {
                AchievementsHelper.NotifyItemCraft(recipeContext.Recipe);
                AchievementsHelper.NotifyItemPickup(Main.LocalPlayer, item);
            }
        }

        public override void OnSpawn(Item item, IEntitySource source)
        {
            if (source is EntitySource_TileBreak tile)
            {
                // Check that the local player is the one closest to the destroyed tile
                if (Player.FindClosest(item.Center, 1, 1) != Main.myPlayer)
                    return;

                // Check that the local player is within range of the tile
                // TODO: Doesn't work very well according to user feedback
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

        /// <summary>
        /// Detour to notify achievement conditions when an item type is changed (right-clicked to activate, etc.)
        /// </summary>
        /// <param name="orig">Original ChangeItemType method</param>
        /// <param name="self">Item that is changing</param>
        /// <param name="to">Item to change to</param>
        private void On_Item_ChangeItemType(On_Item.orig_ChangeItemType orig, Item self, int to)
        {
            orig.Invoke(self, to);

            CustomAchievementsHelper.NotifyItemGrab(Main.LocalPlayer, (short)to, 1);
        }

        /// <summary>
        /// Detour to notify achievement conditions when an item has its defaults set (Wilson beard grows, etc.)
        /// </summary>
        /// <param name="orig">Original SetDefaults method</param>
        /// <param name="self">Item that had its defaults set</param>
        /// <param name="Type">Type to set the item to</param>
        private void On_Item_SetDefaults_int(On_Item.orig_SetDefaults_int orig, Item self, int Type)
        {
            orig.Invoke(self, Type);

            if (Type == ItemID.WilsonBeardLong || Type == ItemID.WilsonBeardMagnificent)
                CustomAchievementsHelper.NotifyItemGrab(Main.LocalPlayer, (short)Type, 1);
        }
    }
}
