using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


static class ActionCompositionUtil
{

    public static ActorInteractionBlock CreateInteractionBlock(EncounterActorController owner, EncounterActorController target, InteractionResult result, ISynchronizable interactionPoint, List<ActionEvent> timeline)
    {
        ActorInteractionBlock targetInteractionBlock = new ActorInteractionBlock(
            target
            , AnimationUtil.InteractionResultTypeToAnimationId(result.InteractionResultType)
            , result.InteractionResultType == InteractionResultType.Hit ? null : owner.transform
            , result.StateChange);

        targetInteractionBlock.SyncronizeTo(AllignmentPosition.Interaction, 0, interactionPoint, AllignmentPosition.Interaction);

        return targetInteractionBlock;
    }
}

