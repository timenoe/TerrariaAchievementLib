using System;
using System.Collections.Generic;

namespace TerrariaAchievementLib.Achievements
{
    /// <summary>
    /// Base class for item conditions that are triggered by item events (e.g., Consume or Craft).
    /// </summary>
    public class CustomIdCondition : CustomAchievementCondition
    {
        /// <summary>
        /// IDs that need to be triggered to satisfy the condition
        /// </summary>
        protected readonly int[] Ids;


        /// <summary>
        /// Constructor to initialize the base condition
        /// </summary>
        protected CustomIdCondition(string name, ConditionRequirements reqs, int[] ids) : base($"{name}_{reqs.Identifier}-{string.Join(",", ids)}", reqs) => Ids = ids;

        /// <summary>
        /// Helper to create a condition that listens for the buff to be activated
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Buff ID to listen for</param>
        /// <returns>Buff activate achievement condition</returns>
        public static CustomAchievementCondition Do<T>(ConditionRequirements reqs, int id) => (T)Activator.CreateInstance(typeof(T), reqs, id) as CustomAchievementCondition;

        /// <summary>
        /// Helper to create a condition that listens for any of the IDs to be activated
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Buff IDs to listen for</param>
        /// <returns>Buff activate achievement condition</returns>
        public static CustomAchievementCondition DoAny<T>(ConditionRequirements reqs, params int[] ids) => (T)Activator.CreateInstance(typeof(T), reqs, ids) as CustomAchievementCondition;

        /// <summary>
        /// Helper to create a condition that listens for all of the IDs to be activated
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Buff IDs to listen for</param>
        /// <returns>Buff activate achievement conditions</returns>
        public static List<CustomAchievementCondition> DoAll<T>(ConditionRequirements reqs, params int[] ids)
        {
            List<CustomAchievementCondition> conditions = [];
            foreach (var id in ids)
                conditions.Add((T)Activator.CreateInstance(typeof(T), reqs, id) as CustomAchievementCondition); 
            return conditions;
        }


        /// <summary>
        /// Helper to check if any conditions are listening for the item event
        /// </summary>
        protected static bool IsListeningForId<T>(int id, Dictionary<int, List<T>> listeners, out List<T> conditions) => listeners.TryGetValue(id, out conditions);

        /// <summary>
        /// Registers IDs and hooks the event if not already hooked
        /// </summary>
        protected void ListenForId<T>(T condition, Dictionary<int, List<T>> listeners)
        {
            // Loop through all IDs in the condition
            foreach (int id in Ids)
            {
                // Create empty list of listeners for the ID if there are none
                if (!IsListeningForId(id, listeners, out var conditions))
                {
                    conditions = [];
                    listeners.Add(id, conditions);
                }
                    
                // Add the current condition to the listeners for this ID
                conditions.Add(condition);
            }
        }
    }

}
