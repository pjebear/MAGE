using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Common.ProfessionEnums;
using Common.StatusEnums;
using Common.EquipmentEnums;

namespace WorldSystem
{
    namespace Profession
    {
        class Archer : ProfessionBase
        {
            public Archer() : base(ProfessionType.Archer)
            {
                // Abilities

                // Equipment
                EquipmentProficiencies.HasDualWieldProficiency = false;
                EquipmentProficiencies.AddProficiency(EquipmentType.Weapon, (int)WeaponType.Bow);
                //EquipmentProficiencies.AddProficiency(EquipmentType.Weapon, (int)WeaponType.Crossbow);
                EquipmentProficiencies.AddProficiency(EquipmentType.Weapon, (int)WeaponType.Longbow);

                EquipmentProficiencies.AddProficiency(EquipmentType.Armor, (int)ArmorType.Leather);
                EquipmentProficiencies.AddProficiency(EquipmentType.Armor, (int)ArmorType.Cloth);

                // Aura
                Auras.Add(AuraIndex.ARCHER_SPEED_INCREASE);

                // Talents

                // Passives

            }
        }
    }
}

