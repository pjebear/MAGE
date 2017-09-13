using Common.EquipmentEnums;
using Common.EquipmentTypes;
using Common.EquipmentUtil;
using Screens.Payloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorldSystem.Items;

namespace WorldSystem.InventorySystem
{
    class Inventory
    {
        private Dictionary<KeyValuePair<EquipmentCategory, int>, KeyValuePair<EquipmentBase, int>> mEquipableInventory; // key = equipmentType, equipmentIndex | value = equipment, stock

        public Inventory()
        {
            mEquipableInventory = new Dictionary<KeyValuePair<EquipmentCategory, int>, KeyValuePair<EquipmentBase, int>>();
        }

        public void AddItem(ItemBase item)
        {
            if (item.ItemType == ItemType.Equippable)
            {
                AddEquipment(item as EquipmentBase);
            }
            else
            {
                UnityEngine.Debug.Log("No Inventory system set up for " + item.ItemType.ToString());
            }
        }

        private void AddEquipment(EquipmentBase toAdd)
        {
            UnityEngine.Debug.Log("Inventory.AddEquipment() " + toAdd.Index.Key.ToString() + "," + toAdd.Index.Value);
            if (!mEquipableInventory.ContainsKey(toAdd.Index))
            {
                mEquipableInventory.Add(toAdd.Index, new KeyValuePair<EquipmentBase, int>(toAdd, 0));
            }
            KeyValuePair<EquipmentBase, int> stock = mEquipableInventory[toAdd.Index];
            mEquipableInventory[toAdd.Index] = new KeyValuePair<EquipmentBase, int>(stock.Key, stock.Value + 1);
        }

        public EquipmentBase CheckoutEquipment(KeyValuePair<EquipmentCategory, int> equipmentIndex)
        {
            UnityEngine.Debug.Log("Inventory.CheckoutEquipment() " + equipmentIndex.Key.ToString() + "," + equipmentIndex);
            EquipmentBase toReturn = null;
            if (mEquipableInventory.ContainsKey(equipmentIndex))
            {
                KeyValuePair<EquipmentBase, int> stock = mEquipableInventory[equipmentIndex];
                if (stock.Value == 1)
                {
                    mEquipableInventory.Remove(equipmentIndex);
                }
                else
                {
                    mEquipableInventory[equipmentIndex] = new KeyValuePair<EquipmentBase, int>(stock.Key, stock.Value - 1);
                }
                toReturn = stock.Key;
            }
            else
            {
                UnityEngine.Debug.Log("Attempting to checkout item not in inventory");
            }
            return toReturn;
        }

        public List<InventoryEquipmentPayload> GetEquipmentPayloads()
        {
            List<InventoryEquipmentPayload> toReturn = new List<InventoryEquipmentPayload>();
            foreach (var inStock in mEquipableInventory)
            {
                toReturn.Add(
                    new InventoryEquipmentPayload()
                    {
                        EquipmentIndex = new KeyValuePair<int, int>((int)inStock.Key.Key, inStock.Key.Value),
                        EquipmentName = inStock.Value.Key.Name,
                        IconAssetPath = inStock.Value.Key.InventoryAssetId,
                        Count = inStock.Value.Value,
                    }
                );
            }
            return toReturn;
        }

        public List<InventoryEquipmentPayload> GetEquipmentPayloadsForSlot(EquipmentProficienciesContainer proficiencies, WornEquipmentSlot slot)
        {
            List<InventoryEquipmentPayload> toReturn = new List<InventoryEquipmentPayload>();
            foreach (var equipment in mEquipableInventory)
            {
                if (EquipmentUtil.IsValidForSlot(equipment.Key.Key, slot) && proficiencies.CanEquip(equipment.Value.Key))
                {
                    toReturn.Add(new InventoryEquipmentPayload()
                    {
                        EquipmentName = equipment.Value.Key.Name,
                        Count = equipment.Value.Value,
                        EquipmentIndex = new KeyValuePair<int, int>((int)equipment.Key.Key, equipment.Key.Value)
                    });
                }
            }
            return toReturn;
        }
    }
}
