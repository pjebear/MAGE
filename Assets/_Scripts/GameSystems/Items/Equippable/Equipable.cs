using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class EquippableTag
{
    public EquippableCategory Category;
    public int Type;

    public EquippableTag(ArmorType type) : this(EquippableCategory.Armor, (int)type) { }
    public EquippableTag(OneHandWeaponType type) : this(EquippableCategory.OneHandWeapon, (int)type) { }
    public EquippableTag(TwoHandWeaponType type) : this(EquippableCategory.TwoHandWeapon, (int)type) { }
    public EquippableTag(ShieldType type) : this(EquippableCategory.Shield, (int)type) { }
    public EquippableTag(AccessoryType type) : this(EquippableCategory.Accessory, (int)type) { }
    public EquippableTag(EquippableCategory category, int type) { Category = category; Type = type; }
}

class Equippable : Item
{
    public EquippableTag EquipmentTag;
    public EquippableId EquipmentId { get { return (EquippableId)ItemTag.ItemId; } }
    public AppearancePrefabId PrefabId;

    public List<AttributeModifier> EquipBonuses;

    public Equippable(EquippableId id, EquippableTag tag, AppearancePrefabId prefabId, ItemIconSpriteId spriteId, List<AttributeModifier> equipBonuses)
        : base(new ItemTag(id), spriteId)
    {
        EquipmentTag = tag;
        EquipBonuses = equipBonuses;
        PrefabId = prefabId;
    }
}

class HeldEquippable : Equippable
{
    public float BlockChance = 0;
    public float ParryChance = 0;
    public List<AttributeScalar> EffectivenessScalars = new List<AttributeScalar>();
    public int NumHandsRequired;

    public HeldEquippable(int numHandsRequired, float blockChance, float parryChance, List<AttributeScalar> effectivenessScalars, EquippableId id, EquippableTag tag, AppearancePrefabId prefabId, ItemIconSpriteId spriteId, List<AttributeModifier> modifiers) 
        : base(id, tag, prefabId, spriteId, modifiers)
    {
        BlockChance = blockChance;
        ParryChance = parryChance;
        EffectivenessScalars = effectivenessScalars;
        NumHandsRequired = numHandsRequired;
    }
}

