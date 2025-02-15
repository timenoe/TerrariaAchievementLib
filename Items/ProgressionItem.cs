using Terraria;
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
            if (AchievementProgression.IsElementAllowed(ProgressionElement.Ammo, ammo.type) == false)
                return false;

            else
                return null;
        }

        public override bool CanUseItem(Item item, Player player) => AchievementProgression.IsElementAllowed(ProgressionElement.Usable, item.type);
    }
}
