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
        /// Item ID to consume
        /// </summary>
        protected abstract short Id { get; }

        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == Id;

        public override void OnConsumeItem(Item item, Player player)
        { 
            if (player == Main.LocalPlayer)
                NewAchievementsHelper.NotifyItemConsume(player, item.type);
        } 
    }
}
