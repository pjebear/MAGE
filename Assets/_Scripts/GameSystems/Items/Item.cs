using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems
{
    struct ItemTag
    {
        public ItemType ItemType;
        public int ItemId;
        public ItemTag(EquippableId id) : this(ItemType.Equippable, (int)id) { }
        public ItemTag(ConsumableId id) : this(ItemType.Consumable, (int)id) { }
        public ItemTag(StoryItemId id) : this(ItemType.Story, (int)id) { }
        public ItemTag(VendorItemId id) : this(ItemType.Vendor, (int)id) { }
        public ItemTag(BundleId id) : this(ItemType.Bundle, (int)id) { }
        public ItemTag(int itemId) : this(ItemUtil.TypeFromId(itemId), itemId) { }

        private ItemTag(ItemType type, int id)
        {
            ItemType = type;
            ItemId = id;
        }
    }

    abstract class Item
    {
        public ItemTag ItemTag;
        public UI.ItemIconSpriteId SpriteId;
        public string Name = "MISSING";
        public int Value = 0;

        protected Item(ItemTag tag, UI.ItemIconSpriteId spriteId, string name, int value)
        {
            ItemTag = tag;
            SpriteId = spriteId;
            Name = name;
            Value = value;
        }
    }

    class VendorItem : Item
    {
        public VendorItem(VendorItemId itemId, UI.ItemIconSpriteId spriteId, int vendorPrice)
            : base(new ItemTag(itemId), spriteId, itemId.ToString(), vendorPrice)
        {
            // empty
        }
    }

    class StoryItem : Item
    {
        public StoryItem(StoryItemId storyItemId, UI.ItemIconSpriteId spriteId)
            :base(new ItemTag(storyItemId), spriteId, storyItemId.ToString(), -1)
        {
            // empty
        }
    }
}

