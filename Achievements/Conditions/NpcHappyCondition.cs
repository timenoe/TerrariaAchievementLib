using System.Collections.Generic;
using Terraria;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for NPC(s) to be happy
    /// </summary>
    public class NpcHappyCondition : AchIdCondition
    {
        /// <summary>
        /// Base condition identifier (used for saving to achievements.dat)
        /// </summary>
        private const string CustomName = "CUSTOM_NPC_HAPPY";


        /// <summary>
        /// True if the class has been hooked to the helper event
        /// </summary>
        private static bool _isHooked;

        /// <summary>
        /// IDs and the conditions that are listening for them to be triggered
        /// </summary>
        private static readonly Dictionary<int, List<NpcHappyCondition>> _listeners = [];


        /// <summary>
        /// Creates a condition that listens for the NPC to be happy
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">NPC ID to listen for</param>
        private NpcHappyCondition(ConditionReqs reqs, int id) : base(CustomName, reqs, [id]) => Listen(this);

        /// <summary>
        /// Creates a condition that listens for any of the NPCs to be happy
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">NPC IDs to listen for</param>
        private NpcHappyCondition(ConditionReqs reqs, int[] ids) : base(CustomName, reqs, ids) => Listen(this);


        /// <summary>
        /// Helper to create a condition that listens for the NPC to be happy
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">NPC ID to listen for</param>
        /// <returns>NPC happy achievement condition</returns>
        public static CustomAchievementCondition Happy(ConditionReqs reqs, int id) => new NpcHappyCondition(reqs, id);

        /// <summary>
        /// Helper to create a condition that listens for any of the NPCs to be happy
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">NPC IDs to listen for</param>
        /// <returns>NPC happy achievement condition</returns>
        public static CustomAchievementCondition HappyAny(ConditionReqs reqs, params int[] ids) => new NpcHappyCondition(reqs, ids);

        /// <summary>
        /// Helper to create a condition that listens for all of the NPCs to be happy
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">NPC IDs to listen for</param>
        /// <returns>NPC happy achievement conditions</returns>
        public static List<CustomAchievementCondition> HappyAll(ConditionReqs reqs, params int[] ids)
        {
            List<CustomAchievementCondition> conditions = [];
            foreach (var id in ids)
                conditions.Add(new NpcHappyCondition(reqs, id));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when an NPC is happy
        /// </summary>
        /// <param name="player">Player that talked to the NPC</param>
        /// <param name="id">NPC ID that is happy</param>
        private static void CustomAchievementsHelper_OnNpcHappy(Player player, int id)
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
        private static void Listen(NpcHappyCondition condition)
        {
            if (!_isHooked)
            {
                CustomAchievementsHelper.OnNpcHappy += CustomAchievementsHelper_OnNpcHappy;
                _isHooked = true;
            }

            ListenForIds(condition, condition.Ids, _listeners);
        }
    }
}
