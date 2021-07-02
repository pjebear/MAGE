using MAGE.GameModes.Combat;
using MAGE.GameSystems;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Items;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameSystems.Actions
{
    static class InteractionUtil
    {
        public static void GetAvoidanceAttributesForEntity(CombatEntity combatEntity, out float dodgeChance, out float blockChance, out float parryChance, RelativeOrientation relativeOrientation = RelativeOrientation.Front)
        {
            blockChance = 0;
            dodgeChance = 0;
            parryChance = 0;

            EquipmentControl equipmentControl = combatEntity.GetComponent<EquipmentControl>();
            if (equipmentControl != null)
            {
                Equipment.Slot parryEquippableSlot = Equipment.Slot.INVALID;
                Equipment.Slot blockEquippableSlot = Equipment.Slot.INVALID;

                // Calculate evasion from equipment
                if (relativeOrientation == RelativeOrientation.Behind) // target can only dodge
                {

                }
                else if (relativeOrientation == RelativeOrientation.Right)
                {
                    HeldEquippable heldEquippable = equipmentControl.Equipment[Equipment.Slot.RightHand] as HeldEquippable;
                    if (heldEquippable.ParryChance != 0)
                    {
                        parryEquippableSlot = Equipment.Slot.RightHand;
                    }
                    if (heldEquippable.BlockChance != 0)
                    {
                        blockEquippableSlot = Equipment.Slot.RightHand;
                    }
                }
                else if (relativeOrientation == RelativeOrientation.Left)
                {
                    HeldEquippable heldEquippable = equipmentControl.Equipment[Equipment.Slot.LeftHand] as HeldEquippable;
                    if (heldEquippable.ParryChance != 0)
                    {
                        parryEquippableSlot = Equipment.Slot.LeftHand;
                    }
                    if (heldEquippable.BlockChance != 0)
                    {
                        blockEquippableSlot = Equipment.Slot.LeftHand;
                    }
                }
                else // can block/parry with either weapon
                {
                    HeldEquippable leftEquippable = equipmentControl.Equipment[Equipment.Slot.LeftHand] as HeldEquippable;
                    HeldEquippable rightEquippable = equipmentControl.Equipment[Equipment.Slot.RightHand] as HeldEquippable;
                    if (leftEquippable.ParryChance != 0 || rightEquippable.ParryChance != 0)
                    {
                        parryEquippableSlot = leftEquippable.ParryChance > rightEquippable.ParryChance ? Equipment.Slot.LeftHand : Equipment.Slot.RightHand;
                    }

                    if (leftEquippable.BlockChance != 0 || rightEquippable.BlockChance != 0)
                    {
                        blockEquippableSlot = leftEquippable.BlockChance > rightEquippable.BlockChance ? Equipment.Slot.LeftHand : Equipment.Slot.RightHand;
                    }
                }

                if (parryEquippableSlot != Equipment.Slot.INVALID)
                {
                    parryChance = (equipmentControl.Equipment[parryEquippableSlot] as HeldEquippable).ParryChance;
                }

                if (blockEquippableSlot != Equipment.Slot.INVALID)
                {
                    blockChance = (equipmentControl.Equipment[blockEquippableSlot] as HeldEquippable).BlockChance;
                }
            }

            StatsControl targetStatControl = combatEntity.GetComponent<StatsControl>();
            if (targetStatControl != null)
            {
                float avoidanceMultiplier = 1 + targetStatControl.Attributes[TertiaryStat.AvoidanceMultiplier];
                dodgeChance = targetStatControl.Attributes[TertiaryStat.Dodge];
                dodgeChance *= avoidanceMultiplier;
                if (blockChance > 0)
                {
                    blockChance += targetStatControl.Attributes[TertiaryStat.Block];
                    blockChance *= avoidanceMultiplier;
                }
                if (parryChance > 0)
                {
                    parryChance += targetStatControl.Attributes[TertiaryStat.Parry];
                    parryChance *= avoidanceMultiplier;
                }
            }
        }

        public static InteractionResultType GetOwnerResultTypeFromResults(Dictionary<CombatTarget, List<InteractionResult>> results)
        {
            InteractionResultType interactionResultType = InteractionResultType.Miss;

            foreach (List<InteractionResult> targetResults in results.Values)
            {
                foreach (InteractionResult interactionResult in targetResults)
                {
                    if (interactionResult.InteractionResultType == InteractionResultType.Hit)
                    {
                        interactionResultType = InteractionResultType.Hit;
                        break;
                    }
                    else
                    {
                        // don't break. Keep trying to find a hit
                        interactionResultType = InteractionResultType.Partial;
                    }
                }
            }

            return interactionResultType;
        }

        public static SFXId GetSFXForInteractionResult(InteractionResultType resultType)
        {
            SFXId sFXId = SFXId.INVALID;

            switch (resultType)
            {
                case InteractionResultType.Block: sFXId = SFXId.ShieldBlock; break;
                case InteractionResultType.Dodge: sFXId = SFXId.Dodge; break;
                case InteractionResultType.Parry: sFXId = SFXId.Parry; break;
                case InteractionResultType.Hit: sFXId = SFXId.Slash; break;
            }
            return sFXId;
        }

        public static RelativeOrientation GetRelativeOrientation(Transform relative, Transform to)
        {

            Vector3 toForward = to.forward;
            Vector3 toToRealtive = relative.position - to.position;

            float degreesBetweenForwardAndRelative = Vector3.SignedAngle(toForward, toToRealtive, Vector3.up);

            RelativeOrientation relativeOrientation = RelativeOrientation.Behind;

            if (Mathf.Abs(degreesBetweenForwardAndRelative) < 45)
            {
                relativeOrientation = RelativeOrientation.Front;
            }
            else if (Mathf.Abs(degreesBetweenForwardAndRelative) > 90)
            {
                relativeOrientation = RelativeOrientation.Behind;
            }
            else if (degreesBetweenForwardAndRelative < 0)
            {
                relativeOrientation = RelativeOrientation.Left;
            }
            else
            {
                relativeOrientation = RelativeOrientation.Right;
            }

            return relativeOrientation;
        }
    }


}
