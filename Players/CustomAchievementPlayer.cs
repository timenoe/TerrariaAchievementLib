﻿using Terraria;
using Terraria.ModLoader;
using TerrariaAchievementLib.Achievements;

namespace TerrariaAchievementLib.Players
{
    /// <summary>
    /// Sends notifications to achievement conditions when the local player activates a buff
    /// </summary>
    public class CustomAchievementPlayer : ModPlayer
    {
        public override void Load() => On_Player.AddBuff += On_Player_AddBuff;

        public override void Unload() => On_Player.AddBuff -= On_Player_AddBuff;

        public override void PostBuyItem(NPC vendor, Item[] shopInventory, Item item)
        {
            if (this.Player == Main.LocalPlayer)
                CustomAchievementsHelper.NotifyNpcBuy(this.Player, vendor, item.type);

            base.PostBuyItem(vendor, shopInventory, item);
        }

        public override void ModifyCaughtFish(Item fish)
        {
            base.ModifyCaughtFish(fish);
        }

        public override bool? CanCatchNPC(NPC target, Item item)
        {
            if (this.Player == Main.LocalPlayer)
                CustomAchievementsHelper.NotifyNpcCatch(this.Player, target.type);

            return base.CanCatchNPC(target, item);
        }

        public override bool CanUseItem(Item item)
        {
            if (this.Player == Main.LocalPlayer)
                CustomAchievementsHelper.NotifyItemConsume(this.Player, item.type);

            return base.CanUseItem(item);
        }

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
            if (self == Main.LocalPlayer)
                CustomAchievementsHelper.NotifyBuffActivation(self, type);

            orig.Invoke(self, type, timeToAdd, quiet, foodHack);
        }
    }
}
