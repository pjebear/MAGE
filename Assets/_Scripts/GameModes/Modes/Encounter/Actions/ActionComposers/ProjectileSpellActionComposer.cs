using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ProjectileSpellActionComposer
{
    public static void ComposeAction(ActionProposal proposal, out ActionResult result, out Timeline<ActionEvent> timeline)
    {
        EncounterCharacter owner = proposal.Owner;
        EncounterActorController ownerController = EncounterModule.CharacterDirector.CharacterActorLookup[owner];
        ActionInfo actionInfo = owner.GetActionInfo(proposal.Action);

        InteractionResultType ownerResultType = InteractionResultType.Miss;
        StateChange ownerStateChange = actionInfo.ActionCost;

        Dictionary<EncounterCharacter, InteractionResult> targetResults = new Dictionary<EncounterCharacter, InteractionResult>();

        List<ActionEvent> timelineEvents = new List<ActionEvent>();
        AnimationPlaceholder castAnimation = AnimationFactory.CheckoutAnimation(AnimationId.Cast);
        timelineEvents.Add(new AnimationEvent(owner, castAnimation, 0, proposal.ActionTarget.FocalTarget.GetTargetTransform()));
        timelineEvents.Add(new AudioEvent(ownerController.Actor.AudioSource, SFXId.Cast, 0));

        Tile projectileSpawnPoint = EncounterModule.Map.ActorPositionLookup[ownerController];
        Tile projectileEndPoint = null;
        if (proposal.ActionTarget.FocalTarget.TargetType == TargetSelectionType.Actor)
        {
            TileIdx location = EncounterModule.CharacterDirector.GetActorPosition(proposal.ActionTarget.FocalTarget.ActorTarget);
            projectileEndPoint = EncounterModule.Map[location];
        }
        else
        {
            TileIdx location = proposal.ActionTarget.FocalTarget.TileTarget;
            projectileEndPoint = EncounterModule.Map[location];
        }

        ProjectileSpawnParams spawnParams = EncounterModule.ProjectileDirector.GenerateSpawnParams(projectileSpawnPoint, projectileEndPoint, ProjectilePathType.Linear, ProjectileId.FireBall);
        timelineEvents.Add(new ProjectileSpawnEvent(spawnParams, castAnimation.SyncedFrame));

        List<EncounterCharacter> targets = new List<EncounterCharacter>();
        if (spawnParams.CollisionWith != null)
        {
            EncounterActorController encounterActorController = spawnParams.CollisionWith.GetComponent<EncounterActorController>();
            if (encounterActorController != null)
            {
                targets.Add(encounterActorController.EncounterCharacter);
            }
        }

        List<InteractionResult> interactionResults = InteractionResolver.ResolveInteraction(owner, actionInfo, targets);
        Logger.Assert(targets.Count <= 1, LogTag.GameModes, "ProjectileActionComposer", string.Format("Expected at most one target, got {0}", targets.Count));
        if (targets.Count > 0)
        {
            EncounterCharacter target = targets[0];
            InteractionResult interactionResult = interactionResults[0];

            targetResults.Add(target, interactionResult);

            // TODO: Move this to a helper
            {
                if (interactionResult.InteractionResultType == InteractionResultType.Hit)
                {
                    ownerResultType = InteractionResultType.Hit;
                }
                else
                {
                    ownerResultType = InteractionResultType.Partial;
                }
            }

            AnimationPlaceholder animation = AnimationFactory.CheckoutAnimation(AnimationUtil.InteractionResultTypeToAnimationId(interactionResult.InteractionResultType));

            SyncPoint.Syncronize(castAnimation, AllignmentPosition.Interaction, animation, AllignmentPosition.Interaction, AnimationConstants.FRAMES_IN_DURATION(spawnParams.FlightDuration));

            Transform targetFocus = null;
            if (interactionResult.InteractionResultType != InteractionResultType.Hit)
            {
                targetFocus = EncounterModule.CharacterDirector.CharacterActorLookup[owner].transform;
            }
            timelineEvents.Add(new AnimationEvent(target, animation, animation.Parent.GetAbsoluteOffset(AllignmentPosition.Start), targetFocus));
            timelineEvents.Add(new StateChangeEvent(target, interactionResult.StateChange, animation.Parent.GetAbsoluteOffset(AllignmentPosition.Interaction)));

            SFXId targetSFX = InteractionUtil.GetSFXForInteractionResult(interactionResult.InteractionResultType);
            if (targetSFX != SFXId.INVALID)
            {
                timelineEvents.Add(new AudioEvent(EncounterModule.CharacterDirector.CharacterActorLookup[target].GetComponent<AudioSource>()
                    , targetSFX, animation.Parent.GetAbsoluteOffset(AllignmentPosition.Interaction)));
            }
        }

        result = new ActionResult(owner, ActionId.SwordAttack, actionInfo,
            new InteractionResult(ownerResultType, ownerStateChange),
            targetResults);

        timeline = new Timeline<ActionEvent>(timelineEvents);
    }
}

