﻿using System.Collections.Generic;
using Terraria;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for item(s) to be grabbed
    /// </summary>
    public class ItemGrabCondition : AchIdCondition
    {
        /// <summary>
        /// Base condition identifier (used for saving to achievements.dat)
        /// </summary>
        private const string CustomName = "CUSTOM_ITEM_GRAB";


        /// <summary>
        /// True if the class has been hooked to the helper event
        /// </summary>
        private static bool _isHooked;

        /// <summary>
        /// IDs and the conditions that are listening for them to be triggered
        /// </summary>
        private static readonly Dictionary<int, List<ItemGrabCondition>> _listeners = [];


        /// <summary>
        /// Creates a condition that listens for the item to be grabbed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Item ID to listen for</param>
        private ItemGrabCondition(ConditionReqs reqs, int id) : base(CustomName, reqs, [id]) => Listen(this);

        /// <summary>
        /// Creates a condition that listens for any of the items to be grabbed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        private ItemGrabCondition(ConditionReqs reqs, int[] ids) : base(CustomName, reqs, ids) => Listen(this);


        /// <summary>
        /// Helper to create a condition that listens for the item to be grabbed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Item ID to listen for</param>
        /// <returns>Item grab achievement condition</returns>
        public static CustomAchievementCondition Grab(ConditionReqs reqs, int id) => new ItemGrabCondition(reqs, id);

        /// <summary>
        /// Helper to create a condition that listens for any of the items to be grabbed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        /// <returns>Item grab achievement condition</returns>
        public static CustomAchievementCondition GrabAny(ConditionReqs reqs, params int[] ids) => new ItemGrabCondition(reqs, ids);

        /// <summary>
        /// Helper to create a condition that listens for all of the items to be grabbed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Item IDs to listen for</param>
        /// <returns>Item grab achievement conditions</returns>
        public static List<CustomAchievementCondition> GrabAll(ConditionReqs reqs, params int[] ids)
        {
            List<CustomAchievementCondition> conditions = [];
            foreach (var id in ids)
                conditions.Add(new ItemGrabCondition(reqs, id));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when an item is grabbed
        /// </summary>
        /// <param name="player">Player that grabbed the item</param>
        /// <param name="id">Item ID that was grabbed</param>
        /// <param name="count">Count of the grabbed item(s)</param>
        private static void CustomAchievementsHelper_OnItemPickup(Player player, int id, int count)
        {
            if (!IsListeningForId(id, _listeners, out var conditions))
                return;

            foreach (var condition in conditions)
            {
                if (condition.Reqs.Pass(player))
                    condition.Complete();
            }
        }

        /// <summary>
        /// Listen for events so the condition can be completed
        /// <param name="condition">Achievement condition</param>
        /// </summary>
        private static void Listen(ItemGrabCondition condition)
        {
            if (!_isHooked)
            {
                CustomAchievementsHelper.OnItemGrab += CustomAchievementsHelper_OnItemPickup;
                _isHooked = true;
            }

            ListenForIds(condition, condition.Ids, _listeners);
        }
    }
}
