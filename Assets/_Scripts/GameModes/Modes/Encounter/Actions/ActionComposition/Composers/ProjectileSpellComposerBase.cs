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
    abstract class ProjectileSpellComposerBase : ActionComposerBase
    {
        public ConcreteVar<Target> PreviousTarget = new ConcreteVar<Target>();
        public ConcreteVar<Target> CurrentTarget = new ConcreteVar<Target>();

        protected ProjectileSpellComposerBase(CombatEntity owner) : base(owner)
        {

        }

        protected override CompositionNode PopulateComposition()
        {
            return new AnimationComposer()
            {
                // AnimationConstructor
                ToAnimate = new DeferredMonoConversion<CombatEntity, ActorAnimator>(DeferredOwner),
                AnimationTarget = new DeferredTargetPosition(mTargetingSolver),
                AnimationInfo = new ConcreteVar<AnimationInfo>(AnimationFactory.CheckoutAnimation(GameSystems.AnimationId.Cast)),

                ChildComposers = new List<CompositionLink<CompositionNode>>()
                {
                    new CompositionLink<CompositionNode>(AllignmentPosition.Interaction, AllignmentPosition.Start,
                        new MultiTargetComposer()
                        {
                            Targeting = mTargetingSolver
                            , PreviousTarget = PreviousTarget
                            , CurrentTarget = CurrentTarget

                            , ChainedComposition = new CompositionLink<CompositionNode>(AllignmentPosition.Interaction, AllignmentPosition.Start,
                                new ProjectileSpawnComposer()
                                {
                                    CasterPosition = new DeferredTargetPosition(PreviousTarget),
                                    Target = CurrentTarget,
                                    ProjectileType = ProjectileId.FireBall,

                                    ChildComposers = new List<CompositionLink<CompositionNode>>()
                                    {
                                        new CompositionLink<CompositionNode>(AllignmentPosition.Interaction, AllignmentPosition.Interaction,
                                            new InteractionTargetComposer()
                                            {
                                                 Caster = DeferredOwner,
                                                 Target = CurrentTarget,
                                                 InteractionSolver = mInteractionSolver
                                            }
                                        )
                                    }
                                }
                            )
                        }
                    )
                }
            };
        }

        protected override InteractionSolverBase CreateInteractionSolver()
        {
            return new SpellInteractionSolver();
        }

        protected override void InitInteractionSolver(InteractionSolverBase interactionSolverBase)
        {
            SpellEffectivenessCalculator damageCalculator = new SpellEffectivenessCalculator();
            damageCalculator.BaseEffectiveness = ActionInfo.Effectiveness;
            damageCalculator.DeferredCaster = new DeferredMonoConversion<CombatEntity, StatsControl>(DeferredOwner);
            damageCalculator.IsBeneficial = false;

            DeferredStateChange deferredStateChange = new DeferredStateChange();
            deferredStateChange.HealthChange = damageCalculator;

            interactionSolverBase.StateChange = deferredStateChange;
        }

        public override ActionComposition Compose(Target target)
        {
            PreviousTarget.Set(new Target(Owner.GetComponent<CombatTarget>()));

            return base.Compose(target);
        }
    }
}
