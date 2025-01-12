using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Achievements;

namespace TerrariaAchievementLib.Achievements.Conditions
{
    /// <summary>
    /// Helper to create a condition that listens for tile(s) to be destroyed
    /// </summary>
    public class CustomTileDestroyCondition : CustomIdCondition
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
        /// Creates a condition that listens for the tile to be destroyed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Tile ID to listen for</param>
        private CustomTileDestroyCondition(ConditionRequirements reqs, int id) : base(CustomName, reqs, [id]) { }

        /// <summary>
        /// Creates a condition that listens for any of the tiles to be destroyed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Tile IDs to listen for</param>
        private CustomTileDestroyCondition(ConditionRequirements reqs, int[] ids) : base(CustomName, reqs, ids) { }


        protected override void HookIdEvent()
        {
            if (!_isHooked)
            {
                AchievementsHelper.OnTileDestroyed += AchievementsHelper_OnTileDestroyed;
                _isHooked = true;
            }
        }


        /// <summary>
        /// Helper to create a condition that listens for the tile to be destroyed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="id">Tile ID to listen for</param>
        /// <returns>Tile destroy achievement condition</returns>
        public static CustomAchievementCondition Destroy(ConditionRequirements reqs, int id) => new CustomTileDestroyCondition(reqs, id);

        /// <summary>
        /// Helper to create a condition that listens for any of the tiles to be destroyed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Tile IDs to listen for</param>
        /// <returns>Tile destroy achievement condition</returns>
        public static CustomAchievementCondition DestroyAny(ConditionRequirements reqs, params int[] ids) => new CustomTileDestroyCondition(reqs, ids);

        /// <summary>
        /// Helper to create a condition that listens for all of the tiles to be destroyed
        /// </summary>
        /// <param name="reqs">Conditions requirements that must be met</param>
        /// <param name="ids">Tile IDs to listen for</param>
        /// <returns>Tile destroy achievement conditions</returns>
        public static List<CustomAchievementCondition> DestroyAll(ConditionRequirements reqs, params int[] ids)
        {
            List<CustomAchievementCondition> conditions = [];
            foreach (var id in ids)
                conditions.Add(new CustomTileDestroyCondition(reqs, id));
            return conditions;
        }

        /// <summary>
        /// Hook that is called when a tile is destroyed
        /// </summary>
        /// <param name="player">Player that destroyed the tile</param>
        /// <param name="id">Tile ID that was destroyed</param>
        private static void AchievementsHelper_OnTileDestroyed(Player player, ushort id)
        {
            if (!IsListeningForId(id, out var conditions))
                return;

            foreach (var condition in conditions)
            {
                if (condition.Reqs.Pass(player))
                    condition.Complete();
            }
        }
    }
}
