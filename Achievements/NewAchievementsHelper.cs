using Terraria;

namespace TerrariaAchievementLib.Achievements
{
    /// <summary>
    /// Helper to notify achievement conditions when specific events occur
    /// </summary>
    public class NewAchievementsHelper
    {
        /// <summary>
        /// Signature that defines a buff activation event
        /// </summary>
        /// <param name="player">Player that activated the buff</param>
        /// <param name="buffId">Buff ID that was activated</param>
        public delegate void BuffActivationEvent(Player player, int buffId);

        /// <summary>
        /// Signature that defines an item consume event
        /// </summary>
        /// <param name="player">Player that consumed the item</param>
        /// <param name="itemId">Item ID that was consumed</param>
        public delegate void ItemConsumeEvent(Player player, int itemId);


        /// <summary>
        /// Event that is invoked when a buff is activated
        /// </summary>
        public static event BuffActivationEvent OnBuffActivation;

        /// <summary>
        /// Event that is invoked when an item is consumed
        /// </summary>
        public static event ItemConsumeEvent OnItemConsume;


        /// <summary>
        /// Helper to notify achievement conditions when a buff is activated
        /// </summary>
        /// <param name="player">Player that activated the Buff</param>
        /// <param name="buffId">Buff ID that was activated</param>
        public static void NotifyBuffActivation(Player player, int itemId) => OnBuffActivation?.Invoke(player, itemId);

        /// <summary>
        /// Helper to notify achievement conditions when an item is consumed
        /// </summary>
        /// <param name="player">Player that consumed the item</param>
        /// <param name="itemId">Item ID that was consumed</param>
        public static void NotifyItemConsume(Player player, int itemId) => OnItemConsume?.Invoke(player, itemId);
    }
}
