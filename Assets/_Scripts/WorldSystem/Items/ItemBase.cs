using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldSystem.Items
{
    public enum ItemType
    {
        Equippable,
        Consumable,
        Story
    }

    public abstract class ItemBase
    {
        public ItemType ItemType { get; private set; }
        public string Name { get; private set; }
        public string InventoryAssetId { get; private set; }

        protected ItemBase (string name, ItemType type, string inventoryAssetId)
        {
            Name = name;
            ItemType = type;
            InventoryAssetId = inventoryAssetId;
        }
    }
}