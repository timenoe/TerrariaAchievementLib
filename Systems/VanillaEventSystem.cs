using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using TerrariaAchievementLib.Achievements;

namespace TerrariaAchievementLib.Systems
{
    /// <summary>
    /// Hooks vanilla events to custom ones<br/>
    /// Ensures vanilla events are properly unsubscribed to
    /// </summary>
    public class VanillaEventSystem : ModSystem
    {
        public override void OnModLoad()
        {
            if (Main.dedServ)
                return;

            On_AchievementsHelper.HandleOnEquip += On_AchievementsHelper_HandleOnEquip;

            AchievementsHelper.OnItemCraft += CustomAchievementsHelper.NotifyItemCraft;
            AchievementsHelper.OnItemPickup += CustomAchievementsHelper.NotifyItemGrab;
            AchievementsHelper.OnProgressionEvent += CustomAchievementsHelper.NotifyFlagProgression;
            AchievementsHelper.OnNPCKilled += CustomAchievementsHelper.NotifyNpcKill;
            AchievementsHelper.OnTileDestroyed += CustomAchievementsHelper.NotifyTileDestroy;
        }

        public override void OnModUnload()
        {
            if (Main.dedServ)
                return;

            On_AchievementsHelper.HandleOnEquip -= On_AchievementsHelper_HandleOnEquip;

            AchievementsHelper.OnItemCraft -= CustomAchievementsHelper.NotifyItemCraft;
            AchievementsHelper.OnItemPickup -= CustomAchievementsHelper.NotifyItemGrab;
            AchievementsHelper.OnProgressionEvent -= CustomAchievementsHelper.NotifyFlagProgression;
            AchievementsHelper.OnNPCKilled -= CustomAchievementsHelper.NotifyNpcKill;
            AchievementsHelper.OnTileDestroyed -= CustomAchievementsHelper.NotifyTileDestroy;
        }

        /// <summary>
        /// Detour to notify achievement conditions when an item has been equipped
        /// </summary>
        /// <param name="orig">Original HandleOnEquip method</param>
        /// <param name="player">Player that equipped the item</param>
        /// <param name="item">Item ID that was equipped</param>
        /// <param name="context">Item slot context ID</param>
        private void On_AchievementsHelper_HandleOnEquip(On_AchievementsHelper.orig_HandleOnEquip orig, Player player, Item item, int context)
        {
            // Correct invalid contexts
            if (context == ItemSlot.Context.InventoryItem)
            {
                if (item.vanity)
                {
                    if (item.accessory)
                        context = ItemSlot.Context.EquipAccessoryVanity;
                    else
                        context = ItemSlot.Context.EquipArmorVanity;
                }

                else if (item.accessory)
                    context = ItemSlot.Context.EquipAccessory;

                else
                    context = ItemSlot.Context.EquipArmor;
            }

            // Invoke original with the corrected context
            orig.Invoke(player, item, context);

            // Apply custom context ID if applicable
            if ((context == ItemSlot.Context.EquipAccessory || context == ItemSlot.Context.EquipAccessoryVanity) && item.wingSlot > 0)
                context = AchievementData.CustomItemSlotContextID.EquipWings;

            // Notify with just the item slot context ID for equipping anything in that slot
            CustomAchievementsHelper.NotifyItemEquip(player, context, ItemID.None);

            // Notify with the item slot context ID and the specific item ID
            CustomAchievementsHelper.NotifyItemEquip(player, context, item.type);
        }
    }
}
