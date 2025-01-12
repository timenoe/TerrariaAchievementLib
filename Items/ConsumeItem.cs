using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TerrariaAchievementLib.Achievements;

namespace TerrariaAchievementLib.Items
{
    /// <summary>
    /// Sends notifications to achievement conditions when a specific item is consumed by the local player
    /// </summary>
    public abstract class ConsumeItem : GlobalItem
    {
        /// <summary>
        /// Item IDs to send consume notifications for
        /// </summary>
        private readonly List<int> _itemIds = [];


        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => _itemIds.Contains(entity.type);

        public override void OnConsumeItem(Item item, Player player)
        { 
            if (player == Main.LocalPlayer)
                CustomAchievementsHelper.NotifyItemConsume(player, item.type);
        }


        /// <summary>
        /// Register item IDs to send notifications when they are consumed<br/>
        /// Makes sure this class only runs for the desired items
        /// </summary>
        /// <param name="ids"></param>
        public void RegisterItems(List<int> ids)
        {
            foreach (var id in ids)
                if (!_itemIds.Contains(id))
                    _itemIds.Add(id);
        }
    }
}
