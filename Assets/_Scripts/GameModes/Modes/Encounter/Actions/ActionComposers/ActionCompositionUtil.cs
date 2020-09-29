using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Encounter
{
    static class ActionCompositionUtil
    {
        public static ActorInteractionBlock CreateOwnerInteractionBlock(CharacterActorController owner, Target target, AnimationId animationId)
        {
            Transform lookAt = null;
            if ((target.TargetType == TargetSelectionType.Character && target.CharacterTarget != owner.Character) // Not targeting yourself
                || (target.TileTarget != EncounterFlowControl.MapControl.Map.GetCharacterPosition(owner.Character).Location)) // Not targeting your feet
            {
                lookAt = EncounterFlowControl.MapControl.GetTargetTransform(target);
            }

            ActorInteractionBlock casterAnimationBlock = new ActorInteractionBlock(
                owner,
                animationId,
                lookAt,
                StateChange.Empty);

            return casterAnimationBlock;
        }

        public static ActorInteractionBlock CreateTargetInteractionBlock(CharacterActorController owner, CharacterActorController target, InteractionResult result, ISynchronizable interactionPoint)
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


}
