using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems
{
    enum EquippableCategory
    {
        Armor,
        EmptyHandMelee,
        OneHandMelee,
        TwoHandMelee,
        Ranged,
        Shield,
        Accessory,

        NUM
    }

    enum ArmorType
    {
        Cloth,
        Leather,
        Chain,
        Plate,
        All
    }

    enum EmptyHandMeleeWeaponType
    {
        Fist,

        NUM
    }

    enum OneHandMeleeWeaponType
    {
        All = -1,

        Dagger,
        Sword,
        Axe,
        Mace,
    }

    enum TwoHandMeleeWeaponType
    {
        All = -1,

        BastardSword,
        BattleAxe,
        Maul,
        Staff,
    }

    enum RangedWeaponType
    {
        All = -1,

        Crossbow,
        Bow,
    }

    enum ShieldType
    {
        All = -1,

        Shield,
        TowerShield,

    }

    enum AccessoryType
    {
        StatBonus,

        NUM
    }

    enum EquippableId
    {
        INVALID = -1,

        // Armor
        ClothArmor_0 = ItemType.Equippable,
        LeatherArmor_0,
        ChainArmor_0,
        PlateArmor_0,

        // 1 handers
        Fists_0,
        Dagger_0,
        Sword_0,
        Axe_0,
        Mace_0,
        Crossbow_0,

        // 2 handers
        BastardSword_0,
        BattleAxe_0,
        Maul_0,
        Staff_0,
        MageSource_0,
        Bow_0,
        LongBow_0,

        // shields
        Shield_0,
        TowerShield_0,

        // Accessories
        Relic,
        Relic_1,

        END = Relic
    }

}
