using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaAchievementLib.Achievements;

namespace TerrariaAchievementLib.Players
{
    /// <summary>
    /// Sends notifications to achievement conditions for various player events
    /// </summary>
    public class AchievementPlayer : ModPlayer
    {
        public override void Load()
        {
            if (Main.dedServ)
                return;
            
            On_Player.AddBuff += On_Player_AddBuff;
            On_Player.DropItemFromExtractinator += On_Player_DropItemFromExtractinator;
            On_Player.GetItem += On_Player_GetItem;
            On_Player.SetTalkNPC += On_Player_SetTalkNPC;
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            On_Player.AddBuff -= On_Player_AddBuff;
            On_Player.DropItemFromExtractinator -= On_Player_DropItemFromExtractinator;
            On_Player.GetItem -= On_Player_GetItem;
            On_Player.SetTalkNPC -= On_Player_SetTalkNPC;
        }

        public override bool CanBuyItem(NPC vendor, Item[] shopInventory, Item item)
        {
            // Don't unlock achievements for buying if the player just sold the item to the NPC
            if (!item.buyOnce)
                CustomAchievementsHelper.NotifyNpcBuy(Player, vendor.type, item.type);

            return true;
        }

        public override bool CanUseItem(Item item)
        {
            CustomAchievementsHelper.NotifyItemUse(Player, item.type);
            return true;
        }

        public override void ModifyCaughtFish(Item fish) => CustomAchievementsHelper.NotifyItemCatch(Player, fish.type);

        public override void OnCatchNPC(NPC npc, Item item, bool failed)
        {
            if (!failed)
                CustomAchievementsHelper.NotifyNpcCatch(Player, npc.releaseOwner != 255, npc.type);
        }

        /// <summary>
        /// Detour to send a notification when the local player activates a buff
        /// </summary>
        /// <param name="orig">Original AddBuff method</param>
        /// <param name="self">Player adding the buff</param>
        /// <param name="type">Buff ID being added</param>
        /// <param name="timeToAdd">Buff time in ticks</param>
        /// <param name="quiet">True if skipping network sync message</param>
        /// <param name="foodHack">Unused parameter</param>
        private void On_Player_AddBuff(On_Player.orig_AddBuff orig, Player self, int type, int timeToAdd, bool quiet, bool foodHack)
        {
            orig.Invoke(self, type, timeToAdd, quiet, foodHack);

            CustomAchievementsHelper.NotifyBuffUse(self, type);
        }

        /// <summary>
        /// Detour to send a notification when the local player gets an item from an extractinator
        /// </summary>
        /// <param name="orig">Original DropItemFromExtractinator method</param>
        /// <param name="self">Player getting the item from the extractinator</param>
        /// <param name="itemType">item ID being dropped</param>
        /// <param name="stack">Count of item being dropped</param>
        private void On_Player_DropItemFromExtractinator(On_Player.orig_DropItemFromExtractinator orig, Player self, int itemType, int stack)
        {
            orig.Invoke(self, itemType, stack);

            CustomAchievementsHelper.NotifyItemExtract(self, itemType);
        }

        /// <summary>
        /// Detour to send a notification when the local player gets a gift from an NPC
        /// </summary>
        /// <param name="orig">Original GetItem method</param>
        /// <param name="self">Player getting the gift</param>
        /// <param name="plr">Player index</param>
        /// <param name="newItem">Gift that the player is receiving</param>
        /// <param name="settings">GetItemSettings instance</param>
        /// <returns>Gift that the player got</returns>
        private Item On_Player_GetItem(On_Player.orig_GetItem orig, Player self, int plr, Item newItem, GetItemSettings settings)
        {
            if (settings.Equals(GetItemSettings.NPCEntityToPlayerInventorySettings))
            {
                int npc = NPCID.None;
                if (self.TalkNPC != null)
                    npc = self.TalkNPC.type;

                CustomAchievementsHelper.NotifyNpcGift(self, npc, newItem.type);
            }

            return orig.Invoke(self, plr, newItem, settings);
        }

        /// <summary>
        /// Detour to send a notification when the local player talks to a happy NPC
        /// </summary>
        /// <param name="orig">Original SetTalkNPC method</param>
        /// <param name="self">Player that talked to the NPC</param>
        /// <param name="npcIndex">NPC ID of the NPC</param>
        /// <param name="fromNet">True if a network message</param>
        private void On_Player_SetTalkNPC(On_Player.orig_SetTalkNPC orig, Player self, int npcIndex, bool fromNet)
        {
            orig.Invoke(self, npcIndex, fromNet);

            if (!fromNet && self.currentShoppingSettings.PriceAdjustment <= 0.81999999284744263)
                CustomAchievementsHelper.NotifyNpcHappy(self, npcIndex);
        }
    }
}
