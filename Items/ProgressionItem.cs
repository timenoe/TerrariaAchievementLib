using Terraria;
using Terraria.ModLoader;
using TerrariaAchievementLib.Achievements;

namespace TerrariaAchievementLib.Items
{
    public class ProgressionItem : GlobalItem
    {
        public override bool? CanBeChosenAsAmmo(Item ammo, Item weapon, Player player) => AchProgression.IsElementAllowed(ProgressionElement.Ammo, ammo.type);

        public override bool CanUseItem(Item item, Player player) => AchProgression.IsElementAllowed(ProgressionElement.Usable, item.type);
    }
}
