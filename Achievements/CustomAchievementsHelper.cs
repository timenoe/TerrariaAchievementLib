using Terraria;

namespace TerrariaAchievementLib.Achievements
{
    /// <summary>
    /// Helper to notify achievement conditions when specific events occur
    /// </summary>
    public class CustomAchievementsHelper
    {
        /// <summary>
        /// Signature that defines a buff activation event
        /// </summary>
        /// <param name="player">Player that activated the buff</param>
        /// <param name="id">Buff ID that was activated</param>
        public delegate void BuffActivationEvent(Player player, int id);

        /// <summary>
        /// Signature that defines a NPC loot drop event
        /// </summary>
        /// <param name="npc">NPC that dropped the loot</param>
        /// <param name="id">Item ID of the loot</param>
        public delegate void NpcLootEvent(NPC npc, int id);

        /// <summary>
        /// Signature that defines an item consume event
        /// </summary>
        /// <param name="player">Player that consumed the item</param>
        /// <param name="id">Item ID that was consumed</param>
        public delegate void ItemConsumeEvent(Player player, int id);

        /// <summary>
        /// Signature that defines a special flag event
        /// </summary>
        /// <param name="player">Player that raised the special flag</param>
        /// <param name="id">Special flag ID</param>
        public delegate void SpecialFlagEvent(Player player, int id);


        /// <summary>
        /// Event that is invoked when a buff is activated
        /// </summary>
        public static event BuffActivationEvent OnBuffActivation;

        /// <summary>
        /// Event that is invoked when an NPC drops loot
        /// </summary>
        public static event NpcLootEvent OnNpcLoot;

        /// <summary>
        /// Event that is invoked when an item is consumed
        /// </summary>
        public static event ItemConsumeEvent OnItemConsume;

        /// <summary>
        /// Event that is invoked when a special flag is raised
        /// </summary>
        public static event SpecialFlagEvent OnSpecialFlag;


        /// <summary>
        /// Helper to notify achievement conditions when a buff is activated
        /// </summary>
        /// <param name="player">Player that activated the Buff</param>
        /// <param name="id">Buff ID that was activated</param>
        public static void NotifyBuffActivation(Player player, int id) => OnBuffActivation?.Invoke(player, id);

        
        /// <summary>
        /// Helper to notify achievement conditions when a buff is activated
        /// </summary>
        /// <param name="npc">NPC that dropped the loot</param>
        /// <param name="id">Item ID of the loot</param>
        public static void NotifyNpcLoot(NPC npc, int id) => OnNpcLoot?.Invoke(npc, id);

        /// <summary>
        /// Helper to notify achievement conditions when an item is consumed
        /// </summary>
        /// <param name="player">Player that consumed the item</param>
        /// <param name="id">Item ID that was consumed</param>
        public static void NotifyItemConsume(Player player, int id) => OnItemConsume?.Invoke(player, id);

        /// <summary>
        /// Helper to notify achievement conditions when a special flag is raised
        /// </summary>
        /// <param name="player">Player that raised the special flag</param>
        /// <param name="id">Special flag ID</param>
        public static void NotifySpecialFlag(Player player, int id) => OnSpecialFlag?.Invoke(player, id);
    }
}
