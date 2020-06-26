using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class EffectActionComposer
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
            EncounterActorController targetController = EncounterModule.CharacterDirector.CharacterActorLookup[targets[i]];

            // Effect Spawn
            Logger.Assert(actionInfo.EffectInfo.EffectId != EffectType.INVALID, LogTag.GameModes,
               "EffectActionComposer", string.Format("No Effcet information provided for action {0}", actionInfo.ActionId.ToString()));
            EffectSpawnBlock effectSpawnBlock = new EffectSpawnBlock(actionInfo.EffectInfo.EffectId, targetController.transform);
            effectSpawnBlock.SyncronizeTo(AllignmentPosition.Start, 0, casterAnimationBlock, AllignmentPosition.Interaction);
            timelineEvents.AddRange(effectSpawnBlock.Events);

            // Time the state change to effect spawn

            ActorInteractionBlock targetInteractionBlock = ActionCompositionUtil.CreateInteractionBlock(
                ownerController
                , targetController
                , interactionResult
                , effectSpawnBlock
                , timelineEvents);

            timelineEvents.AddRange(targetInteractionBlock.Events);
        }

        result = new ActionResult(ownerController.EncounterCharacter, actionInfo,
            new InteractionResult(InteractionUtil.GetOwnerResultTypeFromResults(interactionResults), actionInfo.ActionCost),
            targetResults);

        timeline = new Timeline<ActionEvent>(timelineEvents);
    }
}

