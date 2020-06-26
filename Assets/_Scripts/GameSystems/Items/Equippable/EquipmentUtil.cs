using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


static class EquipmentUtil
{ 
    public static bool HasProficiencyFor(Specialization specialization, Equippable equippable)
    {
        bool hasProficiency 
            = specialization.Proficiencies.Contains(EquipableTagToProficiency(equippable.EquipmentTag.Category, equippable.EquipmentTag.Type))
            || specialization.Proficiencies.Contains(EquipmentCategoryToProficiency(equippable.EquipmentTag.Category));

        return hasProficiency;
    }

    public static ProficiencyType EquipmentCategoryToProficiency(EquippableCategory category)
    {
        ProficiencyType proficiencyType = ProficiencyType.INVALID;

        switch (category)
        {
            case EquippableCategory.Armor:           proficiencyType = ProficiencyType.Armors;       break;
            case EquippableCategory.OneHandWeapon:   proficiencyType = ProficiencyType.OneHands;     break;
            case EquippableCategory.TwoHandWeapon:   proficiencyType = ProficiencyType.TwoHands;     break;
            case EquippableCategory.Shield:          proficiencyType = ProficiencyType.Sheilds;      break;
            case EquippableCategory.Accessory:       proficiencyType = ProficiencyType.Accessorys;   break;
        }

        Logger.Assert(proficiencyType != ProficiencyType.INVALID, LogTag.Character, "EquipmentUtil",
            string.Format("::EquipmentCategoryToProficiency() - Failed to find proficiency for equipable. Category [{0}]", category.ToString()), LogLevel.Warning);

        return proficiencyType;
    }

    public static bool IsHeld(Equipment.Slot slot)
    {
        return (slot == Equipment.Slot.LeftHand || slot == Equipment.Slot.RightHand);
    }

    public static bool FitsInSlot(EquippableCategory category, Equipment.Slot slot)
    {
        bool fits = false;

        switch (category)
        {
            case EquippableCategory.Accessory:
                fits = slot == Equipment.Slot.Accessory;
                break;

            case EquippableCategory.Armor:
                fits = slot == Equipment.Slot.Armor;
                break;

            case EquippableCategory.Shield:
            case EquippableCategory.TwoHandWeapon:
            case EquippableCategory.OneHandWeapon:
                fits = slot == Equipment.Slot.LeftHand || slot == Equipment.Slot.RightHand;
                break;
        }

        return fits;
    }

    public static bool IsSlotEmpty(Equipment equipment, Equipment.Slot slot)
    {
        return equipment[slot] == null || IsHeld(slot) && equipment[slot].EquipmentId == EquippableId.Fists_0;
    }

    public static ProficiencyType EquipableTagToProficiency(EquippableCategory category, int equipableType)
    {
        ProficiencyType proficiencyType = ProficiencyType.INVALID;

        switch (category)
        {
            case EquippableCategory.Armor:
            {
                switch ((ArmorType)equipableType)
                {
                    case ArmorType.Cloth: proficiencyType = ProficiencyType.Cloth; break;
                    case ArmorType.Leather: proficiencyType = ProficiencyType.Leather; break;
                    case ArmorType.Chain: proficiencyType = ProficiencyType.Chain; break;
                    case ArmorType.Plate: proficiencyType = ProficiencyType.Plate; break;
                }
            }
            break;

            case EquippableCategory.OneHandWeapon:
            {
                switch ((OneHandWeaponType)equipableType)
                {
                    case OneHandWeaponType.Fist:        proficiencyType = ProficiencyType.Fists;    break;
                    case OneHandWeaponType.Axe:         proficiencyType = ProficiencyType.Axe;      break;
                    case OneHandWeaponType.Sword:       proficiencyType = ProficiencyType.Sword;    break;
                    case OneHandWeaponType.Mace:        proficiencyType = ProficiencyType.Hammer;   break;
                    case OneHandWeaponType.Dagger:      proficiencyType = ProficiencyType.Dagger;   break;
                    case OneHandWeaponType.Crossbow:    proficiencyType = ProficiencyType.Crossbow; break;
                }
            }
            break;

            case EquippableCategory.TwoHandWeapon:
            {
                switch ((TwoHandWeaponType)equipableType)
                {
                    case TwoHandWeaponType.BattleAxe:       proficiencyType = ProficiencyType.BattleAxe;    break;
                    case TwoHandWeaponType.BastardSword:    proficiencyType = ProficiencyType.BastardSword; break;
                    case TwoHandWeaponType.Maul:            proficiencyType = ProficiencyType.Maul;         break;
                    case TwoHandWeaponType.Bow:             proficiencyType = ProficiencyType.Bow;          break;
                    case TwoHandWeaponType.Staff:           proficiencyType = ProficiencyType.Staff;        break;
                }
            }
            break;

            case EquippableCategory.Shield:
            {
                switch ((ShieldType)equipableType)
                {
                    case ShieldType.Shield:         proficiencyType = ProficiencyType.Sheild;       break;
                    case ShieldType.TowerShield:    proficiencyType = ProficiencyType.TowerShield;  break;
                }
            }
            break;

            case EquippableCategory.Accessory:
            {
                switch ((AccessoryType)equipableType)
                {
                    default: proficiencyType = ProficiencyType.Accessorys; break;
                }
            }
            break;
        }

        Logger.Assert(proficiencyType != ProficiencyType.INVALID, LogTag.Character, "EquipmentUtil",
            string.Format("::EquipableTagToProficiency() - Failed to find proficiency for equipable. Category [{0}] Type[{1}]", category.ToString(), equipableType.ToString()), LogLevel.Warning);

        return proficiencyType;
    }

    public static float GetHeldEquippableEffectiveness(EncounterCharacter caster, HeldEquippable held)
    {
        float effectiveness = 0;

        foreach (AttributeScalar scalar in held.EffectivenessScalars)
        {
            effectiveness += caster.Attributes[scalar.AttributeIndex] * scalar.Scalar;
        }

        return effectiveness;
    }
}

