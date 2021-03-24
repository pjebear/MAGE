using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameSystems
{
    static class ItemUtil
    {
        public static ItemType TypeFromId(int itemId)
        {
            ItemType type = ItemType.Equippable;

            if (itemId >= (int)ItemType.Bundle)
            {
                type = ItemType.Bundle;
            }
            else if (itemId >= (int)ItemType.Vendor)
            {
                type = ItemType.Vendor;
            }
            else if (itemId >= (int)ItemType.Story)
            {
                type = ItemType.Story;
            }
            else if (itemId >= (int)ItemType.Consumable)
            {
                type = ItemType.Consumable;
            }

            return type;
        }

        public static string ToString(int itemId)
        {
            ItemType type = TypeFromId(itemId);
            switch (type)
            {
                case (ItemType.Equippable):
                    return ((EquippableId)itemId).ToString();
                case (ItemType.Story):
                    return ((StoryItemId)itemId).ToString();
                case (ItemType.Consumable):
                    return ((ConsumableId)itemId).ToString();
                case (ItemType.Bundle):
                    return ((BundleId)itemId).ToString();
                case (ItemType.Vendor):
                    return ((VendorItemId)itemId).ToString();
            }
            Debug.LogWarning("");
            return "INVALID";
        }
    }
}
