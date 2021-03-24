using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems
{
    enum ConsumableId
    {
        TODO = ItemType.Consumable,

        END
    }

    enum StoryItemId
    {
        GateKey = ItemType.Story,

        DEMO_GoldenBearPelt,

        END
    }

    enum VendorItemId
    {
        DEMO_BearPelt = ItemType.Vendor,
        DEMO_BearClaw,

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
        Vendor = StoryItemId.END + 1,
        Bundle = VendorItemId.END + 1
    }

    enum ItemId
    {
        NUM = BundleId.END
    }
}