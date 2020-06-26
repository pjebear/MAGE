using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

static class ItemFactory
{
    public static Equippable LoadEquipable(EquippableId id)
    {
        //Equippable equippable = new Equippable();
        DB.DBEquipment dbEquipment = DB.DBHelper.LoadEquipment((int)id);

        EquippableTag tag = new EquippableTag((EquippableCategory)dbEquipment.Category, dbEquipment.Type);

        // Appearance
        AppearancePrefabId appearancePrefabId = (AppearancePrefabId)dbEquipment.PrefabId;
        ItemIconSpriteId spriteId = (ItemIconSpriteId)dbEquipment.SpriteId;
        
        // Equip Bonuses
        List<AttributeModifier> equipBonuses = new List<AttributeModifier>();
        foreach (DB.DBAttributeModifier dbModifier in dbEquipment.EquipBonuses)
        {
            equipBonuses.Add(new AttributeModifier(
                new AttributeIndex((AttributeCategory)dbModifier.AttributeCategory, dbModifier.AttributeId)
                , (ModifierType)dbModifier.ModifierType
                , dbModifier.Modifier));
        }

        // Proficiency Modifiers
        List<AttributeScalar> proficiencyBonuses = new List<AttributeScalar>();
        foreach (DB.DBAttributeScalar attributeScalar in dbEquipment.EffectivenessScalars)
        {
            proficiencyBonuses.Add(new AttributeScalar(
                new AttributeIndex((AttributeCategory)attributeScalar.AttributeCategory, attributeScalar.AttributeId)
                , attributeScalar.Scalar));
        }

        EquippableCategory category = (EquippableCategory)dbEquipment.Category;

        Equippable equippable = null;

        switch (category)
        {
            case EquippableCategory.Accessory:
            case EquippableCategory.Armor:
                equippable = new Equippable(id, tag, appearancePrefabId, spriteId, equipBonuses);
                break;

            case EquippableCategory.OneHandWeapon:
            case EquippableCategory.TwoHandWeapon:
            case EquippableCategory.Shield:
            {
                int numHandsRequired = category == EquippableCategory.TwoHandWeapon ? 2 : 1;
                AnimationId animationId = (AnimationId)dbEquipment.AnimationId;
                ProjectileId projectileId = (ProjectileId)dbEquipment.ProjectileId;

                if (category == EquippableCategory.Shield)
                {
                    equippable = new HeldEquippable(numHandsRequired, dbEquipment.BlockChance, dbEquipment.ParryChance, proficiencyBonuses, id, tag, appearancePrefabId, spriteId, equipBonuses);
                }
                else
                {
                    RangeInfo range = new RangeInfo(dbEquipment.Range.Min, dbEquipment.Range.Max, dbEquipment.Range.Elevation, (AreaType)dbEquipment.Range.Type);
                    equippable = new WeaponEquippable(animationId, projectileId, range, numHandsRequired, dbEquipment.BlockChance, dbEquipment.ParryChance, proficiencyBonuses, id, tag, appearancePrefabId, spriteId, equipBonuses);
                }
            }
            break;

            default:
                Debug.Assert(false);
                break;
        }

        return equippable;
    }

    //static Equippable CreateEquipable(ItemId itemId)
    //{
    //    Appearance appearance = new Appearance();

    //    Debug.Assert(ItemUtil.TypeFromId((int)itemId) == ItemType.Equippable);
    //    EquippableId equippableId = (EquippableId)itemId;
    //    EquippableTag tag = null;

    //    switch (equippableId)
    //    {
    //        case EquippableId.Sword_0:
    //            appearance[AppearanceType.Prefab] = (int)AppearancePrefabId.Sword_0;
    //            tag = new EquippableTag(OneHandWeaponType.Sword);
    //            break;

    //        case EquippableId.Axe_0:
    //            appearance[AppearanceType.Prefab] = (int)AppearancePrefabId.Axe_0;
    //            tag = new EquippableTag(OneHandWeaponType.Axe);
    //            break;

    //        case EquippableId.Mace_0:
    //            appearance[AppearanceType.Prefab] = (int)AppearancePrefabId.Mace_0;
    //            tag = new EquippableTag(OneHandWeaponType.Mace);
    //            break;

    //        case EquippableId.Shield_0:
    //            appearance[AppearanceType.Prefab] = (int)AppearancePrefabId.Shield_0;
    //            tag = new EquippableTag(ShieldType.Shield);
    //            break;

    //        case EquippableId.Staff_0:
    //            appearance[AppearanceType.Prefab] = (int)AppearancePrefabId.Staff_0;
    //            tag = new EquippableTag(TwoHandWeaponType.Staff);
    //            break;

    //        case EquippableId.ChainArmor_0:
    //            appearance[AppearanceType.Prefab] = (int)AppearancePrefabId.Chain_0;
    //            tag = new EquippableTag(ArmorType.Chain);
    //            break;

    //        case EquippableId.LeatherArmor_0:
    //            appearance[AppearanceType.Prefab] = (int)AppearancePrefabId.Leather_0;
    //            tag = new EquippableTag(ArmorType.Leather);
    //            break;

    //        case EquippableId.ClothArmor_0:
    //            appearance[AppearanceType.Prefab] = (int)AppearancePrefabId.Cloth_0;
    //            tag = new EquippableTag(ArmorType.Cloth);
    //            break;

    //        case EquippableId.Relic:
    //            appearance[AppearanceType.Prefab] = (int)AppearancePrefabId.Cloth_0;
    //            tag = new EquippableTag(ArmorType.Cloth);
    //            break;

    //        default:
    //            Debug.Assert(false);
    //            break;
    //    }

    //    return new Equippable(equippableId, tag, appearance, new List<AttributeModifier>());
    //}
}

