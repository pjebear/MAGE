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
    static class InteractionResolver
    {
        public static List<InteractionResult> ResolveInteraction(Character caster, ActionInfoBase info, List<Character> targets, Map map)
        {
            List<InteractionResult> results = new List<InteractionResult>();

            foreach (Character target in targets)
            {
                InteractionResult result = null;
                if (info.ActionSource == ActionSource.Weapon)
                {
                    result = GetWeaponInteractionResult(caster, target, info, map);
                }
                else
                {
                    // todo
                    result = new InteractionResult(InteractionResultType.Hit, info.GetTargetStateChange(caster, target));
                }

                results.Add(result);
            }

            return results;
        }

        static InteractionResult GetWeaponInteractionResult(Character caster, Character target, ActionInfoBase info, Map map)
        {
            StateChange stateChange = StateChange.Empty;

            InteractionResultType interactionResultType = InteractionResultType.NUM;

            RelativeOrientation relativeOrientation = InteractionUtil.GetRelativeOrientation(caster, target, map);

            Equipment.Slot parryEquippableSlot = Equipment.Slot.INVALID;
            Equipment.Slot blockEquippableSlot = Equipment.Slot.INVALID;

            if (relativeOrientation == RelativeOrientation.Behind) // target can only dodge
            {

            }
            else if (relativeOrientation == RelativeOrientation.Right)
            {
                HeldEquippable heldEquippable = target.Equipment[Equipment.Slot.RightHand] as HeldEquippable;
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
                HeldEquippable heldEquippable = target.Equipment[Equipment.Slot.LeftHand] as HeldEquippable;
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
                HeldEquippable leftEquippable = target.Equipment[Equipment.Slot.LeftHand] as HeldEquippable;
                HeldEquippable rightEquippable = target.Equipment[Equipment.Slot.RightHand] as HeldEquippable;
                if (leftEquippable.ParryChance != 0 || rightEquippable.ParryChance != 0)
                {
                    parryEquippableSlot = leftEquippable.ParryChance > rightEquippable.ParryChance ? Equipment.Slot.LeftHand : Equipment.Slot.RightHand;
                }

                if (leftEquippable.BlockChance != 0 || rightEquippable.BlockChance != 0)
                {
                    blockEquippableSlot = leftEquippable.BlockChance > rightEquippable.BlockChance ? Equipment.Slot.LeftHand : Equipment.Slot.RightHand;
                }
            }

            float dodgeChance = target.CurrentAttributes[TertiaryStat.Dodge];
            float parryChance = 0;
            if (parryEquippableSlot != Equipment.Slot.INVALID)
            {
                parryChance =
                    (target.Equipment[parryEquippableSlot] as HeldEquippable).ParryChance
                    + target.CurrentAttributes[TertiaryStat.Parry];
            }
            float blockChance = 0;
            if (blockEquippableSlot != Equipment.Slot.INVALID)
            {
                blockChance =
                    (target.Equipment[blockEquippableSlot] as HeldEquippable).BlockChance
                    + target.CurrentAttributes[TertiaryStat.Block];
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
                stateChange = info.GetTargetStateChange(caster, target);

                float physicalReductionMultiplier = 1 - target.CurrentAttributes[TertiaryStat.PhysicalResistance];
                float modifiedHealthChange = stateChange.healthChange * physicalReductionMultiplier;
                stateChange.healthChange = (int)modifiedHealthChange;

            }

            return new InteractionResult(interactionResultType, stateChange);
        }

        // TODO: Spells

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
