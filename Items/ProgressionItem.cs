﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaAchievementLib.Achievements;

namespace TerrariaAchievementLib.Items
{
    /// <summary>
    /// Blocks items from being used based on world progression
    /// </summary>
    public class ProgressionItem : GlobalItem
    {
        public override bool? CanBeChosenAsAmmo(Item ammo, Item weapon, Player player)
        {
            if (ammo.ammo == AmmoID.None)
                return false;

            else
                return AchievementProgression.IsElementAllowed(ProgressionElement.Ammo, ammo.type);
        }

        public override bool CanUseItem(Item item, Player player) => AchievementProgression.IsElementAllowed(ProgressionElement.Usable, item.type);
    }
}
