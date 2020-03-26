using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

static class ItemFactory
{
    public static Equippable CreateEquipable(ItemId itemId)
    {
        Appearance appearance = new Appearance();

        Debug.Assert(ItemUtil.TypeFromId((int)itemId) == ItemType.Equippable);
        EquippableId equippableId = (EquippableId)itemId;
        EquipableTag tag = null;

        switch (equippableId)
        {
            case EquippableId.Sword_0:
                appearance[AppearanceType.Prefab] = (int)AppearancePrefabId.Sword_0;
                tag = new EquipableTag(OneHandWeaponType.Sword);
                break;

            case EquippableId.Shield_0:
                appearance[AppearanceType.Prefab] = (int)AppearancePrefabId.Shield_0;
                tag = new EquipableTag(ShieldType.Shield);
                break;

            case EquippableId.Staff_0:
                appearance[AppearanceType.Prefab] = (int)AppearancePrefabId.Staff_0;
                tag = new EquipableTag(TwoHandWeaponType.Staff);
                break;

            case EquippableId.ChainArmor_0:
                appearance[AppearanceType.Prefab] = (int)AppearancePrefabId.Chain_0;
                tag = new EquipableTag(ArmorType.Chain);
                break;

            case EquippableId.ClothArmor_0:
                appearance[AppearanceType.Prefab] = (int)AppearancePrefabId.Cloth_0;
                tag = new EquipableTag(ArmorType.Cloth);
                break;

            default:
                Debug.Assert(false);
                break;
        }

        return new Equippable(equippableId, tag, appearance);
    }
}

