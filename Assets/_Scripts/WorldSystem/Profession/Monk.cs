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
        class Monk : ProfessionBase
        {
            public Monk() : base(ProfessionType.Monk)
            {
                // Abilities
                Actions.Add(ActionIndex.HEAL);
                Actions.Add(ActionIndex.PROTECT);
                Actions.Add(ActionIndex.SHELL);
                Actions.Add(ActionIndex.CLEANSE);
                Actions.Add(ActionIndex.REVIVE);

                // Equipment
                EquipmentProficiencies.HasDualWieldProficiency = false;
                EquipmentProficiencies.AddProficiency(EquipmentCategory.Weapon, (int)WeaponType.Staff);
                EquipmentProficiencies.AddProficiency(EquipmentCategory.Armor, (int)ArmorType.Cloth);

                // Aura
                Auras.Add(AuraIndex.MONK_HEALTH_REGEN);

                // Talents

                // Passives

            }
        }
    }
}

