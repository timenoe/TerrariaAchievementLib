using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Achievements;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for tile(s) to be destroyed
    /// </summary>
    public class TileDestroyCondition : AchIdCondition
    {
        /// <summary>
        /// Base condition identifier (used for saving to achievements.dat)
        /// </summary>
        private const string CustomName = "CUSTOM_TILE_DESTROY";


        /// <summary>
        /// True if the class has been hooked to the helper event
        /// </summary>
        private static bool _isHooked;

        /// <summary>
        /// IDs and the conditions that are listening for them to be triggered
        /// </summary>
        private static readonly Dictionary<int, List<TileDestroyCondition>> _listeners = [];


        /// <summary>
        /// Creates a condition that listens for the tile to be destroyed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Tile ID to listen for</param>
        private TileDestroyCondition(ConditionReqs reqs, int id) : base(CustomName, reqs, [id]) => Listen(this);

        /// <summary>
        /// Creates a condition that listens for any of the tiles to be destroyed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Tile IDs to listen for</param>
        private TileDestroyCondition(ConditionReqs reqs, int[] ids) : base(CustomName, reqs, ids) => Listen(this);


        /// <summary>
        /// Helper to create a condition that listens for the tile to be destroyed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Tile ID to listen for</param>
        /// <returns>Tile destroy achievement condition</returns>
        public static CustomAchievementCondition Destroy(ConditionReqs reqs, int id) => new TileDestroyCondition(reqs, id);

        /// <summary>
        /// Helper to create a condition that listens for any of the tiles to be destroyed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Tile IDs to listen for</param>
        /// <returns>Tile destroy achievement condition</returns>
        public static CustomAchievementCondition DestroyAny(ConditionReqs reqs, params int[] ids) => new TileDestroyCondition(reqs, ids);

        /// <summary>
        /// Helper to create a condition that listens for all of the tiles to be destroyed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Tile IDs to listen for</param>
        /// <returns>Tile destroy achievement conditions</returns>
        public static List<CustomAchievementCondition> DestroyAll(ConditionReqs reqs, params int[] ids)
        {
            List<CustomAchievementCondition> conditions = [];
            foreach (var id in ids)
                conditions.Add(new TileDestroyCondition(reqs, id));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when a tile is destroyed
        /// </summary>
        /// <param name="player">Player that destroyed the tile</param>
        /// <param name="id">Tile ID that was destroyed</param>
        private static void AchievementsHelper_OnTileDestroyed(Player player, ushort id)
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
        private static void Listen(TileDestroyCondition condition)
        {
            if (!_isHooked)
            {
                AchievementsHelper.OnTileDestroyed += AchievementsHelper_OnTileDestroyed;
                _isHooked = true;
            }

            ListenForIds(condition, condition.Ids, _listeners);
        }
    }
}
