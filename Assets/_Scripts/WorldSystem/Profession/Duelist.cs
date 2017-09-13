using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Common.ProfessionEnums;
using Common.ActionEnums;
using Common.EquipmentEnums;

namespace WorldSystem
{
	namespace Profession
	{
		class Duelist : ProfessionBase 
		{
			public Duelist() : base (ProfessionType.Duelist)
            {
                // Abilities

                // Equipment
                EquipmentProficiencies.HasDualWieldProficiency = true;

                EquipmentProficiencies.AddProficiency(EquipmentCategory.Weapon, (int)WeaponType.Sword);
                EquipmentProficiencies.AddProficiency(EquipmentCategory.Weapon, (int)WeaponType.Dagger);

                EquipmentProficiencies.AddProficiency(EquipmentCategory.Armor, (int)ArmorType.Cloth);
                EquipmentProficiencies.AddProficiency(EquipmentCategory.Armor, (int)ArmorType.Leather);

                // Aura

                // Talents

                // Passives
            }
		}
	}
}

