using System.Collections.Generic;
using Terraria;
using Terraria.Achievements;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Achievement condition that is satisfied when a buff is activated
    /// </summary>
    public class BuffActivateCondition : AchievementCondition
    {
        /// <summary>
        /// Base condition identifier (used for saving to achievements.dat)
        /// </summary>
        private const string Identifier = "BUFF_ACTIVATE";


        /// <summary>
        /// Buff IDs that need to be activated to satisfy the condition
        /// </summary>
        public readonly int[] BuffIds;

        /// <summary>
        /// Buff IDs and the conditions that are listening for them to be activated
        /// </summary>
        private static readonly Dictionary<int, List<BuffActivateCondition>> _listeners = [];

        /// <summary>
        /// True if the class has been hooked to the helper event
        /// </summary>
        private static bool _isHooked;


        /// <summary>
        /// Creates a condition that listens for the buff to be activated
        /// </summary>
        /// <param name="buffId">Buff ID to listen for</param>
        private BuffActivateCondition(int buffId) : base($"{Identifier}_{buffId}")
        {
            BuffIds = [buffId];
            ListenForBuffActivation(this);
        }

        /// <summary>
        /// Creates a condition that listens for any of the buffs to be activated
        /// </summary>
        /// <param name="buffIds">Buff IDs to listen for</param>
        private BuffActivateCondition(int[] buffIds) : base($"{Identifier}_{string.Join(",", buffIds)}")
        {
            BuffIds = buffIds;
            ListenForBuffActivation(this);
        }


        /// <summary>
        /// Helper to create a condition that listens for the buff to be activated
        /// </summary>
        /// <param name="buffId">Buff ID to listen for</param>
        /// <returns>Buff activate achievement condition</returns>
        public static AchievementCondition Activate(int buffId) => new BuffActivateCondition(buffId);

        /// <summary>
        /// Helper to create a condition that listens for any of the buffs to be activated
        /// </summary>
        /// <param name="buffIds">Buff IDs to listen for</param>
        /// <returns>Buff activate achievement condition</returns>
        public static AchievementCondition ActivateAny(params int[] buffIds) => new BuffActivateCondition(buffIds);

        /// <summary>
        /// Helper to create a condition that listens for all of the buffs to be activated
        /// </summary>
        /// <param name="buffIds">Buff IDs to listen for</param>
        /// <returns>Buff activate achievement conditions</returns>
        public static AchievementCondition[] ActivateAll(params int[] buffIds)
        {
            AchievementCondition[] array = new AchievementCondition[buffIds.Length];

            for (int i = 0; i < buffIds.Length; i++)
                array[i] = new BuffActivateCondition(buffIds[i]);

            return array;
        }

        /// <summary>
        /// Add condition to the list of buff activate listeners
        /// </summary>
        /// <param name="condition">Condition to add to the list of buff activate listeners</param>
        private static void ListenForBuffActivation(BuffActivateCondition condition)
        {
            // Hook the helper event only once for the class
            if (!_isHooked)
            {
                NewAchievementsHelper.OnBuffActivation += NewAchievementsHelper_OnBuffActivation;
                _isHooked = true;
            }

            // Loop through all buffs in the condition
            foreach (int buffId in condition.BuffIds)
            {
                // Create empty list of listeners for the buff if there are none
                if (!_listeners.TryGetValue(buffId, out List<BuffActivateCondition> conditions))
                {
                    conditions = [];
                    _listeners[buffId] = conditions;
                }

                // Add the current condition to the listeners for this buff
                conditions.Add(condition);
            }
        }

        /// <summary>
        /// Hook that is called when a buff is activated
        /// </summary>
        /// <param name="player">Player that activated the buff</param>
        /// <param name="buffId">Buff ID that was activated</param>
        private static void NewAchievementsHelper_OnBuffActivation(Player player, int buffId)
        {
            if (player.whoAmI != Main.myPlayer)
                return;

            if (!_listeners.TryGetValue(buffId, out List<BuffActivateCondition> conditions))
                return;

            foreach (BuffActivateCondition condition in conditions)
                condition.Complete();
        }
    }
}
