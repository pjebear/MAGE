﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class HealActionComposer
{
    public static void ComposeAction(ActionProposal proposal, out ActionResult result, out Timeline<ActionEvent> timeline)
    {
        EncounterCharacter owner = proposal.Owner;
        ActionInfo actionInfo = owner.GetActionInfo(proposal.Action);

        InteractionResultType ownerResultType = InteractionResultType.Miss;
        StateChange ownerStateChange = actionInfo.ActionCost;

        Dictionary<EncounterCharacter, InteractionResult> targetResults = new Dictionary<EncounterCharacter, InteractionResult>();

        List<ActionEvent> timelineEvents = new List<ActionEvent>();
        AnimationPlaceholder cast = AnimationFactory.CheckoutAnimation(AnimationId.Cast);
        Transform targetTransform = proposal.ActionTarget.FocalTarget.GetTargetTransform();
        
        timelineEvents.Add(new AnimationEvent(owner, cast, 0, targetTransform));

        List<EncounterCharacter> targets = EncounterModule.Map.GetActors(proposal.ActionTarget);
        List<InteractionResult> interactionResults = InteractionResolver.ResolveInteraction(owner, actionInfo, targets);
        for (int i = 0; i < targets.Count; ++i)
        {
            EncounterCharacter target = targets[i];
            InteractionResult interactionResult = interactionResults[i];

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

            EffectPlaceholder healEffect = EffectFactory.GetEffectPlaceholder(EffectType.Heal, targetTransform);
            SyncPoint.Syncronize(cast, AllignmentPosition.Interaction, healEffect, AllignmentPosition.Start, 0);

            timelineEvents.Add(new EffectSpawnEvent(healEffect, healEffect.Parent.GetAbsoluteOffset(AllignmentPosition.Start)));

            AnimationPlaceholder targetReaction = AnimationFactory.CheckoutAnimation(AnimationUtil.InteractionResultTypeToAnimationId(interactionResult.InteractionResultType));
            SyncPoint.Syncronize(healEffect, AllignmentPosition.Interaction, targetReaction, AllignmentPosition.Interaction, 0);
            timelineEvents.Add(new AnimationEvent(target, targetReaction, targetReaction.Parent.GetAbsoluteOffset(AllignmentPosition.Start), null));
            timelineEvents.Add(new StateChangeEvent(target, interactionResult.StateChange, targetReaction.Parent.GetAbsoluteOffset(AllignmentPosition.Interaction)));
        }

        result = new ActionResult(owner, ActionId.SwordAttack, actionInfo,
            new InteractionResult(ownerResultType, ownerStateChange),
            targetResults);

        timeline = new Timeline<ActionEvent>(timelineEvents);
    }
}
