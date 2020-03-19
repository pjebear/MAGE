using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

enum ConsumableId
{
    TODO = ItemType.Consumable,

    END
}

enum StoryItemId
{
    Story_FirstQuestItem = ItemType.Story,

    END
}

enum BundleId
{
    TODO = ItemType.Bundle,

    END
}

enum ItemType
{
    Equippable = 0,
    Consumable = EquippableId.END + 1,
    Story = ConsumableId.END + 1,
    Bundle = StoryItemId.END + 1
}

enum ItemId
{
    NUM = BundleId.END
}