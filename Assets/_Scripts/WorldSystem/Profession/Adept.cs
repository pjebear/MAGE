using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Common.ProfessionEnums;
using Common.ActionEnums;
using Common.EquipmentEnums;
using Common.StatusEnums;

namespace WorldSystem
{
    namespace Profession
    {
        class Adept : ProfessionBase
        {
            public Adept() : base(ProfessionType.Adept)
            {
                // Abilities
                Actions.Add(ActionIndex.FIRE);
                Actions.Add(ActionIndex.FIRE_BLAST);
                Actions.Add(ActionIndex.FLAME_THROWER);

                // Equipment
                EquipmentProficiencies.HasDualWieldProficiency = false;
                EquipmentProficiencies.AddProficiency(EquipmentCategory.Weapon, (int)WeaponType.MageSource);
                EquipmentProficiencies.AddProficiency(EquipmentCategory.Armor, (int)ArmorType.Cloth);

                // Aura
                Auras.Add(AuraIndex.ADEPT_MAGIC_DAMAGE_INCREASE);

                // Talents

                // Passives

            }
        }
    }
}

