using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ProjectileActionComposer
{
    public static void ComposeAction(EncounterActorController ownerController, ActionInfo actionInfo, TargetSelection targetSelection, out ActionResult result, out Timeline<ActionEvent> timeline)
    {
        List<ActionEvent> timelineEvents = new List<ActionEvent>();

        ActorInteractionBlock casterAnimationBlock = ActionCompositionUtil.CreateOwnerInteractionBlock(ownerController, targetSelection.FocalTarget, actionInfo.AnimationInfo.AnimationId);
        timelineEvents.AddRange(casterAnimationBlock.Events);

        Logger.Assert(actionInfo.ProjectileInfo.ProjectileId != ProjectileId.INVALID, LogTag.GameModes,
            "ProjectileActionComposer", string.Format("No projectile information provided for action {0}", actionInfo.ActionId.ToString()));

        ProjectileSpawnParams projectileSpawnParams = ProjectileUtil.GenerateSpawnParams(
            EncounterModule.Map.ActorPositionLookup[ownerController],
            targetSelection.FocalTarget,
            actionInfo.CastRange.AreaType == AreaType.Expanding ? ProjectilePathType.Arc : ProjectilePathType.Linear,
            actionInfo.ProjectileInfo.ProjectileId);

        ProjectileSpawnBlock projectileSpawnBlock = new ProjectileSpawnBlock(projectileSpawnParams);
        projectileSpawnBlock.SyncronizeTo(AllignmentPosition.Start, 0, casterAnimationBlock, AllignmentPosition.Interaction);
        timelineEvents.AddRange(projectileSpawnBlock.Events);

        List<EncounterCharacter> targets = new List<EncounterCharacter>();
        if (projectileSpawnParams.CollisionWith != null)
        {
            EncounterActorController encounterActorController = projectileSpawnParams.CollisionWith.GetComponent<EncounterActorController>();
            if (encounterActorController != null)
            {
                targets.Add(encounterActorController.EncounterCharacter);
            }
        }

        List<InteractionResult> interactionResults = InteractionResolver.ResolveInteraction(ownerController.EncounterCharacter, actionInfo, targets);
        Dictionary<EncounterCharacter, InteractionResult> targetResults = new Dictionary<EncounterCharacter, InteractionResult>();

        for (int i = 0; i < targets.Count; ++i)
        {
            InteractionResult interactionResult = interactionResults[i];
            targetResults.Add(targets[i], interactionResult);

            ActorInteractionBlock targetInteractionBlock = ActionCompositionUtil.CreateTargetInteractionBlock(
                ownerController
                , EncounterModule.CharacterDirector.CharacterActorLookup[targets[i]]
                , interactionResult
                , casterAnimationBlock);

            timelineEvents.AddRange(targetInteractionBlock.Events);
        }

        result = new ActionResult(ownerController.EncounterCharacter, actionInfo,
            new InteractionResult(InteractionUtil.GetOwnerResultTypeFromResults(interactionResults), actionInfo.ActionCost),
            targetResults);

        timeline = new Timeline<ActionEvent>(timelineEvents);
    }
}