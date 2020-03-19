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
        EquipableType type = EquipableType.NUM;

        switch (equippableId)
        {
            case EquippableId.Sword_0:
                appearance[AppearanceType.Prefab] = (int)AppearancePrefabId.Sword_0;
                type = EquipableType.OneHandWeapon;
                break;

            case EquippableId.Shield_0:
                appearance[AppearanceType.Prefab] = (int)AppearancePrefabId.Shield_0;
                type = EquipableType.Shield;
                break;

            case EquippableId.Staff_0:
                appearance[AppearanceType.Prefab] = (int)AppearancePrefabId.Staff_0;
                type = EquipableType.TwoHandWeapon;
                break;

            case EquippableId.ChainArmor_0:
                appearance[AppearanceType.Prefab] = (int)AppearancePrefabId.Chain_0;
                type = EquipableType.Armor;
                break;

            case EquippableId.ClothArmor_0:
                appearance[AppearanceType.Prefab] = (int)AppearancePrefabId.Cloth_0;
                type = EquipableType.Armor;
                break;

            default:
                Debug.Assert(false);
                break;
        }

        return new Equippable(equippableId, type, appearance);
    }
}

