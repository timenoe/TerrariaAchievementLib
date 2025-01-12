﻿using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Achievements;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for item(s) to be crafted
    /// </summary>
    public class CustomItemCraftCondition : CustomIdCondition
    {
        /// <summary>
        /// Base condition identifier (used for saving to achievements.dat)
        /// </summary>
        private const string CustomName = "CUSTOM_ITEM_CRAFT";


        /// <summary>
        /// True if the class has been hooked to the helper event
        /// </summary>
        private static bool _isHooked;


        /// <summary>
        /// Creates a condition that listens for the item to be crafted
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Item ID to listen for</param>
        private CustomItemCraftCondition(ConditionRequirements reqs, int id) : base(CustomName, reqs, [id]) { }

        /// <summary>
        /// Creates a condition that listens for any of the items to be crafted
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        private CustomItemCraftCondition(ConditionRequirements reqs, int[] ids) : base(CustomName, reqs, ids) { }


        protected override void HookIdEvent()
        {
            if (!_isHooked)
            {
                AchievementsHelper.OnItemCraft += AchievementsHelper_OnItemCraft;
                _isHooked = true;
            }
        }


        /// <summary>
        /// Helper to create a condition that listens for the item to be crafted
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Item ID to listen for</param>
        /// <returns>Item craft achievement condition</returns>
        public static CustomAchievementCondition Craft(ConditionRequirements reqs, int id) => new CustomItemCraftCondition(reqs, id);

        /// <summary>
        /// Helper to create a condition that listens for any of the items to be crafted
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        /// <returns>Item craft achievement condition</returns>
        public static CustomAchievementCondition CraftAny(ConditionRequirements reqs, params int[] ids) => new CustomItemCraftCondition(reqs, ids);

        /// <summary>
        /// Helper to create a condition that listens for all of the items to be crafted
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        /// <returns>Item craft achievement conditions</returns>
        public static List<CustomAchievementCondition> CraftAll(ConditionRequirements reqs, params int[] ids)
        {
            List<CustomAchievementCondition> conditions = [];
            foreach (var id in ids)
                conditions.Add(new CustomItemCraftCondition(reqs, id));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when an item is crafted
        /// </summary>
        /// <param name="id">Item ID that was crafted</param>
        /// <param name="count">Count of the crafted item</param>
        private static void AchievementsHelper_OnItemCraft(short id, int count)
        {
            if (!IsListeningForId(id, out var conditions))
                return;

            foreach (var condition in conditions)
            {
                if (condition.Reqs.Pass(Main.LocalPlayer))
                    condition.Complete();
            }
        }
    }
}
