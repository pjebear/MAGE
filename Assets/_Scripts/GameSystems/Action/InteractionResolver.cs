﻿using MAGE.GameModes.Combat;
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
    static class InteractionResolver
    {
        public static InteractionResult GetWeaponInteractionResult(CombatEntity attacker, CombatTarget target, StateChange weaponStateChange)
        {
            StateChange stateChange = StateChange.Empty;
            
            InteractionResultType interactionResultType = InteractionResultType.NUM;

            RelativeOrientation relativeOrientation = InteractionUtil.GetRelativeOrientation(attacker.transform, target.transform);

            float blockChance = 0;
            float dodgeChance = 0;
            float parryChance = 0;

            EquipmentControl targetEquipmentControl = target.GetComponent<EquipmentControl>();
            if (targetEquipmentControl != null)
            {
                Equipment.Slot parryEquippableSlot = Equipment.Slot.INVALID;
                Equipment.Slot blockEquippableSlot = Equipment.Slot.INVALID;

                // Calculate evasion from equipment
                if (relativeOrientation == RelativeOrientation.Behind) // target can only dodge
                {

                }
                else if (relativeOrientation == RelativeOrientation.Right)
                {
                    HeldEquippable heldEquippable = targetEquipmentControl.Equipment[Equipment.Slot.RightHand] as HeldEquippable;
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
                    HeldEquippable heldEquippable = targetEquipmentControl.Equipment[Equipment.Slot.LeftHand] as HeldEquippable;
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
                    HeldEquippable leftEquippable = targetEquipmentControl.Equipment[Equipment.Slot.LeftHand] as HeldEquippable;
                    HeldEquippable rightEquippable = targetEquipmentControl.Equipment[Equipment.Slot.RightHand] as HeldEquippable;
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
                    parryChance = (targetEquipmentControl.Equipment[parryEquippableSlot] as HeldEquippable).ParryChance;
                }
               
                if (blockEquippableSlot != Equipment.Slot.INVALID)
                {
                    blockChance = (targetEquipmentControl.Equipment[blockEquippableSlot] as HeldEquippable).BlockChance;
                }
            }

            StatsControl targetStatControl = target.GetComponent<StatsControl>();
            if (targetStatControl != null)
            {
                dodgeChance = targetStatControl.Attributes[TertiaryStat.Dodge];
                if (blockChance > 0)
                {
                    blockChance += targetStatControl.Attributes[TertiaryStat.Block];
                }
                if (parryChance > 0)
                {
                    parryChance += targetStatControl.Attributes[TertiaryStat.Parry];
                }
            }

            if (UnityEngine.Random.Range(0, 100) < parryChance)
            {
                interactionResultType = InteractionResultType.Parry;
            }
            else if (UnityEngine.Random.Range(0, 100) < blockChance)
            {
                interactionResultType = InteractionResultType.Block;
            }
            else if (UnityEngine.Random.Range(0, 100) < dodgeChance)
            {
                interactionResultType = InteractionResultType.Dodge;
            }
            else
            {
                interactionResultType = InteractionResultType.Hit;

                stateChange = weaponStateChange.Copy();
                float physicalReductionMultiplier = 1;
                if (targetStatControl != null)
                {
                    physicalReductionMultiplier = 1 - targetStatControl.Attributes[TertiaryStat.PhysicalResistance];
                    if (physicalReductionMultiplier < 0)
                    {
                        Debug.LogWarning("Target has greater than 100 percent physical reduction. defaulting to 90 percent");
                        physicalReductionMultiplier = .1f;
                    }
                    
                }

                stateChange.healthChange = (int)(stateChange.healthChange * physicalReductionMultiplier);
            }

            return new InteractionResult(interactionResultType, stateChange);
        }
    }


}
