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
            ReturnDisallowedConsumables(Player);
            UnequipDisallowedItems(Player);
        }

        /// <summary>
        /// Detour to unequip newly equipped items from the player based on the current world progression
        /// </summary>
        /// <param name="orig">Original HandleOnEquip method</param>
        /// <param name="player">Player that equipped the item</param>
        /// <param name="item">Item that was equipped</param>
        /// <param name="context">Item slot context</param>
        private void On_AchievementsHelper_HandleOnEquip(On_AchievementsHelper.orig_HandleOnEquip orig, Player player, Item item, int context)
        {
            orig.Invoke(player, item, context);

            UnequipDisallowedItems(player);
        }

        /// <summary>
        /// Detour to block buffs from being activated based on the current world progression
        /// </summary>
        /// <param name="orig">Original AddBuff method</param>
        /// <param name="self">Player adding the buff</param>
        /// <param name="type">Buff ID being added</param>
        /// <param name="timeToAdd">Buff time in ticks</param>
        /// <param name="quiet">True if skipping network sync message</param>
        /// <param name="foodHack">Unused parameter</param>
        private void On_Player_AddBuff(On_Player.orig_AddBuff orig, Player self, int type, int timeToAdd, bool quiet, bool foodHack)
        {
            if (!AchievementProgression.IsElementAllowed(ProgressionElement.Buff, type))
                return;

            orig.Invoke(self, type, timeToAdd, quiet, foodHack);
        }

        /// <summary>
        /// Returns any consumables to the player based on the current world progression
        /// </summary>
        /// <param name="player">Player to return consumables to</param>
        private static void ReturnDisallowedConsumables(Player player)
        {
            if (!AchievementProgression.Enabled)
                return;

            // The following items are Hardmode only
            if (Main.hardMode)
                return;
            
            // Return consumed Life Fruit
            if (player.ConsumedLifeFruit > 0)
            {
                for (int i = 0; i < player.ConsumedLifeFruit; i++)
                    Item.NewItem(new EntitySource_DropAsItem(player), player.Center, ItemID.LifeFruit);

                player.ConsumedLifeFruit = 0;
            }

            // Return consumed Aegis Fruit
            if (player.usedAegisFruit)
            {
                Item.NewItem(new EntitySource_DropAsItem(player), player.Center, ItemID.AegisFruit);
                player.usedAegisFruit = false;
            }

            // Return consumed Demon Heart
            if (player.extraAccessory)
            {
                Item.NewItem(new EntitySource_DropAsItem(player), player.Center, ItemID.DemonHeart);
                player.extraAccessory = false;
            }
        }

        /// <summary>
        /// Unequips items from the player based on the current world progression
        /// </summary>
        /// <param name="player">Player to unequip items from</param>
        private static void UnequipDisallowedItems(Player player)
        {
            if (!AchievementProgression.Enabled)
                return;

            // Armor and Accessories
            for (int slot = 0; slot <= 9; slot++)
            {
                if (!AchievementProgression.IsElementAllowed(ProgressionElement.Equippable, player.armor[slot].type))
                {
                    player.DropItem(new EntitySource_DropAsItem(player), player.Center, ref player.armor[slot]);
                    player.armor[slot].ChangeItemType(0);
                }
            }

            // Mount and Hook
            for (int slot = 3; slot <= 4; slot++)
            {
                if (!AchievementProgression.IsElementAllowed(ProgressionElement.Equippable, player.miscEquips[slot].type))
                {
                    player.DropItem(new EntitySource_DropAsItem(player), player.Center, ref player.miscEquips[slot]);
                    player.miscEquips[slot].ChangeItemType(0);
                }
            }
        }
    }
}
