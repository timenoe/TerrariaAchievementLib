using System.Collections.Generic;
using Terraria;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for buff(s) to be added
    /// </summary>
    public class BuffAddCondition : AchIdCondition
    {
        /// <summary>
        /// Base condition identifier (used for saving to achievements.dat)
        /// </summary>
        private const string CustomName = "CUSTOM_BUFF_ADD";


        /// <summary>
        /// True if the class has been hooked to the helper event
        /// </summary>
        private static bool _isHooked;

        /// <summary>
        /// IDs and the conditions that are listening for them to be triggered
        /// </summary>
        private static readonly Dictionary<int, List<BuffAddCondition>> _listeners = [];


        /// <summary>
        /// Creates a condition that listens for the buff to be added
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Buff ID to listen for</param>
        private BuffAddCondition(ConditionReqs reqs, int id) : base(CustomName, reqs, [id]) => Listen(this);

        /// <summary>
        /// Creates a condition that listens for any of the buffs to be added
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Buff IDs to listen for</param>
        private BuffAddCondition(ConditionReqs reqs, int[] ids) : base(CustomName, reqs, ids) => Listen(this);

        /// <summary>
        /// Helper to create a condition that listens for the buff to be added
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Buff ID to listen for</param>
        /// <returns>Buff add achievement condition</returns>
        public static AchCondition Add(ConditionReqs reqs, int id) => new BuffAddCondition(reqs, id);

        /// <summary>
        /// Helper to create a condition that listens for any of the buffs to be added
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Buff IDs to listen for</param>
        /// <returns>Buff add achievement condition</returns>
        public static AchCondition AddAny(ConditionReqs reqs, params int[] ids) => new BuffAddCondition(reqs, ids);

        /// <summary>
        /// Helper to create a condition that listens for all of the buffs to be added
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Buff IDs to listen for</param>
        /// <returns>Buff add achievement conditions</returns>
        public static List<AchCondition> AddAll(ConditionReqs reqs, params int[] ids)
        {
            List<AchCondition> conditions = [];
            foreach (var id in ids)
                conditions.Add(new BuffAddCondition(reqs, id));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when a buff is added
        /// </summary>
        /// <param name="player">Player that added the buff</param>
        /// <param name="id">Buff ID that was added</param>
        private static void AchHelper_OnBuffAdd(Player player, int id)
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
        private static void Listen(BuffAddCondition condition)
        {
            if (!_isHooked)
            {
                AchHelper.OnBuffAdd += AchHelper_OnBuffAdd;
                _isHooked = true;
            }

            ListenForIds(condition, condition.Ids, _listeners);
        }
    }
}
