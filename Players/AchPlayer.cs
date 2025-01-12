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
        public override void Load() => On_Player.AddBuff += On_Player_AddBuff;

        public override void Unload() => On_Player.AddBuff -= On_Player_AddBuff;

        public override bool CanUseItem(Item item)
        {
            AchHelper.NotifyItemUse(Player, item.type);
            return true;
        }

        public override void ModifyCaughtFish(Item fish) => AchHelper.NotifyFishCatch(Player, fish.type);

        public override void OnCatchNPC(NPC npc, Item item, bool failed)
        {
            if (!failed)
                AchHelper.NotifyNpcCatch(Player, npc.type);
        }

        public override void PostBuyItem(NPC vendor, Item[] shopInventory, Item item) => AchHelper.NotifyNpcBuy(Player, vendor, item.type);


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
            AchHelper.NotifyBuffUse(self, type);

            orig.Invoke(self, type, timeToAdd, quiet, foodHack);
        }
    }
}
