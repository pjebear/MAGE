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

                EquipmentProficiencies.AddProficiency(EquipmentType.Weapon, (int)WeaponType.Sword);
                EquipmentProficiencies.AddProficiency(EquipmentType.Weapon, (int)WeaponType.Dagger);

                EquipmentProficiencies.AddProficiency(EquipmentType.Armor, (int)ArmorType.Cloth);
                EquipmentProficiencies.AddProficiency(EquipmentType.Armor, (int)ArmorType.Leather);

                // Aura

                // Talents

                // Passives
            }
		}
	}
}

