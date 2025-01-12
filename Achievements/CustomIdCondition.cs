using System.Collections.Generic;

namespace TerrariaAchievementLib.Achievements
{
    /// <summary>
    /// Base class for item conditions that are triggered by item events (e.g., Consume or Craft).
    /// </summary>
    public abstract class CustomIdCondition : CustomAchievementCondition
    {
        /// <summary>
        /// IDs and the conditions that are listening for them to be triggered
        /// </summary>
        protected static readonly Dictionary<int, List<CustomIdCondition>> _listeners = [];

        /// <summary>
        /// IDs that need to be triggered to satisfy the condition
        /// </summary>
        protected readonly int[] Ids;


        /// <summary>
        /// Constructor to initialize the base condition
        /// </summary>
        protected CustomIdCondition(string name, ConditionRequirements reqs, int[] ids) : base($"{name}_{reqs.Identifier}-{string.Join(",", ids)}", reqs)
        {
            Ids = ids;
            ListenForIdEvent();
        }


        /// <summary>
        /// Hook method to be implemented by derived classes for the specific ID event
        /// </summary>
        protected abstract void HookIdEvent();


        /// <summary>
        /// Helper to check if any conditions are listening for the item event
        /// </summary>
        protected static bool IsListeningForId(int id, out List<CustomIdCondition> conditions) => _listeners.TryGetValue(id, out conditions);

        /// <summary>
        /// Registers IDs and hooks the event if not already hooked
        /// </summary>
        protected void ListenForIdEvent()
        {
            // Loop through all IDs in the condition
            foreach (int id in Ids)
            {
                // Create empty list of listeners for the ID if there are none
                if (!IsListeningForId(id, out var conditions))
                    conditions = [];

                // Add the current condition to the listeners for this ID
                conditions.Add(this);
            }
        }
    }

}
