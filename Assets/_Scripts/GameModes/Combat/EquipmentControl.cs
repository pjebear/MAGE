using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MAGE.GameSystems.Items;
using MAGE.GameSystems;
using MAGE.GameSystems.Stats;
using MAGE.GameModes.SceneElements;

namespace MAGE.GameModes.Combat
{
    class EquipmentControl : MonoBehaviour
    {
        public Equipment Equipment = new Equipment();

        public bool IsDisarmed = false;
        public Equipment DisarmedEquipment = new Equipment();

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

        public Equipment.Slot GetSlotMatchingProficiency(ProficiencyType proficiencyType)
        {
            Equipment.Slot slot = Equipment.Slot.INVALID;

            List<ProficiencyType> proficiences = new List<ProficiencyType>() { proficiencyType };

            if (EquipmentUtil.HasProficiencyFor(proficiences, Equipment[Equipment.Slot.RightHand]))
            {
                slot = Equipment.Slot.RightHand;
            }

            if (EquipmentUtil.HasProficiencyFor(proficiences, Equipment[Equipment.Slot.LeftHand]))
            {
                slot = Equipment.Slot.LeftHand;
            }

            if (Equipment[Equipment.Slot.RangedWeapon] != Equipment.NO_EQUIPMENT 
                && EquipmentUtil.HasProficiencyFor(proficiences, Equipment[Equipment.Slot.RangedWeapon]))
            {
                slot = Equipment.Slot.RangedWeapon;
            }

            return slot;
        }

        public bool DoesSlotMatchProficiency(Equipment.Slot slot, ProficiencyType proficiencyType)
        {
            List<ProficiencyType> proficiences = new List<ProficiencyType>() { proficiencyType };

            return EquipmentUtil.HasProficiencyFor(proficiences, Equipment[slot]);
        }

        public bool IsRangedEquipped()
        {
            return Equipment[Equipment.Slot.RangedWeapon] != Equipment.NO_EQUIPMENT;
        }

        void OnStatsUpdated()
        {
            StatsControl statsControl = GetComponent<StatsControl>();
            if (statsControl.Attributes[StatusType.Disarmed] > 0)
            {
                if (!IsDisarmed)
                {
                    IsDisarmed = true;

                    if (Equipment[Equipment.Slot.RightHand] != Equipment.NO_EQUIPMENT)
                    {
                        DisarmedEquipment[Equipment.Slot.RightHand] = Equipment[Equipment.Slot.RightHand];
                        Equipment[Equipment.Slot.RightHand] = ItemFactory.LoadEquipable(EquippableId.Fists_0);

                        GetComponentInChildren<ActorOutfitter>()?.ApplyHeldApparel(GameSystems.Appearances.ApparelAssetId.NONE, HumanoidActorConstants.HeldApparelType.Melee, HumanoidActorConstants.Hand.Right);
                    }

                    if (Equipment[Equipment.Slot.LeftHand] != Equipment.NO_EQUIPMENT)
                    {
                        DisarmedEquipment[Equipment.Slot.LeftHand] = Equipment[Equipment.Slot.LeftHand];
                        Equipment[Equipment.Slot.LeftHand] = ItemFactory.LoadEquipable(EquippableId.Fists_0);
                        GetComponentInChildren<ActorOutfitter>()?.ApplyHeldApparel(GameSystems.Appearances.ApparelAssetId.NONE, HumanoidActorConstants.HeldApparelType.Melee, HumanoidActorConstants.Hand.Left);
                    }

                    if (Equipment[Equipment.Slot.RangedWeapon] != Equipment.NO_EQUIPMENT)
                    {
                        DisarmedEquipment[Equipment.Slot.RangedWeapon] = Equipment[Equipment.Slot.RangedWeapon];
                        Equipment[Equipment.Slot.RangedWeapon] = Equipment.NO_EQUIPMENT;
                        GetComponentInChildren<ActorOutfitter>()?.ApplyHeldApparel(GameSystems.Appearances.ApparelAssetId.NONE, HumanoidActorConstants.HeldApparelType.Ranged, HumanoidActorConstants.Hand.Right);
                    }

                    // update appearance
                }
            }
            else if (IsDisarmed)
            {
                IsDisarmed = false;

                if (DisarmedEquipment[Equipment.Slot.RightHand] != Equipment.NO_EQUIPMENT)
                {
                    Equipment[Equipment.Slot.RightHand] = DisarmedEquipment[Equipment.Slot.RightHand];
                    DisarmedEquipment[Equipment.Slot.RightHand] = Equipment.NO_EQUIPMENT;

                    GetComponentInChildren<ActorOutfitter>()?.ApplyHeldApparel(Equipment[Equipment.Slot.RightHand].PrefabId, HumanoidActorConstants.HeldApparelType.Melee, HumanoidActorConstants.Hand.Right);
                }

                if (DisarmedEquipment[Equipment.Slot.LeftHand] != Equipment.NO_EQUIPMENT)
                {
                    Equipment[Equipment.Slot.LeftHand] = DisarmedEquipment[Equipment.Slot.LeftHand];
                    DisarmedEquipment[Equipment.Slot.LeftHand] = Equipment.NO_EQUIPMENT;

                    GetComponentInChildren<ActorOutfitter>()?.ApplyHeldApparel(Equipment[Equipment.Slot.RightHand].PrefabId, HumanoidActorConstants.HeldApparelType.Melee, HumanoidActorConstants.Hand.Left);
                }

                if (DisarmedEquipment[Equipment.Slot.RangedWeapon] != Equipment.NO_EQUIPMENT)
                {
                    Equipment[Equipment.Slot.RangedWeapon] = DisarmedEquipment[Equipment.Slot.RangedWeapon];
                    DisarmedEquipment[Equipment.Slot.RangedWeapon] = Equipment.NO_EQUIPMENT;

                    GetComponentInChildren<ActorOutfitter>()?.ApplyHeldApparel(Equipment[Equipment.Slot.RightHand].PrefabId, HumanoidActorConstants.HeldApparelType.Ranged, HumanoidActorConstants.Hand.Right);
                }
            }
        }
    }
}
