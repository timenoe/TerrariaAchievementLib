﻿using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaAchievementLib.Achievements;
using TerrariaAchievementLib.Systems;
using TerrariaAchievementLib.Tools;

namespace TerrariaAchievementLib.Items
{
    /// <summary>
    /// Sends notifications to achievement conditions when an item is dropped
    /// </summary>
    public class AchievementItem : GlobalItem
    {
        /// <summary>
        /// True if the item has been synced over the network
        /// </summary>
        private bool _synced;

        /// <summary>
        /// Player interaction with the NPC that dropped an item as loot if applicable<br/>
        /// CloneByReference is to suppress an irrelevant TML warning
        /// </summary>
        [CloneByReference]
        private readonly bool[] _npcPlayerInteraction = new bool[256];

        /// <summary>
        /// Player nearest to the spawned item
        /// </summary>
        private byte _nearestPlayer;

        /// <summary>
        /// ID of the bag that the item came from if applicable
        /// </summary>
        private int _bag;

        /// <summary>
        /// ID of the NPC that the item came from if applicable
        /// </summary>
        private int _npc;

        /// <summary>
        /// Reason the item spawned
        /// </summary>
        private SpawnReason _spawnReason;


        /// <summary>
        /// Identifies the reason an item spawn
        /// </summary>
        public enum SpawnReason
        {
            None,
            TileBreak,
            ShakeTree,
            NpcDrop,
            BagOpen,
            NpcGift
        }


        public override bool InstancePerEntity => true;

        public override void Load()
        {
            if (Main.dedServ)
                return;
            
            On_Item.ChangeItemType += On_Item_ChangeItemType;
            On_Item.SetDefaults_int += On_Item_SetDefaults_int;
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            On_Item.ChangeItemType -= On_Item_ChangeItemType;
            On_Item.SetDefaults_int -= On_Item_SetDefaults_int;
        }

        // This allows for Magic Storage crafts to be recognized
        public override void OnCreated(Item item, ItemCreationContext context)
        {
            _nearestPlayer = Player.FindClosest(item.Center, 1, 1);

            if (context is RecipeItemCreationContext recipeContext && _nearestPlayer == Main.myPlayer)
            {
                MagicStorageConfig config = new();
                try
                {
                    string json = File.ReadAllText($"{Main.SavePath}/ModConfigs/MagicStorage_MagicStorageConfig.json");
                    config = JsonSerializer.Deserialize<MagicStorageConfig>(json);
                }
                catch { };

                if (config.RecursionCraftingDepth == 0)
                {
                    AchievementsHelper.NotifyItemCraft(recipeContext.Recipe);
                    AchievementsHelper.NotifyItemPickup(Main.LocalPlayer, item);
                }
                else
                    VanillaEventSystem.DisplayMagicStorageWarning();
            }
        }

        public override void NetSend(Item item, BinaryWriter writer)
        {
            if (!NetTool.MultiplayerServer())
                return;
            
            writer.Write(_nearestPlayer);
            writer.Write((Int32)_spawnReason);
            for (int i = 0; i < 256; i++)
                writer.Write(_npcPlayerInteraction[i]);
            writer.Write(_npc);
            writer.Write(_bag);
            writer.Write(_synced);
            _synced = true;
        }

        public override void NetReceive(Item item, BinaryReader reader)
        {
            if (!NetTool.MultiplayerClient())
                return;

            _nearestPlayer = reader.ReadByte();
            _spawnReason = (SpawnReason)reader.ReadInt32();
            for (int i = 0; i < 256; i++)
                _npcPlayerInteraction[i] = reader.ReadBoolean();
            _npc = reader.ReadInt32();
            _bag = reader.ReadInt32();
            _synced = reader.ReadBoolean();

            // Only send achievement notification once
            if (_synced)
                return;

            switch (_spawnReason)
            {
                case SpawnReason.TileBreak:
                    if (_nearestPlayer == Main.myPlayer)
                        CustomAchievementsHelper.NotifyTileDrop(Main.LocalPlayer, item.type);
                    break;

                case SpawnReason.ShakeTree:
                    if (_nearestPlayer == Main.myPlayer)
                        CustomAchievementsHelper.NotifyItemShake(Main.LocalPlayer, item.type);
                    break;

                case SpawnReason.NpcDrop:
                    if (_npcPlayerInteraction[Main.myPlayer])
                        CustomAchievementsHelper.NotifyNpcDrop(Main.LocalPlayer, _npc, item.type);
                    break;

                case SpawnReason.BagOpen:
                    if (_nearestPlayer == Main.myPlayer)
                        CustomAchievementsHelper.NotifyItemOpen(Main.LocalPlayer, _bag, item.type);
                    break;

                case SpawnReason.NpcGift:
                    if (_nearestPlayer == Main.myPlayer)
                        CustomAchievementsHelper.NotifyNpcGift(Main.LocalPlayer, _npc, item.type);
                    break;
            }
        }

        public override void OnSpawn(Item item, IEntitySource source)
        {
            _nearestPlayer = Player.FindClosest(item.Center, 1, 1);

            if (source is EntitySource_TileBreak)
            {
                _spawnReason = SpawnReason.TileBreak;

                if (NetTool.Singleplayer() && _nearestPlayer == Main.myPlayer)
                    CustomAchievementsHelper.NotifyTileDrop(Main.LocalPlayer, item.type);
            }

            else if (source is EntitySource_ShakeTree)
            {
                _spawnReason = SpawnReason.ShakeTree;

                if (NetTool.Singleplayer() && _nearestPlayer == Main.myPlayer)
                    CustomAchievementsHelper.NotifyItemShake(Main.LocalPlayer, item.type);
            }

            else if (source is EntitySource_Loot loot && loot.Entity is NPC deadNpc)
            {
                _spawnReason = SpawnReason.NpcDrop;

                _npc = deadNpc.type;
                Array.Copy(deadNpc.playerInteraction, _npcPlayerInteraction, 256);

                if (NetTool.Singleplayer() && _npcPlayerInteraction[Main.myPlayer])
                    CustomAchievementsHelper.NotifyNpcDrop(Main.LocalPlayer, _npc, item.type);
            }

            else if (source is EntitySource_ItemOpen openedBag)
            {
                _spawnReason = SpawnReason.BagOpen;

                _bag = openedBag.ItemType;

                if (NetTool.Singleplayer() && _nearestPlayer == Main.myPlayer)
                    CustomAchievementsHelper.NotifyItemOpen(Main.LocalPlayer, _bag, item.type);
            }

            else if (source is EntitySource_Gift receivedGift)
            {
                _spawnReason = SpawnReason.NpcGift;

                _npc = NPCID.None;
                if (receivedGift.Entity is NPC giftNpc)
                    _npc = giftNpc.type;

                if (NetTool.Singleplayer() && _nearestPlayer == Main.myPlayer)
                    CustomAchievementsHelper.NotifyNpcGift(Main.LocalPlayer, _npc, item.type);
            }
        }

        /// <summary>
        /// Detour to notify achievement conditions when an item type is changed (right-clicked to activate, etc.)
        /// </summary>
        /// <param name="orig">Original ChangeItemType method</param>
        /// <param name="self">Item that is changing</param>
        /// <param name="to">Item to change to</param>
        private void On_Item_ChangeItemType(On_Item.orig_ChangeItemType orig, Item self, int to)
        {
            orig.Invoke(self, to);

            CustomAchievementsHelper.NotifyItemGrab(Main.LocalPlayer, (short)to, 1);
        }

        /// <summary>
        /// Detour to notify achievement conditions when an item has its defaults set (Wilson beard grows, etc.)
        /// </summary>
        /// <param name="orig">Original SetDefaults method</param>
        /// <param name="self">Item that had its defaults set</param>
        /// <param name="Type">Type to set the item to</param>
        private void On_Item_SetDefaults_int(On_Item.orig_SetDefaults_int orig, Item self, int Type)
        {
            orig.Invoke(self, Type);

            // Prevent items from counting during Main.CreateRecipes() during initialization
            if (Main.gameMenu)
                return;

            if (Type == ItemID.WilsonBeardLong || Type == ItemID.WilsonBeardMagnificent)
                CustomAchievementsHelper.NotifyItemGrab(Main.LocalPlayer, (short)Type, 1);
        }

        /// <summary>
        /// Used to deserialize the Magic Storage config file
        /// </summary>
        private class MagicStorageConfig
        {
            [JsonPropertyName("recursionCraftingDepth")]
            public int RecursionCraftingDepth { get; set; }
        }
    }
}
