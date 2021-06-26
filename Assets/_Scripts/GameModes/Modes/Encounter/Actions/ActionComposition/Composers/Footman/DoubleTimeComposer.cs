using MAGE.GameModes.Combat;
using MAGE.GameModes.SceneElements;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Encounter
{
    class DoubleTimeComposer : ActionComposerBase
    {
        public DoubleTimeComposer(CombatEntity owner) : base(owner)
        {
            // empty
        }

        protected override ActionInfo PopulateActionInfo()
        {
            ActionInfo actionInfo = new ActionInfo();

            actionInfo.ActionId = ActionId.DoubleTime;
            actionInfo.AnimationInfo.AnimationId = GameSystems.AnimationId.INVALID;
            actionInfo.ActionCost = new StateChange(StateChangeType.ActionCost, 0, 0);
            actionInfo.ActionRange = ActionRange.Meele;
            actionInfo.ActionSource = ActionSource.Cast;

            actionInfo.IsSelfCast = true;

            actionInfo.EffectRange = new RangeInfo()
            {
                AreaType = AreaType.Point,
                TargetingType = TargetingType.Allies
            };

            return actionInfo;
        }

        protected override InteractionSolverBase CreateInteractionSolver()
        {
            return new SpellInteractionSolver();
        }

        protected override void InitInteractionSolver(InteractionSolverBase interactionSolverBase)
        {
            interactionSolverBase.StateChange = new ConcreteVar<StateChange>(new StateChange(StateChangeType.ActionTarget, StatusEffectFactory.CheckoutStatusEffect(StatusEffectId.DoubleTime)));
        }

        protected override CompositionNode PopulateComposition()
        {
            CompositionNode composition = new AnimationComposer()
            {
                // AnimationConstructor
                ToAnimate = new DeferredMonoConversion<CombatEntity, ActorAnimator>(DeferredOwner)
                ,
                AnimationTarget = new DeferredTargetPosition(Target)
                ,
                AnimationInfo = new ConcreteVar<AnimationInfo>(AnimationFactory.CheckoutAnimation(ActionInfo.AnimationInfo.AnimationId))
                ,
                ChildComposers = new List<CompositionLink<CompositionNode>>()
                {
                   new CompositionLink<CompositionNode>(AllignmentPosition.Interaction, AllignmentPosition.Interaction,
                        new InteractionTargetComposer()
                        {
                            Target = Target,
                            Caster = DeferredOwner,
                            InteractionSolver = mInteractionSolver
                        }
                    )
                }
            };

            return composition;
        }

        public override ActionComposition Compose(Target target)
        {
            return base.Compose(target);
        }
    }
}
