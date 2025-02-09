using Terraria;
using Terraria.ModLoader;
using TerrariaAchievementLib.Achievements;

namespace TerrariaAchievementLib.Players
{
    /// <summary>
    /// Sends notifications to achievement conditions for various player events
    /// </summary>
    public class AchPlayer : ModPlayer
    {
        public override void Load()
        {
            if (Main.dedServ)
                return;

            On_Player.AddBuff += On_Player_AddBuff;
            On_Player.DropItemFromExtractinator += On_Player_DropItemFromExtractinator;
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            On_Player.AddBuff -= On_Player_AddBuff;
            On_Player.DropItemFromExtractinator -= On_Player_DropItemFromExtractinator;
        }

        public override bool CanUseItem(Item item)
        {
            AchHelper.NotifyItemUse(Player, item.type);
            return true;
        }

        public override void ModifyCaughtFish(Item fish) => AchHelper.NotifyItemCatch(Player, fish.type);

        public override void OnCatchNPC(NPC npc, Item item, bool failed)
        {
            if (!failed)
                AchHelper.NotifyNpcCatch(Player, npc.type);
        }

        public override void PostBuyItem(NPC vendor, Item[] shopInventory, Item item) => AchHelper.NotifyNpcBuy(Player, (short)vendor.type, item.type);


        /// <summary>
        /// Detour to send a notification when the local player activates a buff
        /// </summary>
        /// <param name="orig">Original AddBuff</param>
        /// <param name="self">Player adding the buff</param>
        /// <param name="type">type AddBuff parameter</param>
        /// <param name="timeToAdd">timeToAdd AddBuff parameter</param>
        /// <param name="quiet">quiet AddBuff parameter</param>
        /// <param name="foodHack">foodHack AddBuff parameter</param>
        private void On_Player_AddBuff(On_Player.orig_AddBuff orig, Player self, int type, int timeToAdd, bool quiet, bool foodHack)
        {
            orig.Invoke(self, type, timeToAdd, quiet, foodHack);

            AchHelper.NotifyBuffUse(self, type);
        }

        /// <summary>
        /// Detour to send a notification when the local player gets an item from an extractinator
        /// </summary>
        /// <param name="orig">Original DropItemFromExtractinator</param>
        /// <param name="self">Player getting the item from the extractinator</param>
        /// <param name="itemType">itemType DropItemFromExtractinator parameter</param>
        /// <param name="stack">stack DropItemFromExtractinator parameter</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void On_Player_DropItemFromExtractinator(On_Player.orig_DropItemFromExtractinator orig, Player self, int itemType, int stack)
        {
            orig.Invoke(self, itemType, stack);

            AchHelper.NotifyItemExtract(self, itemType);
        }
    }
}
