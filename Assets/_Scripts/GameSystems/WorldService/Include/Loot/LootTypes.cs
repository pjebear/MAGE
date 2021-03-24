using System;
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
        public Dictionary<EncounterScenarioId, TableEntry> EncounterLoot = new Dictionary<EncounterScenarioId, TableEntry>();
        public Dictionary<Mobs.MobId, TableEntry> MobLoot = new Dictionary<Mobs.MobId, TableEntry>();

        public LootTable()
        {
            DebugInit();
        }

        public ClaimLootInfo CheckoutLoot(ClaimLootParams claimLootParams)
        {
            ClaimLootInfo lootResult = new ClaimLootInfo();
            if (claimLootParams.LevelId != LevelId.INVALID)
            {

            }

            if (claimLootParams.EncounterId != EncounterScenarioId.INVALID 
                || claimLootParams.EncounterId != EncounterScenarioId.Random)
            {

            }

            foreach (Mobs.MobId mobId in claimLootParams.Mobs)
            {
                Debug.Assert(MobLoot.ContainsKey(mobId));
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
                        out_result.Currency += numGranted;
                    }
                    break;
                    case LootType.Item:
                    {
                        int itemId = lootInfo.Value;

                        if (!out_result.Items.ContainsKey(itemId))
                            out_result.Items.Add(itemId, numGranted);
                        else
                            out_result.Items[itemId] += numGranted;
                    }
                    break;
                }
            }
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
            { // Bear
                TableEntry bearEntry = new TableEntry();
                { // currency
                    LootInfo lootInfo = new LootInfo();
                    lootInfo.LootType = LootType.Currency;
                    lootInfo.Value = 50;
                    lootInfo.DropAmount = 45;
                    lootInfo.DropVarience = 10;
                    lootInfo.DropChance = 100;

                    bearEntry.Loot.Add(lootInfo);
                }

                { // Bear Claw
                    LootInfo lootInfo = new LootInfo();
                    lootInfo.LootType = LootType.Item;
                    lootInfo.Value = (int)VendorItemId.DEMO_BearClaw;
                    lootInfo.DropAmount = 8;
                    lootInfo.DropVarience = 4;
                    lootInfo.DropChance = 100;

                    bearEntry.Loot.Add(lootInfo);
                }

                { // Bear Pelt
                    LootInfo lootInfo = new LootInfo();
                    lootInfo.LootType = LootType.Item;
                    lootInfo.Value = (int)VendorItemId.DEMO_BearPelt;
                    lootInfo.DropAmount = 1;
                    lootInfo.DropVarience = 0;
                    lootInfo.DropChance = 100;

                    bearEntry.Loot.Add(lootInfo);
                }
                MobLoot.Add(Mobs.MobId.DEMO_Bear, bearEntry);
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
        public EncounterScenarioId EncounterId = EncounterScenarioId.INVALID;
        public List<Mobs.MobId> Mobs = new List<Mobs.MobId>();
    }

    class ClaimLootInfo
    {
        public int Currency = -1;
        public Dictionary<int, int> Items = new Dictionary<int, int>();
    }

}
