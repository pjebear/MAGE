using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

static class ActionCompositionUtil
{
    public static ActorInteractionBlock CreateOwnerInteractionBlock(EncounterActorController owner, Target target, AnimationId animationId)
    {
        Transform lookAt = null;
        if ((target.TargetType == TargetSelectionType.Actor && target.ActorTarget != owner.EncounterCharacter) // Not targeting yourself
            || (target.TileTarget != EncounterModule.CharacterDirector.GetActorPosition(owner.EncounterCharacter))) // Not targeting your feet
        {
            lookAt = target.GetTargetTransform();
        }

        ActorInteractionBlock casterAnimationBlock = new ActorInteractionBlock(
            owner,
            animationId,
            lookAt,
            StateChange.Empty);

        return casterAnimationBlock;
    }

    public static ActorInteractionBlock CreateTargetInteractionBlock(EncounterActorController owner, EncounterActorController target, InteractionResult result, ISynchronizable interactionPoint)
    {
        Transform lookAt = null;
        if (owner != target // don't look at yourself
            && result.InteractionResultType != InteractionResultType.Hit // if they hit you, you must not have been looking!
            && !result.StateChange.IsBeneficial()) // don't need to look if it's beneficial
        {
            lookAt = owner.transform;
        }

        ActorInteractionBlock targetInteractionBlock = new ActorInteractionBlock(
            target
            , AnimationUtil.InteractionResultTypeToAnimationId(result.InteractionResultType)
            , lookAt
            , result.StateChange);

        targetInteractionBlock.SyncronizeTo(AllignmentPosition.Interaction, 0, interactionPoint, AllignmentPosition.Interaction);

        return targetInteractionBlock;
    }
}

