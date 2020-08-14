using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameServices
{
    struct ItemTag
    {
        public ItemType ItemType;
        public int ItemId;
        public ItemTag(EquippableId id) : this(ItemType.Equippable, (int)id) { }
        public ItemTag(ConsumableId id) : this(ItemType.Consumable, (int)id) { }
        public ItemTag(StoryItemId id) : this(ItemType.Story, (int)id) { }
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

        protected Item(ItemTag tag, UI.ItemIconSpriteId spriteId, string name)
        {
            ItemTag = tag;
            SpriteId = spriteId;
            Name = name;
        }
    }

    class StoryItem : Item
    {
        public StoryItem(StoryItemId storyItemId, UI.ItemIconSpriteId spriteId)
            :base(new ItemTag(storyItemId), spriteId, storyItemId.ToString())
        {
            // empty
        }
    }
}

