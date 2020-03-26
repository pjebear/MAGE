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

    public static ProficiencyType EquipmentCategoryToProficiency(EquipableCategory category)
    {
        ProficiencyType proficiencyType = ProficiencyType.INVALID;

        switch (category)
        {
            case EquipableCategory.Armor:           proficiencyType = ProficiencyType.Armors;       break;
            case EquipableCategory.OneHandWeapon:   proficiencyType = ProficiencyType.OneHands;     break;
            case EquipableCategory.TwoHandWeapon:   proficiencyType = ProficiencyType.TwoHands;     break;
            case EquipableCategory.Shield:          proficiencyType = ProficiencyType.Sheilds;      break;
            case EquipableCategory.Accessory:       proficiencyType = ProficiencyType.Accessorys;   break;
        }

        Logger.Assert(proficiencyType != ProficiencyType.INVALID, LogTag.Character, "EquipmentUtil",
            string.Format("::EquipmentCategoryToProficiency() - Failed to find proficiency for equipable. Category [{0}]", category.ToString()), LogLevel.Warning);

        return proficiencyType;
    }

    public static ProficiencyType EquipableTagToProficiency(EquipableCategory category, int equipableType)
    {
        ProficiencyType proficiencyType = ProficiencyType.INVALID;

        switch (category)
        {
            case EquipableCategory.Armor:
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

            case EquipableCategory.OneHandWeapon:
            {
                switch ((OneHandWeaponType)equipableType)
                {
                    case OneHandWeaponType.Axe:         proficiencyType = ProficiencyType.Axe;      break;
                    case OneHandWeaponType.Sword:       proficiencyType = ProficiencyType.Sword;    break;
                    case OneHandWeaponType.Mace:        proficiencyType = ProficiencyType.Hammer;   break;
                    case OneHandWeaponType.Dagger:      proficiencyType = ProficiencyType.Dagger;   break;
                    case OneHandWeaponType.Crossbow:    proficiencyType = ProficiencyType.Crossbow; break;
                }
            }
            break;

            case EquipableCategory.TwoHandWeapon:
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

            case EquipableCategory.Shield:
            {
                switch ((ShieldType)equipableType)
                {
                    case ShieldType.Shield:         proficiencyType = ProficiencyType.Sheild;       break;
                    case ShieldType.TowerShield:    proficiencyType = ProficiencyType.TowerShield;  break;
                }
            }
            break;

            case EquipableCategory.Accessory:
            {
                switch ((AccessoryType)equipableType)
                {
                   
                }
            }
            break;
        }

        Logger.Assert(proficiencyType != ProficiencyType.INVALID, LogTag.Character, "EquipmentUtil",
            string.Format("::EquipableTagToProficiency() - Failed to find proficiency for equipable. Category [{0}] Type[{1}]", category.ToString(), equipableType.ToString()), LogLevel.Warning);

        return proficiencyType;
    }
}

