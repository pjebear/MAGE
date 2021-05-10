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
    class DefendComposer : ActionComposerBase
    {
        public ConcreteVar<CombatEntity> Caster = new ConcreteVar<CombatEntity>();
        public ConcreteVar<CombatTarget> Target = new ConcreteVar<CombatTarget>();

        public DefendComposer(CombatEntity owner) : base(owner)
        {
            Caster.Set(owner);
        }

        protected override ActionInfo PopulateActionInfo()
        {
            ActionInfo actionInfo = new ActionInfo();

            actionInfo.ActionId = ActionId.Defend;
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

        protected override InteractionSolverBase PopulateInteractionSolver()
        {
            SpellInteractionSolver interactionSolver = new SpellInteractionSolver();
            interactionSolver.Attacker = Caster;
            interactionSolver.Target = Target;
            interactionSolver.StateChange = new ConcreteVar<StateChange>(new StateChange(StateChangeType.ActionTarget, StatusEffectFactory.CheckoutStatusEffect(StatusEffectId.Defend)));

            return interactionSolver;
        }

        protected override CompositionNode PopulateComposition()
        {
            CompositionNode composition = new AnimationComposer()
            {
                // AnimationConstructor
                ToAnimate = new MonoConversion<CombatEntity, ActorAnimator>(Caster)
                ,
                AnimationTarget = Target
                ,
                AnimationInfo = new ConcreteVar<AnimationInfo>(AnimationFactory.CheckoutAnimation(ActionInfo.AnimationInfo.AnimationId))
                ,
                ChildComposers = new List<CompositionLink<CompositionNode>>()
                {
                   new CompositionLink<CompositionNode>(AllignmentPosition.Interaction, AllignmentPosition.Interaction,
                        new InteractionTargetComposer()
                        {
                            Target = Target,
                            Caster = Caster,
                            InteractionResult = mInteractionSolver.InteractionResult
                        }
                    )
                }
            };

            return composition;
        }

        public override ActionComposition Compose(Target target)
        {
            Target.Set(target.FocalTarget);
            mInteractionSolver.Solve();

            return base.Compose(target);
        }
    }
}
