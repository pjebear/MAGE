using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.UI
{
    enum StatusIconSpriteId
    {
        INVALID = -1,

        Avenger,
        Disarm,
        Protection,
        BloodScent,
        Poison,
        RighteousGlory,

        NUM
    }

    enum ItemIconSpriteId
    {
        INVALID = -1,

        // Equipment
        // Armor
        Cloth,
        Leather,
        Chain,
        Plate,
        // Held
        // One Hand
        Dagger,
        Sword,
        Axe,
        Mace,
        // two hand
        Staff,
        Bow,

        // Shield
        Shield,

        // Accessory
        Relic,

        // Story
        QuestItem,

        // Vendor
        Fur,
        GoldenFur,
        Claws,

        NUM
    }

}
