using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class EquipableTag
{
    public EquipableCategory Category;
    public int Type;

    public EquipableTag(ArmorType type) : this(EquipableCategory.Armor, (int)type) { }
    public EquipableTag(OneHandWeaponType type) : this(EquipableCategory.OneHandWeapon, (int)type) { }
    public EquipableTag(TwoHandWeaponType type) : this(EquipableCategory.TwoHandWeapon, (int)type) { }
    public EquipableTag(ShieldType type) : this(EquipableCategory.Shield, (int)type) { }
    public EquipableTag(AccessoryType type) : this(EquipableCategory.Accessory, (int)type) { }
    public EquipableTag(EquipableCategory category, int type) { Category = category; Type = type; }
}

class Equippable : Item
{
    public EquipableTag EquipmentTag;
    public EquippableId EquipmentId { get { return (EquippableId)ItemTag.ItemId; } }

    public Equippable(EquippableId id, EquipableTag tag, Appearance appearance)
        : base(new ItemTag(id), appearance)
    {
        EquipmentTag = tag;
    }
}

