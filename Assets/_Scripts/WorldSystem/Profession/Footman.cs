using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Common.ProfessionEnums;
using Common.ActionEnums;
using Common.EquipmentEnums;
using WorldSystem.Talents;

namespace WorldSystem
{
	namespace Profession
	{
		class Footman : ProfessionBase 
		{
			public Footman() : base (ProfessionType.Footman)
            {
                // Abilities
                Actions.Add(ActionIndex.DEFEND);
                Actions.Add(ActionIndex.SHIELD_BASH);

                // Equipment
                EquipmentProficiencies.HasDualWieldProficiency = false;

                EquipmentProficiencies.AddProficiency(EquipmentCategory.Weapon, (int)WeaponType.Sword);
                EquipmentProficiencies.AddProficiency(EquipmentCategory.Weapon, (int)WeaponType.Axe);
                EquipmentProficiencies.AddProficiency(EquipmentCategory.Weapon, (int)WeaponType.Mace);

                EquipmentProficiencies.AddProficiency(EquipmentCategory.Armor, (int)ArmorType.Chain);
                EquipmentProficiencies.AddProficiency(EquipmentCategory.Armor, (int)ArmorType.Leather);

                EquipmentProficiencies.AddProficiency(EquipmentCategory.Shield, (int)ShieldType.Shield);

                // Aura

                // Talents

                /*
                 * <speed increase>          <frontal block>
                 *        |                      |
                 *   <shieldBash>             <defend>
                 *         |                     |
                 *          ________________ 
                 *                    |
                 *                 <Aura>                            
                 */

                //TalentTree.Add(TalentIndex.Footman_Status_SpeedIncrease, new List<TalentIndex>());
                //TalentTree.Add(TalentIndex.Footman_Status_FrontalBlockIncrease, new List<TalentIndex>());
                //TalentTree.Add(TalentIndex.Footman_Action_Defend, new List<TalentIndex>() { TalentIndex.Footman_Status_FrontalBlockIncrease });
                //TalentTree.Add(TalentIndex.Footman_Action_ShieldBash, new List<TalentIndex>() {  TalentIndex.Footman_Status_SpeedIncrease });
                //TalentTree.Add(TalentIndex.Footman_Aura_PhysicalResistanceIncrease, new List<TalentIndex>() { TalentIndex.Footman_Action_Defend, TalentIndex.Footman_Action_ShieldBash });
                
                // Passives
            }
		}
	}
}

