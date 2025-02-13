using System.Collections.Generic;
using Terraria;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for NPC(s) to be caught
    /// </summary>
    public class NpcCatchCondition : AchIdCondition
    {
        /// <summary>
        /// Base condition identifier (used for saving to achievements.dat)
        /// </summary>
        private const string CustomName = "CUSTOM_NPC_CATCH";


        /// <summary>
        /// True if the class has been hooked to the helper event
        /// </summary>
        private static bool _isHooked;

        /// <summary>
        /// IDs and the conditions that are listening for them to be triggered
        /// </summary>
        private static readonly Dictionary<int, List<NpcCatchCondition>> _listeners = [];


        /// <summary>
        /// Creates a condition that listens for the NPC to be caught
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">NPC ID to listen for</param>
        private NpcCatchCondition(ConditionReqs reqs, int id) : base(CustomName, reqs, [id]) => Listen(this);

        /// <summary>
        /// Creates a condition that listens for any of the NPCs to be caught
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">NPC IDs to listen for</param>
        private NpcCatchCondition(ConditionReqs reqs, int[] ids) : base(CustomName, reqs, ids) => Listen(this);


        /// <summary>
        /// Helper to create a condition that listens for the NPC to be caught
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">NPC ID to listen for</param>
        /// <returns>NPC catch achievement condition</returns>
        public static CustomAchievementCondition Catch(ConditionReqs reqs, int id) => new NpcCatchCondition(reqs, id);

        /// <summary>
        /// Helper to create a condition that listens for any of the NPCs to be caught
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">NPC IDs to listen for</param>
        /// <returns>NPC catch achievement condition</returns>
        public static CustomAchievementCondition CatchAny(ConditionReqs reqs, params int[] ids) => new NpcCatchCondition(reqs, ids);

        /// <summary>
        /// Helper to create a condition that listens for all of the NPCs to be caught
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">NPC IDs to listen for</param>
        /// <returns>NPC catch achievement conditions</returns>
        public static List<CustomAchievementCondition> CatchAll(ConditionReqs reqs, params int[] ids)
        {
            List<CustomAchievementCondition> conditions = [];
            foreach (var id in ids)
                conditions.Add(new NpcCatchCondition(reqs, id));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when an NPC is caught
        /// </summary>
        /// <param name="player">Player that caught the NPC</param>
        /// <param name="id">NPC ID that was caught</param>
        private static void AchHelper_OnNpcCatch(Player player, int id)
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
        /// </summary>
        private static void Listen(NpcCatchCondition condition)
        {
            if (!_isHooked)
            {
                CustomAchievementHelper.OnNpcCatch += AchHelper_OnNpcCatch;
                _isHooked = true;
            }

            ListenForIds(condition, condition.Ids, _listeners);
        }
    }
}
