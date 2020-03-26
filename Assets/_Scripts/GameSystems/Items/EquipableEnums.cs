using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

enum EquipableCategory
{
    Armor,
    OneHandWeapon,
    TwoHandWeapon,
    Shield,
    Accessory,

    NUM
}

enum ArmorType
{
    Cloth,
    Leather,
    Chain,
    Plate
}

enum OneHandWeaponType
{
    Dagger,
    Sword,
    Axe,
    Mace,
    Crossbow
}

enum TwoHandWeaponType
{
    BastardSword,
    BattleAxe,
    Maul,
    Staff,
    Bow
}

enum ShieldType
{
    Shield,
    TowerShield
}

enum AccessoryType
{

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
    END = TowerShield_0
}
