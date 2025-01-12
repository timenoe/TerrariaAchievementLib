using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TerrariaAchievementLib.Achievements;

namespace TerrariaAchievementLib.Items
{
    public class LootItem : GlobalItem
    {
        public override void OnSpawn(Item item, IEntitySource source)
        {
            if (source is EntitySource_Loot loot)
            {
                if (loot.Entity is NPC npc)
                    CustomAchievementsHelper.NotifyNpcLoot(npc, item.type);
            }

            else if (source is EntitySource_TileBreak tile)
                CustomAchievementsHelper.NotifyTileDrop(item.type);
        }
    }
}
