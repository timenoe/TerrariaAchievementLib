using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaAchievementLib.Achievements;

namespace TerrariaAchievementLib.Players
{
    /// <summary>
    /// Blocks the player from using items based on current world progression
    /// </summary>
    public class ProgressionPlayer : ModPlayer
    {
        public override void Load()
        {
            if (Main.dedServ)
                return;
            
            On_AchievementsHelper.HandleOnEquip += On_AchievementsHelper_HandleOnEquip;
            On_Player.AddBuff += On_Player_AddBuff;
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            On_AchievementsHelper.HandleOnEquip -= On_AchievementsHelper_HandleOnEquip;
            On_Player.AddBuff -= On_Player_AddBuff;
        }

        public override void OnEnterWorld()
        {
            UnequipDisallowedItems(Player);

            if (Main.hardMode)
                return;
            
            // Return consumed Life Fruit
            if (Player.ConsumedLifeFruit > 0)
            {
                for (int i = 0; i < Player.ConsumedLifeFruit; i++)
                    Item.NewItem(new EntitySource_DropAsItem(Player), Player.Center, ItemID.LifeFruit);
                
                Player.ConsumedLifeFruit = 0;
            }

            // Return consumed Aegis Fruit
            if (Player.usedAegisFruit)
            {
                Item.NewItem(new EntitySource_DropAsItem(Player), Player.Center, ItemID.AegisFruit);
                Player.usedAegisFruit = false;
            }

            // Return consumed Demon Heart
            if (Player.extraAccessory)
            {
                Item.NewItem(new EntitySource_DropAsItem(Player), Player.Center, ItemID.DemonHeart);
                Player.extraAccessory = false;
            }
        }

        private void On_AchievementsHelper_HandleOnEquip(On_AchievementsHelper.orig_HandleOnEquip orig, Player player, Item item, int context)
        {
            orig.Invoke(player, item, context);

            UnequipDisallowedItems(player);
        }

        private void On_Player_AddBuff(On_Player.orig_AddBuff orig, Player self, int type, int timeToAdd, bool quiet, bool foodHack)
        {
            if (!AchProgression.IsElementAllowed(ProgressionElement.Buff, type))
                return;

            orig.Invoke(self, type, timeToAdd, quiet, foodHack);
        }

        private static void UnequipDisallowedItems(Player player)
        {
            // Armor and Accessories
            for (int slot = 0; slot <= 9; slot++)
            {
                if (!AchProgression.IsElementAllowed(ProgressionElement.Equippable, player.armor[slot].type))
                {
                    player.DropItem(new EntitySource_DropAsItem(player), player.Center, ref player.armor[slot]);
                    player.armor[slot].ChangeItemType(0);
                }
            }

            // Mount and Hook
            for (int slot = 3; slot <= 4; slot++)
            {
                if (!AchProgression.IsElementAllowed(ProgressionElement.Equippable, player.miscEquips[slot].type))
                {
                    player.DropItem(new EntitySource_DropAsItem(player), player.Center, ref player.miscEquips[slot]);
                    player.miscEquips[slot].ChangeItemType(0);
                }
            }
        }
    }
}
