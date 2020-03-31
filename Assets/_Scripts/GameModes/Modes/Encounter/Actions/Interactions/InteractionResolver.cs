using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

static class InteractionResolver
{
    public static List<InteractionResult> ResolveInteraction(EncounterCharacter caster, ActionInfo info, List<EncounterCharacter> targets)
    {
        List<InteractionResult> results = new List<InteractionResult>();

        foreach (EncounterCharacter target in targets)
        {
            InteractionResult result = null;
            if (info.ActionSource == ActionSource.Weapon)
            {
                result = GetWeaponInteractionResult(caster, target, info);
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

    static InteractionResult GetWeaponInteractionResult(EncounterCharacter caster, EncounterCharacter target, ActionInfo info)
    {
        StateChange stateChange = StateChange.Empty;

        InteractionResultType interactionResultType = InteractionResultType.NUM;

        Transform casterTransform = EncounterModule.CharacterDirector.CharacterActorLookup[caster].transform;
        Transform targetTransform = EncounterModule.CharacterDirector.CharacterActorLookup[target].transform;
        RelativeOrientation relativeOrientation = InteractionUtil.GetRelativeOrientation(casterTransform, targetTransform);

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

        float dodgeChance = target.Attributes[AttributeCategory.Stat][(int)TertiaryStat.Dodge].Value;
        float parryChance = 0;
        if (parryEquippableSlot != Equipment.Slot.INVALID)
        {
            parryChance =
                (target.Equipment[parryEquippableSlot] as HeldEquippable).ParryChance
                + target.Attributes[AttributeCategory.Stat][(int)TertiaryStat.Parry].Value;
        }
        float blockChance = 0;
        if (blockEquippableSlot != Equipment.Slot.INVALID)
        {
            blockChance =
                (target.Equipment[blockEquippableSlot] as HeldEquippable).BlockChance
                + target.Attributes[AttributeCategory.Stat][(int)TertiaryStat.Block].Value;
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
            stateChange = info.GetTargetStateChange(caster, target);

            if (info.ActionSource == ActionSource.Weapon)
            {
                float physicalReductionMultiplier = 1 - target.Attributes[AttributeCategory.Stat][(int)TertiaryStat.PhysicalResistance].Value;
                float modifiedHealthChange = stateChange.healthChange * physicalReductionMultiplier;
                stateChange.healthChange = (int)modifiedHealthChange;
            }

            interactionResultType = InteractionResultType.Hit;
        }

        return new InteractionResult(interactionResultType, stateChange);
    }

    // TODO: Spells
}

