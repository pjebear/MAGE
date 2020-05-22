using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class SwordAttackComposer
{
    public static void ComposeAction(ActionProposal proposal, out ActionResult result, out Timeline<ActionEvent> timeline)
    {
        EncounterCharacter owner = proposal.Owner;
        ActionInfo actionInfo = owner.GetActionInfo(proposal.Action);

        InteractionResultType ownerResultType = InteractionResultType.Miss;
        StateChange ownerStateChange = actionInfo.ActionCost;

        Dictionary<EncounterCharacter, InteractionResult> targetResults = new Dictionary<EncounterCharacter, InteractionResult>();

        List <ActionEvent> timelineEvents = new List<ActionEvent>();
        AnimationPlaceholder swordSwing = AnimationFactory.CheckoutAnimation(AnimationId.SwordSwing);
        timelineEvents.Add(new AnimationEvent(owner, swordSwing, 0, proposal.ActionTarget.FocalTarget.GetTargetTransform()));
        timelineEvents.Add(new AudioEvent(EncounterModule.CharacterDirector.CharacterActorLookup[owner].Actor.AudioSource, SFXId.WeaponSwing, 0));
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
           
            AnimationPlaceholder animation = AnimationFactory.CheckoutAnimation(AnimationUtil.InteractionResultTypeToAnimationId(interactionResult.InteractionResultType));
            
            SyncPoint.Syncronize(swordSwing, AllignmentPosition.Interaction, animation, AllignmentPosition.Interaction, 0);

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