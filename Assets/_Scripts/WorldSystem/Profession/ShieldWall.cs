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

                EquipmentProficiencies.AddProficiency(EquipmentType.Weapon, (int)WeaponType.Sword);
                EquipmentProficiencies.AddProficiency(EquipmentType.Weapon, (int)WeaponType.Axe);
                EquipmentProficiencies.AddProficiency(EquipmentType.Weapon, (int)WeaponType.Mace);

                EquipmentProficiencies.AddProficiency(EquipmentType.Armor, (int)ArmorType.Chain);
                EquipmentProficiencies.AddProficiency(EquipmentType.Armor, (int)ArmorType.Plate);

                EquipmentProficiencies.AddProficiency(EquipmentType.Shield, (int)ShieldType.Base);
                EquipmentProficiencies.AddProficiency(EquipmentType.Shield, (int)ShieldType.Tower);

                // Aura

                // Talents

                // Passives
            }
		}
	}
}

