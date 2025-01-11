using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TerrariaAchievementLib.Achievements;

namespace TerrariaAchievementLib.Items
{
    /// <summary>
    /// Sends notifications to achievement conditions when a specific item is consumed by the local player
    /// </summary>
    public abstract class AchievementConsumeItem : GlobalItem
    {
        /// <summary>
        /// Item IDs to consume
        /// </summary>
        protected abstract List<int> ItemIds { get; }

        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => ItemIds.Contains(entity.type);

        public override void OnConsumeItem(Item item, Player player)
        { 
            if (player == Main.LocalPlayer)
                NewAchievementsHelper.NotifyItemConsume(player, item.type);
        } 
    }
}
