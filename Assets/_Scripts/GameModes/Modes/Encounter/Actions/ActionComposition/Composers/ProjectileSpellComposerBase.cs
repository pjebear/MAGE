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
        public ConcreteVar<CombatEntity> Caster = new ConcreteVar<CombatEntity>();

        public TargetingSolver TargetingSolver = new TargetingSolver();

        public ConcreteVar<CombatTarget> PreviousTarget = new ConcreteVar<CombatTarget>();
        public ConcreteVar<CombatTarget> CurrentTarget = new ConcreteVar<CombatTarget>();

        protected ProjectileSpellComposerBase(CombatEntity owner) : base(owner)
        {
            Caster.Set(owner);
        }

        protected override CompositionNode PopulateComposition()
        {
            return new AnimationComposer()
            {
                // AnimationConstructor
                ToAnimate = new MonoConversion<CombatEntity, ActorAnimator>(Caster),
                AnimationTarget = TargetingSolver,
                AnimationInfo = new ConcreteVar<AnimationInfo>(AnimationFactory.CheckoutAnimation(GameSystems.AnimationId.Cast)),

                ChildComposers = new List<CompositionLink<CompositionNode>>()
                {
                    new CompositionLink<CompositionNode>(AllignmentPosition.Interaction, AllignmentPosition.Start,
                        new MultiTargetComposer()
                        {
                            Targeting = TargetingSolver
                            , InteractionSolver = mInteractionSolver
                            , PreviousTarget = PreviousTarget
                            , CurrentTarget = CurrentTarget

                            , ChainedComposition = new CompositionLink<CompositionNode>(AllignmentPosition.Interaction, AllignmentPosition.Start,
                                new ProjectileSpawnComposer()
                                {
                                    Caster = new DeferredTransform<CombatTarget>(PreviousTarget),
                                    Target = new DeferredTransform<CombatTarget>(CurrentTarget),
                                    ProjectileType = ProjectileId.FireBall,

                                    ChildComposers = new List<CompositionLink<CompositionNode>>()
                                    {
                                        new CompositionLink<CompositionNode>(AllignmentPosition.Interaction, AllignmentPosition.Interaction,
                                            new AnimationComposer()
                                            {
                                                ToAnimate = new MonoConversion<CombatTarget, ActorAnimator>(TargetingSolver),
                                                AnimationTarget = new MonoConversion<CombatEntity, CombatTarget>(Caster),
                                                AnimationInfo = new InteractionResultToAnimation(mInteractionSolver.InteractionResult)
                                            }
                                        )
                                        , new CompositionLink<CompositionNode>(AllignmentPosition.Interaction, AllignmentPosition.Interaction,
                                            new StateChangeComposer()
                                            {
                                                Target = TargetingSolver,
                                                StateChange = mInteractionSolver.InteractionResult
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

        protected override InteractionSolverBase PopulateInteractionSolver()
        {
            SpellInteractionSolver interactionSolver = new SpellInteractionSolver();

            SpellEffectivenessCalculator damageCalculator = new SpellEffectivenessCalculator();
            damageCalculator.BaseEffectiveness = ActionInfo.Effectiveness;
            damageCalculator.DeferredCaster = new MonoConversion<CombatEntity, StatsControl>(Caster);
            damageCalculator.IsBeneficial = false;

            DeferredStateChange deferredStateChange = new DeferredStateChange();
            deferredStateChange.HealthChange = damageCalculator;

            interactionSolver.Attacker = Caster;
            interactionSolver.Target = TargetingSolver;
            interactionSolver.StateChange = deferredStateChange;

            return interactionSolver;
        }

        public override ActionComposition Compose(Target target)
        {
            TargetingSolver.Targeting = Caster;
            TargetingSolver.BeingTargeted = new TargetSelection(target, ActionInfo.EffectRange);

            TargetingSolver.Solve();
            PreviousTarget.Set(Caster.Get().GetComponent<CombatTarget>());

            return base.Compose(target);
        }
    }
}
