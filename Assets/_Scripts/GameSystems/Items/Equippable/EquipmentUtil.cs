using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Items;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameSystems
{
    static class EquipmentUtil
    {
        public static bool HasProficiency(List<ProficiencyType> proficiencies, ProficiencyType proficiency)
        {
            return proficiencies.Contains(proficiency);
        }

        public static bool HasProficiencyFor(List<ProficiencyType> proficiencies, Equippable equippable)
        {
            bool hasProficiency
                = proficiencies.Contains(EquipableTagToProficiency(equippable.EquipmentTag.Category, equippable.EquipmentTag.Type))
                || proficiencies.Contains(EquipmentCategoryToProficiency(equippable.EquipmentTag.Category));

            return hasProficiency;
        }

        public static ProficiencyType EquipmentCategoryToProficiency(EquippableCategory category)
        {
            ProficiencyType proficiencyType = ProficiencyType.INVALID;

            switch (category)
            {
                case EquippableCategory.Armor:              proficiencyType = ProficiencyType.Armors; break;
                case EquippableCategory.EmptyHandMelee:     proficiencyType = ProficiencyType.Fists; break;
                case EquippableCategory.OneHandMelee:       proficiencyType = ProficiencyType.OneHands; break;
                case EquippableCategory.TwoHandMelee:       proficiencyType = ProficiencyType.TwoHands; break;
                case EquippableCategory.Ranged:             proficiencyType = ProficiencyType.Ranged; break;
                case EquippableCategory.Shield:             proficiencyType = ProficiencyType.Sheilds; break;
                case EquippableCategory.Accessory:          proficiencyType = ProficiencyType.Accessorys; break;
            }

            Logger.Assert(proficiencyType != ProficiencyType.INVALID, LogTag.Character, "EquipmentUtil",
                string.Format("::EquipmentCategoryToProficiency() - Failed to find proficiency for equipable. Category [{0}]", category.ToString()), LogLevel.Warning);

            return proficiencyType;
        }

        public static bool IsHeld(Equipment.Slot slot)
        {
            return (slot == Equipment.Slot.LeftHand || slot == Equipment.Slot.RightHand);
        }

        public static bool IsRanged(Equippable equippable)
        {
            bool isRanged = false;

            WeaponEquippable weapon = equippable as WeaponEquippable;
            if (weapon != null)
            {
                isRanged = weapon.ProjectileInfo.ProjectileId != ProjectileId.INVALID;
            }

            return isRanged;
        }

        public static bool FitsInSlot(Equippable equippable, Equipment.Slot slot, bool canDualWeild)
        {
            bool fits = false;

            switch (equippable.EquipmentTag.Category)
            {
                case EquippableCategory.Accessory:
                    fits = slot == Equipment.Slot.Accessory;
                    break;

                case EquippableCategory.Armor:
                    fits = slot == Equipment.Slot.Armor;
                    break;

                case EquippableCategory.Shield:
                case EquippableCategory.TwoHandMelee:
                case EquippableCategory.OneHandMelee:
                case EquippableCategory.EmptyHandMelee:
                {
                    fits = slot == Equipment.Slot.LeftHand || slot == Equipment.Slot.RightHand;
                }
                break;
                case EquippableCategory.Ranged:
                {
                    fits = slot == Equipment.Slot.RangedWeapon;   
                }
                break;
                default:
                {
                    Debug.Assert(false);
                }
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

                case EquippableCategory.EmptyHandMelee:
                {
                    switch ((EmptyHandMeleeWeaponType)equipableType)
                    {
                        case EmptyHandMeleeWeaponType.Fist: proficiencyType = ProficiencyType.Fists; break;
                    }
                }
                break;

                case EquippableCategory.OneHandMelee:
                {
                    switch ((OneHandMeleeWeaponType)equipableType)
                    {
                        case OneHandMeleeWeaponType.Axe: proficiencyType = ProficiencyType.Axe; break;
                        case OneHandMeleeWeaponType.Sword: proficiencyType = ProficiencyType.Sword; break;
                        case OneHandMeleeWeaponType.Mace: proficiencyType = ProficiencyType.Hammer; break;
                        case OneHandMeleeWeaponType.Dagger: proficiencyType = ProficiencyType.Dagger; break;
                    }
                }
                break;

                case EquippableCategory.TwoHandMelee:
                {
                    switch ((TwoHandMeleeWeaponType)equipableType)
                    {
                        case TwoHandMeleeWeaponType.BattleAxe: proficiencyType = ProficiencyType.BattleAxe; break;
                        case TwoHandMeleeWeaponType.BastardSword: proficiencyType = ProficiencyType.BastardSword; break;
                        case TwoHandMeleeWeaponType.Maul: proficiencyType = ProficiencyType.Maul; break;
                        case TwoHandMeleeWeaponType.Staff: proficiencyType = ProficiencyType.Staff; break;
                    }
                }
                break;

                 case EquippableCategory.Ranged:
                {
                    switch ((RangedWeaponType)equipableType)
                    {
                        case RangedWeaponType.Crossbow: proficiencyType = ProficiencyType.Crossbow; break;
                        case RangedWeaponType.Bow: proficiencyType = ProficiencyType.Bow; break;
                    }
                }
                break;

                case EquippableCategory.Shield:
                {
                    switch ((ShieldType)equipableType)
                    {
                        case ShieldType.Shield: proficiencyType = ProficiencyType.Sheild; break;
                        case ShieldType.TowerShield: proficiencyType = ProficiencyType.TowerShield; break;
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

        public static float GetHeldEquippableEffectiveness(Attributes holdersAttributes, HeldEquippable held)
        {
            float effectiveness = 0;

            foreach (AttributeScalar scalar in held.EffectivenessScalars)
            {
                effectiveness += holdersAttributes[scalar.AttributeIndex] * scalar.Scalar;
            }

            return effectiveness;
        }
    }
}
