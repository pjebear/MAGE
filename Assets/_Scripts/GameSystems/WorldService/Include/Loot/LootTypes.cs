﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameSystems.Loot
{
    class LootTable
    {
        public Dictionary<LevelId, TableEntry> LevelLoot = new Dictionary<LevelId, TableEntry>();
        public Dictionary<Mobs.MobId, TableEntry> MobLoot = new Dictionary<Mobs.MobId, TableEntry>();

        public LootTable()
        {
            DebugInit();
        }

        public ClaimLootInfo CheckoutLoot(ClaimLootParams claimLootParams)
        {
            ClaimLootInfo lootResult = new ClaimLootInfo();

            // Temp
            AddCurrencyToLoot(500, ref lootResult);

            AddCurrencyToLoot(claimLootParams.Coins, ref lootResult);
            foreach (ItemId item in claimLootParams.Items)
            {
                AddItemToLoot((int)item, 1, ref lootResult);
            }

            if (claimLootParams.LevelId != LevelId.INVALID)
            {

            }

            foreach (Mobs.MobId mobId in claimLootParams.Mobs)
            {
                if (MobLoot.ContainsKey(mobId))
                {
                    TableEntry tableEntry = MobLoot[mobId];

                    foreach (LootInfo lootInfo in tableEntry.Loot)
                    {
                        AddLootToResult(lootInfo, ref lootResult);
                    }
                }
            }

            return lootResult;
        }

        private void AddLootToResult(LootInfo lootInfo, ref ClaimLootInfo out_result)
        {
            int numGranted = GetNumGranted(lootInfo);

            if (numGranted > 0)
            {
                switch (lootInfo.LootType)
                {
                    case LootType.Currency:
                    {
                        AddCurrencyToLoot(numGranted, ref out_result);
                    }
                    break;
                    case LootType.Item:
                    {
                        AddItemToLoot(lootInfo.Value, numGranted, ref out_result);
                    }
                    break;
                }
            }
        }

        private void AddCurrencyToLoot(int currency, ref ClaimLootInfo out_result)
        {
            out_result.Currency += currency;
        }

        private void AddItemToLoot(int itemId, int count, ref ClaimLootInfo out_result)
        {
            if (!out_result.Items.ContainsKey(itemId))
                out_result.Items.Add(itemId, count);
            else
                out_result.Items[itemId] += count;
        }

        private int GetNumGranted(LootInfo info)
        {
            int numGranted = 0;

            if (info.DropChance >= 100
                || UnityEngine.Random.Range(0, 100) < info.DropChance)
            {
                numGranted = info.DropAmount + UnityEngine.Random.Range(0, info.DropVarience + 1);
            }

            return numGranted;
        }

        private void DebugInit()
        {
            { // Bandit
                TableEntry entry = new TableEntry();
                { // currency
                    LootInfo lootInfo = new LootInfo();
                    lootInfo.LootType = LootType.Currency;
                    lootInfo.Value = 50;
                    lootInfo.DropAmount = 45;
                    lootInfo.DropVarience = 10;
                    lootInfo.DropChance = 100;

                    entry.Loot.Add(lootInfo);
                }

                { // Sword
                    LootInfo lootInfo = new LootInfo();
                    lootInfo.LootType = LootType.Item;
                    lootInfo.Value = (int)EquippableId.Sword_0;
                    lootInfo.DropAmount = 1;
                    lootInfo.DropVarience = 0;
                    lootInfo.DropChance = 50;

                    entry.Loot.Add(lootInfo);
                }

                { // Bow
                    LootInfo lootInfo = new LootInfo();
                    lootInfo.LootType = LootType.Item;  
                    lootInfo.Value = (int)EquippableId.LongBow_0;
                    lootInfo.DropAmount = 1;
                    lootInfo.DropVarience = 0;
                    lootInfo.DropChance = 50;

                    entry.Loot.Add(lootInfo);
                }
                MobLoot.Add(Mobs.MobId.DEMO_Bandit, entry);
            }

            { // Bear
                TableEntry entry = new TableEntry();
                { // currency
                    LootInfo lootInfo = new LootInfo();
                    lootInfo.LootType = LootType.Currency;
                    lootInfo.Value = 50;
                    lootInfo.DropAmount = 45;
                    lootInfo.DropVarience = 10;
                    lootInfo.DropChance = 100;

                    entry.Loot.Add(lootInfo);
                }

                { // Bear Claw
                    LootInfo lootInfo = new LootInfo();
                    lootInfo.LootType = LootType.Item;
                    lootInfo.Value = (int)VendorItemId.DEMO_BearClaw;
                    lootInfo.DropAmount = 8;
                    lootInfo.DropVarience = 4;
                    lootInfo.DropChance = 100;

                    entry.Loot.Add(lootInfo);
                }

                { // Bear Pelt
                    LootInfo lootInfo = new LootInfo();
                    lootInfo.LootType = LootType.Item;
                    lootInfo.Value = (int)VendorItemId.DEMO_BearPelt;
                    lootInfo.DropAmount = 1;
                    lootInfo.DropVarience = 0;
                    lootInfo.DropChance = 100;

                    entry.Loot.Add(lootInfo);
                }
                MobLoot.Add(Mobs.MobId.DEMO_Bear, entry);
            }
        }
    }

    class TableEntry
    {
        public List<LootInfo> Loot = new List<LootInfo>();
    }

    class LootInfo
    {
        public LootType LootType;
        public int Value = 0;

        public int DropAmount = 0;
        public int DropVarience = 0;

        public int DropChance = 100;
    }

    enum LootType
    {
        Item,
        Currency,
        NUM
    }

    class ClaimLootParams
    {
        public LevelId LevelId = LevelId.INVALID;
        public int Coins = 0;
        public List<ItemId> Items = new List<ItemId>();
        public List<Mobs.MobId> Mobs = new List<Mobs.MobId>();
    }

    class ClaimLootInfo
    {
        public int Currency = -1;
        public Dictionary<int, int> Items = new Dictionary<int, int>();
    }

}
