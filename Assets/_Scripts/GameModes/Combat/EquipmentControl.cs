using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MAGE.GameSystems.Items;
using MAGE.GameSystems;

namespace MAGE.GameModes.Combat
{
    class EquipmentControl : MonoBehaviour
    {
        public Equipment Equipment = new Equipment();

        public void SetEquipment(Equipment equipment)
        {
            Equipment = equipment;

            if (Equipment[Equipment.Slot.RightHand] == Equipment.NO_EQUIPMENT)
            {
                Equipment[Equipment.Slot.RightHand] = ItemFactory.LoadEquipable(EquippableId.Fists_0);
            }

            if (Equipment[Equipment.Slot.LeftHand] == Equipment.NO_EQUIPMENT)
            {
                Equipment[Equipment.Slot.LeftHand] = ItemFactory.LoadEquipable(EquippableId.Fists_0);
            }
        }

        public bool IsRangedEquipped()
        {
            return Equipment[Equipment.Slot.RightHand] != Equipment.NO_EQUIPMENT
                && (Equipment[Equipment.Slot.RightHand] as WeaponEquippable).ProjectileInfo.ProjectileId != ProjectileId.INVALID;
        }
    }
}
