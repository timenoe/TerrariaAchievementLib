﻿using Terraria;

namespace TerrariaAchievementLib.Achievements
{
    /// <summary>
    /// Helper to notify achievement conditions when specific events occur
    /// </summary>
    public class AchHelper
    {
        /// <summary>
        /// Signature that defines a buff add event
        /// </summary>
        /// <param name="player">Player that the buff was added to</param>
        /// <param name="id">Buff ID that was added</param>
        public delegate void BuffAddEvent(Player player, int id);

        /// <summary>
        /// Signature that defines a flag special event
        /// </summary>
        /// <param name="player">Player that raised the special flag</param>
        /// <param name="id">Flag ID that was set</param>
        public delegate void FlagSpecialEvent(Player player, int id);

        /// <summary>
        /// Signature that defines a fishing item catch event
        /// </summary>
        /// <param name="player">Player that caught the item</param>
        /// <param name="id">Item ID that was caught</param>
        public delegate void ItemCatchEvent(Player player, int id);

        /// <summary>
        /// Signature that defines an item equip event
        /// </summary>
        /// <param name="player">Player that equipped the item</param>
        /// <param name="context">Item slot context ID</param>
        /// <param name="id">Item ID that was equipped</param>
        public delegate void ItemEquipEvent(Player player, int context, int id);

        /// <summary>
        /// Signature that defines an item extract event
        /// </summary>
        /// <param name="player">Player that extracts the item</param>
        /// <param name="id">Item ID that was extracted</param>
        public delegate void ItemExtractEvent(Player player, int id);

        /// <summary>
        /// Signature that defines an item open event
        /// </summary>
        /// <param name="player">Player that opened the grab bag</param>
        /// <param name="bagId">Grab bag ID that was opened</param>
        /// <param name="itemId">Item ID that was received</param>
        public delegate void ItemOpenEvent(Player player, int bagId, int itemId);

        /// <summary>
        /// Signature that defines an item tree shake event
        /// </summary>
        /// <param name="player">Player that shook the tree</param>
        /// <param name="id">Item ID that appeared</param>
        public delegate void ItemShakeEvent(Player player, int id);

        /// <summary>
        /// Signature that defines an item use event
        /// </summary>
        /// <param name="player">Player that used the item</param>
        /// <param name="id">Item ID that was used</param>
        public delegate void ItemUseEvent(Player player, int id);

        /// <summary>
        /// Signature that defines a NPC buy event
        /// </summary>
        /// <param name="player">Player that bought the item</param>
        /// <param name="npcId">NPC that sold the item</param>
        /// <param name="itemId">Item ID that was bought</param>
        public delegate void NpcBuyEvent(Player player, short npcId, int itemId);

        /// <summary>
        /// Signature that defines a NPC catch event
        /// </summary>
        /// <param name="player">Player that caught the NPC</param>
        /// <param name="id">NPC ID that was caught</param>
        public delegate void NpcCatchEvent(Player player, int id);

        /// <summary>
        /// Signature that defines a NPC drop event
        /// </summary>
        /// <param name="player">NPC that damaged the NPC</param>
        /// <param name="npcId">NPC ID that dropped the item</param>
        /// <param name="itemId">Item ID that was dropped</param>
        public delegate void NpcDropEvent(Player player, short npcId, int itemId);

        /// <summary>
        /// Signature that defines an NPC gift event
        /// </summary>
        /// <param name="player">Player that received the item</param>
        /// <param name="npcId">NPC ID that gave the item</param>
        /// <param name="itemId">Item ID that was gifted</param>
        public delegate void NpcGiftEvent(Player player, int npcId, int itemId);

        /// <summary>
        /// Signature that defines a tile drop event
        /// </summary>
        /// <param name="player">Player that likely broke the tile</param>
        /// <param name="id">Item ID that was dropped</param>
        public delegate void TileDropEvent(Player player, int id);


        /// <summary>
        /// Event that is invoked when a buff is added to the player
        /// </summary>
        public static event BuffAddEvent OnBuffAdd;

        /// <summary>
        /// Event that is invoked when a special flag is set by the player
        /// </summary>
        public static event FlagSpecialEvent OnFlagSpecial;

        /// <summary>
        /// Event that is invoked when an item is caught by the player while fishing
        /// </summary>
        public static event ItemCatchEvent OnItemCatch;

        /// <summary>
        /// Event that is invoked when an item is equipped by the player
        /// </summary>
        public static event ItemEquipEvent OnItemEquip;

        /// <summary>
        /// Event that is invoked when an item is extracted by the player
        /// </summary>
        public static event ItemExtractEvent OnItemExtract;

        /// <summary>
        /// Event that is invoked when an item is opened from a grab bag
        /// </summary>
        public static event ItemOpenEvent OnItemOpen;

        /// <summary>
        /// Event that is invoked when an item is shaken from a tree
        /// </summary>
        public static event ItemShakeEvent OnItemShake;

        /// <summary>
        /// Event that is invoked when an item is used by the player
        /// </summary>
        public static event ItemUseEvent OnItemUse;

        /// <summary>
        /// Event that is invoked when an item is bought from an NPC by the player
        /// </summary>
        public static event NpcBuyEvent OnNpcBuy;

        /// <summary>
        /// Event that is invoked when an NPC is caught by the player
        /// </summary>
        public static event NpcCatchEvent OnNpcCatch;

        /// <summary>
        /// Event that is invoked when an NPC drops an item
        /// </summary>
        public static event NpcDropEvent OnNpcDrop;

        /// <summary>
        /// Event that is invoked when an item is gifted from an NPC to the player
        /// </summary>
        public static event NpcGiftEvent OnNpcGift;

        /// <summary>
        /// Event that is invoked when a tile drops an item
        /// </summary>
        public static event TileDropEvent OnTileDrop;


        /// <summary>
        /// Helper to notify achievement conditions when a buff is added to the player
        /// </summary>
        /// <param name="player">Player that the buff was added to</param>
        /// <param name="id">Buff ID that was added</param>
        public static void NotifyBuffUse(Player player, int id) => OnBuffAdd?.Invoke(player, id);

        /// <summary>
        /// Helper to notify achievement conditions when a special flag is set by the player
        /// </summary>
        /// <param name="player">Player that set the special flag</param>
        /// <param name="id">Flag ID that was set</param>
        public static void NotifyFlagSpecial(Player player, int id) => OnFlagSpecial?.Invoke(player, id);

        /// <summary>
        /// Helper to notify achievement conditions when an item is caught by the player while fishing
        /// </summary>
        /// <param name="player">Player that caught the item</param>
        /// <param name="id">Item ID that was caught</param>
        public static void NotifyItemCatch(Player player, int id) => OnItemCatch?.Invoke(player, id);

        /// <summary>
        /// Helper to notify achievement conditions when an item is equipped by the player
        /// </summary>
        /// <param name="player">Player that equipped the item</param>
        /// <param name="context">Item slot context ID</param>
        /// <param name="id">Item ID that was equipped</param>
        public static void NotifyItemEquip(Player player, int context, int id) => OnItemEquip?.Invoke(player, context, id);

        /// <summary>
        /// Helper to notify achievement conditions when an item is extracted by the player
        /// </summary>
        /// <param name="player">Player that extracted the item</param>
        /// <param name="id">Item ID that was extracted</param>
        public static void NotifyItemExtract(Player player, int id) => OnItemExtract?.Invoke(player, id);

        /// <summary>
        /// Helper to notify achievement conditions when an item is opened from a grab bag
        /// </summary>
        /// <param name="player">Player that opened the grab bag</param>
        /// <param name="bagId">Grab bag ID that was opened/param>
        /// <param name="itemId">Item ID that was received</param>
        public static void NotifyItemOpen(Player player, int bagId, int itemId) => OnItemOpen?.Invoke(player, bagId, itemId);

        /// <summary>
        /// Helper to notify achievement conditions when an item is shaken from a tree
        /// </summary>
        /// <param name="player">Player that shook the tree</param>
        /// <param name="id">Item ID that appeared</param>
        public static void NotifyItemShake(Player player, int id) => OnItemShake?.Invoke(player, id);

        /// <summary>
        /// Helper to notify achievement conditions when an item is used by the player
        /// </summary>
        /// <param name="player">Player that used the item</param>
        /// <param name="id">Item ID that was used</param>
        public static void NotifyItemUse(Player player, int id) => OnItemUse?.Invoke(player, id);

        /// <summary>
        /// Helper to notify achievement conditions when a item is bought from an NPC by the player
        /// </summary>
        /// <param name="player">Player that bought the item</param>
        /// <param name="npcId">NPC ID that sold the item</param>
        /// <param name="itemId">Item ID that was bought</param>
        public static void NotifyNpcBuy(Player player, short npcId, int itemId) => OnNpcBuy?.Invoke(player, npcId, itemId);

        /// <summary>
        /// Helper to notify achievement conditions when an NPC is caught by the player
        /// </summary>
        /// <param name="player">Player that caught the NPC</param>
        /// <param name="id">NPC ID that was caught</param>
        public static void NotifyNpcCatch(Player player, int id) => OnNpcCatch?.Invoke(player, id);

        /// <summary>
        /// Helper to notify achievement conditions when an NPC drops an item
        /// </summary>
        /// <param name="player">Player that damaged the NPC</param>
        /// <param name="npcId">NPC ID that dropped the item</param>
        /// <param name="itemId">Item ID that was dropped</param>
        public static void NotifyNpcDrop(Player player, short npcId, int itemId) => OnNpcDrop?.Invoke(player, npcId, itemId);

        /// <summary>
        /// Helper to notify achievement conditions when an item is gifted from an NPC to the player
        /// </summary>
        /// <param name="player">Player that received the item</param>
        /// <param name="npcId">NPC ID that gave the item</param>
        /// <param name="itemId">Item ID that was gifted</param>
        public static void NotifyNpcGift(Player player, int npcId, int itemId) => OnNpcGift?.Invoke(player, npcId, itemId);

        /// <summary>
        /// Helper to notify achievement conditions when a tile drops an item
        /// </summary>
        /// <param name="player">Player that likely broke the tile</param>
        /// <param name="id">Item ID that was dropped</param>
        public static void NotifyTileDrop(Player player, int id) => OnTileDrop?.Invoke(player, id);
    }
}
