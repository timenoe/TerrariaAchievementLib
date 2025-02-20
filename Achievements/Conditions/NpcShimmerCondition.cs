using System.Collections.Generic;
using Terraria;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for NPC(s) to be shimmered
    /// </summary>
    public class NpcShimmerCondition : AchIdCondition
    {
        /// <summary>
        /// Base condition identifier (used for saving to achievements.dat)
        /// </summary>
        private const string CustomName = "CUSTOM_NPC_SHIMMER";


        /// <summary>
        /// True if the class has been hooked to the helper event
        /// </summary>
        private static bool _isHooked;

        /// <summary>
        /// IDs and the conditions that are listening for them to be triggered
        /// </summary>
        private static readonly Dictionary<int, List<NpcShimmerCondition>> _listeners = [];


        /// <summary>
        /// Creates a condition that listens for the NPC to be shimmered
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">NPC ID to listen for</param>
        private NpcShimmerCondition(ConditionReqs reqs, int id) : base(CustomName, reqs, [id]) => Listen(this);

        /// <summary>
        /// Creates a condition that listens for any of the NPCs to be shimmered
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">NPC IDs to listen for</param>
        private NpcShimmerCondition(ConditionReqs reqs, int[] ids) : base(CustomName, reqs, ids) => Listen(this);


        /// <summary>
        /// Helper to create a condition that listens for the NPC to be shimmered
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">NPC ID to listen for</param>
        /// <returns>NPC shimmer achievement condition</returns>
        public static CustomAchievementCondition Shimmer(ConditionReqs reqs, int id) => new NpcShimmerCondition(reqs, id);

        /// <summary>
        /// Helper to create a condition that listens for any of the NPCs to be shimmered
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">NPC IDs to listen for</param>
        /// <returns>NPC shimmer achievement condition</returns>
        public static CustomAchievementCondition ShimmerAny(ConditionReqs reqs, params int[] ids) => new NpcShimmerCondition(reqs, ids);

        /// <summary>
        /// Helper to create a condition that listens for all of the NPCs to be shimmered
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">NPC IDs to listen for</param>
        /// <returns>NPC shimmer achievement conditions</returns>
        public static List<CustomAchievementCondition> ShimmerAll(ConditionReqs reqs, params int[] ids)
        {
            List<CustomAchievementCondition> conditions = [];
            foreach (var id in ids)
                conditions.Add(new NpcShimmerCondition(reqs, id));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when an NPC is shimmered
        /// </summary>
        /// <param name="player">Player that shimmered the NPC</param>
        /// <param name="id">NPC ID that was shimmered</param>
        private static void CustomAchievementsHelper_OnNpcCatch(Player player, int id)
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
        private static void Listen(NpcShimmerCondition condition)
        {
            if (!_isHooked)
            {
                CustomAchievementsHelper.OnNpcCatch += CustomAchievementsHelper_OnNpcCatch;
                _isHooked = true;
            }

            ListenForIds(condition, condition.Ids, _listeners);
        }
    }
}
