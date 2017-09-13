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
		class ShieldWall : ProfessionBase 
		{
			public ShieldWall() : base (ProfessionType.ShieldWall)
            {
                // Abilities
                Actions.Add(ActionIndex.SHIELDWALL_ADVANCE);

                // Equipment
                EquipmentProficiencies.HasDualWieldProficiency = false;

                EquipmentProficiencies.AddProficiency(EquipmentCategory.Weapon, (int)WeaponType.Sword);
                EquipmentProficiencies.AddProficiency(EquipmentCategory.Weapon, (int)WeaponType.Axe);
                EquipmentProficiencies.AddProficiency(EquipmentCategory.Weapon, (int)WeaponType.Mace);

                EquipmentProficiencies.AddProficiency(EquipmentCategory.Armor, (int)ArmorType.Chain);
                EquipmentProficiencies.AddProficiency(EquipmentCategory.Armor, (int)ArmorType.Plate);

                EquipmentProficiencies.AddProficiency(EquipmentCategory.Shield, (int)ShieldType.Shield);
                EquipmentProficiencies.AddProficiency(EquipmentCategory.Shield, (int)ShieldType.TowerShield);

                // Aura

                // Talents

                // Passives
            }
		}
	}
}

