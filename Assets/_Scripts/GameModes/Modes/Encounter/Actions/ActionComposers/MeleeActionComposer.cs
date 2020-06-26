using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class MeleeActionComposer
{
    public static void ComposeAction(EncounterActorController ownerController, ActionInfo actionInfo, TargetSelection targetSelection, out ActionResult result, out Timeline<ActionEvent> timeline)
    {
        List<ActionEvent> timelineEvents = new List<ActionEvent>();
        ActorInteractionBlock casterAnimationBlock = new ActorInteractionBlock(
            ownerController, 
            actionInfo.AnimationInfo.AnimationId,
            targetSelection.FocalTarget.GetTargetTransform(), 
            StateChange.Empty);

        timelineEvents.AddRange(casterAnimationBlock.Events);

        List<EncounterCharacter> targets = EncounterModule.Map.GetActors(targetSelection);
        List<InteractionResult> interactionResults = InteractionResolver.ResolveInteraction(ownerController.EncounterCharacter, actionInfo, targets);
        Dictionary<EncounterCharacter, InteractionResult> targetResults = new Dictionary<EncounterCharacter, InteractionResult>();

        for (int i = 0; i < targets.Count; ++i)
        {
            InteractionResult interactionResult = interactionResults[i];
            targetResults.Add(targets[i], interactionResult);

            ActorInteractionBlock targetInteractionBlock = ActionCompositionUtil.CreateInteractionBlock(
                ownerController
                , EncounterModule.CharacterDirector.CharacterActorLookup[targets[i]]
                , interactionResult
                , casterAnimationBlock
                , timelineEvents);

            timelineEvents.AddRange(targetInteractionBlock.Events);
        }

        result = new ActionResult(ownerController.EncounterCharacter, actionInfo,
            new InteractionResult(InteractionUtil.GetOwnerResultTypeFromResults(interactionResults), actionInfo.ActionCost),
            targetResults);

        timeline = new Timeline<ActionEvent>(timelineEvents);
    }
}

