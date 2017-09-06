using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Common.ProfessionEnums;
using Common.ActionEnums;
using Common.StatusEnums;
using Common.EquipmentEnums;

namespace WorldSystem
{
	namespace Profession
	{
		class Berserker : ProfessionBase 
		{
			public Berserker() : base (ProfessionType.Berserker)
            {
                // Abilities
                Actions.Add(ActionIndex.BERSERKER_BATTLECRY);
                Actions.Add(ActionIndex.BERSERKER_CLEAVE);

                // Equipment
                EquipmentProficiencies.HasDualWieldProficiency = false;

                EquipmentProficiencies.AddProficiency(EquipmentType.Weapon, (int)WeaponType.BastardSword);
                EquipmentProficiencies.AddProficiency(EquipmentType.Weapon, (int)WeaponType.BattleAxe);
                EquipmentProficiencies.AddProficiency(EquipmentType.Weapon, (int)WeaponType.Maul);

                EquipmentProficiencies.AddProficiency(EquipmentType.Armor, (int)ArmorType.Leather);

                // Aura
                Auras.Add(AuraIndex.PHYSICAL_CRITICAL_PERCENT);
                Listeners.Add(EncounterSystem.EventSystem.EventListenerIndex.BloodScent);

                // Talents

                // Passives
                
            }
		}
	}
}

