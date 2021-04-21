using MAGE.GameModes.Combat;
using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Items;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Encounter
{
    class FireBallComposer : ActionComposerBase
    {
        public ConcreteVar<CombatEntity> Caster = new ConcreteVar<CombatEntity>();
        public ConcreteVar<CombatTarget> Target = new ConcreteVar<CombatTarget>();

        public FireBallComposer(CombatEntity owner) : base(owner)
        {
            Caster.Set(owner);
        }

        protected override ActionInfo PopulateActionInfo()
        {
            ActionInfo actionInfo = new ActionInfo();

            actionInfo.ActionCost = new StateChange(StateChangeType.ActionCost, 0, 0);
            actionInfo.ActionRange = ActionRange.Projectile;
            actionInfo.ActionSource = ActionSource.Cast;
            actionInfo.AnimationInfo.AnimationId = AnimationId.Cast;
            actionInfo.ProjectileInfo.ProjectileId = ProjectileId.FireBall;

            actionInfo.CastRange = new RangeInfo()
            {
                AreaType = AreaType.Circle,
                MaxRange = 5,
                TargetingType = TargetingType.Enemies
            };

            actionInfo.EffectRange = new RangeInfo()
            {
                AreaType = AreaType.Circle,
                MaxRange = 1,
                TargetingType = TargetingType.Any
            };

            return actionInfo;
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
            interactionSolver.Target = Target;
            interactionSolver.StateChange = deferredStateChange;

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
                ChildComposers = new List<CompositionLink<CompositionNode>>() {
                    new CompositionLink<CompositionNode>(AllignmentPosition.Interaction, AllignmentPosition.Start,
                        new ProjectileSpawnComposer()
                        {
                            Caster = new DeferredTransform<CombatEntity>(Caster),
                            Target = new DeferredTransform<CombatTarget>(Target),
                            ProjectileType = ActionInfo.ProjectileInfo.ProjectileId,

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
                        })
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
