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
    class RegenComposer : ActionComposerBase
    {
        public ConcreteVar<CombatEntity> Caster = new ConcreteVar<CombatEntity>();
        public ConcreteVar<Transform> EffectSpawnPoint = new ConcreteVar<Transform>();

        public StatusEffect RegenEffect = new RegenEffect();

        public ConcreteVar<StateChange> StateChange = new ConcreteVar<StateChange>();

        public TargetingSolver TargetingSolver = new TargetingSolver();
        public SpellInteractionSolver InteractionSolver = new SpellInteractionSolver();
    
        private CompositionNode mRootComposition = null;

        public RegenComposer(CombatEntity owner) : base (owner)
        {
            mRootComposition = new AnimationComposer()
            {
                // AnimationConstructor
                ToAnimate = new MonoConversion<CombatEntity, ActorAnimator>(Caster),
                AnimationTarget = TargetingSolver,
                AnimationInfo = new ConcreteVar<AnimationInfo>(AnimationFactory.CheckoutAnimation(GameSystems.AnimationId.Cast)),

                ChildComposers = new List<CompositionLink<CompositionNode>>()
                {
                    new CompositionLink<CompositionNode>(AllignmentPosition.Start, AllignmentPosition.Start,
                    new ParticleEffectComposer()
                    {
                        SpawnPoint = EffectSpawnPoint,
                        EffectType = EffectType.Regen,
                        ChildComposers = new List<CompositionLink<CompositionNode>>()
                        {
                            new CompositionLink<CompositionNode>(AllignmentPosition.Interaction, AllignmentPosition.Start,
                            new MultiTargetComposer()
                            {
                                Targeting = TargetingSolver
                                , InteractionSolver = InteractionSolver
                                , ChainedComposition = new CompositionLink<CompositionNode>(AllignmentPosition.Interaction, AllignmentPosition.Interaction,
                                new AnimationComposer()
                                {
                                    ToAnimate = new MonoConversion<CombatTarget, ActorAnimator>(TargetingSolver),
                                    AnimationTarget = new MonoConversion<CombatEntity, CombatTarget>(Caster),
                                    AnimationInfo = new InteractionResultToAnimation(InteractionSolver.InteractionResult)
                                            
                                    , ChildComposers = new List<CompositionLink<CompositionNode>>()
                                    {
                                        new CompositionLink<CompositionNode>(AllignmentPosition.Interaction, AllignmentPosition.Interaction,
                                        new StateChangeComposer()
                                        {
                                            Target = TargetingSolver,
                                            StateChange = InteractionSolver.InteractionResult
                                        })
                                    }
                                })
                            })
                        }
                    })
                }
            };
        }

        protected override ActionInfo PopulateActionInfo()
        {
            ActionInfo actionInfo = new ActionInfo();

            actionInfo.ActionCost = new StateChange(StateChangeType.ActionCost, 0, 0);
            actionInfo.ActionRange = ActionRange.Projectile;
            actionInfo.ActionSource = ActionSource.Cast;

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

        public override ActionComposition Compose(Target target)
        {
            Caster.Set(Owner);

            TargetingSolver.Targeting = Caster;
            TargetingSolver.BeingTargeted = new TargetSelection(target, ActionInfo.EffectRange);
            EffectSpawnPoint.Set(target.GetTargetPoint());

            StateChange = new ConcreteVar<StateChange>(new GameSystems.Stats.StateChange(StateChangeType.ActionTarget,
                StatusEffectFactory.CheckoutStatusEffect(StatusEffectId.Regen)));

            InteractionSolver.Attacker = Caster;
            InteractionSolver.Target = TargetingSolver;
            InteractionSolver.StateChange = StateChange;

            TargetingSolver.Solve();

            ActionComposition actionComposition = new ActionComposition();
            actionComposition.ActionTimeline = mRootComposition.Compose();
            actionComposition.ActionResults = new GameModes.Combat.ActionResult(
               Owner,
               ActionInfo,
               new InteractionResult(InteractionUtil.GetOwnerResultTypeFromResults(InteractionSolver.Results.Values.ToList()), ActionInfo.ActionCost),
               InteractionSolver.Results);

            return actionComposition;
        }
    }
}
