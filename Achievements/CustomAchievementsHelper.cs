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
        /// Signature that defines a fish catch event
        /// </summary>
        /// <param name="player">Player that caught the fish</param>
        /// <param name="id">Item ID that was caught</param>
        public delegate void FishCatchEvent(Player player, int id);

        /// <summary>
        /// Signature that defines a NPC buy event
        /// </summary>
        /// <param name="player">Player that bought the item</param>
        /// <param name="npc">NPC that sold the item</param>
        /// <param name="id">Item ID that was bought</param>
        public delegate void NpcBuyEvent(Player player, NPC npc, int id);

        /// <summary>
        /// Signature that defines a NPC catch event
        /// </summary>
        /// <param name="player">Player that caught the NPC</param>
        /// <param name="id">NPC ID that was caught</param>
        public delegate void NpcCatchEvent(Player player, int id);

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
        /// Signature that defines a tile drop event
        /// </summary>
        /// <param name="id">Item ID of the drop</param>
        public delegate void TileDropEvent(int id);


        /// <summary>
        /// Event that is invoked when a buff is activated
        /// </summary>
        public static event BuffActivationEvent OnBuffActivation;

        /// <summary>
        /// Event that is invoked when a fish is caught
        /// </summary>
        public static event FishCatchEvent OnFishCatch;

        /// <summary>
        /// Event that is invoked when an item is bought from an NPC
        /// </summary>
        public static event NpcBuyEvent OnNpcBuy;

        /// <summary>
        /// Event that is invoked when an NPC is caught
        /// </summary>
        public static event NpcCatchEvent OnNpcCatch;

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
        /// Event that is invoked when a tile drops an item
        /// </summary>
        public static event TileDropEvent OnTileDrop;


        /// <summary>
        /// Helper to notify achievement conditions when a buff is activated
        /// </summary>
        /// <param name="player">Player that activated the Buff</param>
        /// <param name="id">Buff ID that was activated</param>
        public static void NotifyBuffActivation(Player player, int id) => OnBuffActivation?.Invoke(player, id);

        /// <summary>
        /// Helper to notify achievement conditions when a fish is caught
        /// </summary>
        /// <param name="player">Player that caught the fish</param>
        /// <param name="id">Item ID that was caught</param>
        public static void NotifyFishCatch(Player player, int id) => OnFishCatch?.Invoke(player, id);

        /// <summary>
        /// Helper to notify achievement conditions when a item is bought from an NPC
        /// </summary>
        /// <param name="player">Player that bought the item</param>
        /// <param name="npc">NPC that sold the item</param>
        /// <param name="id">Item ID that was bought</param>
        public static void NotifyNpcBuy(Player player, NPC npc, int id) => OnNpcBuy?.Invoke(player, npc, id);

        /// <summary>
        /// Helper to notify achievement conditions when an NPC is caught
        /// </summary>
        /// <param name="player">Player that activated the Buff</param>
        /// <param name="id">NPC ID that was caught</param>
        public static void NotifyNpcCatch(Player player, int id) => OnNpcCatch?.Invoke(player, id);

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

        /// <summary>
        /// Helper to notify achievement conditions when a tile has dropped an item
        /// </summary>
        /// <param name="id">Item ID of the drop</param>
        public static void NotifyTileDrop(int id) => OnTileDrop?.Invoke(id);
    }
}
