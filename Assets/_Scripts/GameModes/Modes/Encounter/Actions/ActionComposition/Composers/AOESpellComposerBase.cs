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
    abstract class AOESpellComposerBase : ActionComposerBase
    {
        public ConcreteVar<CombatEntity> Caster = new ConcreteVar<CombatEntity>();
        public ConcreteVar<Vector3> EffectSpawnPoint = new ConcreteVar<Vector3>();

        public TargetingSolver TargetingSolver = new TargetingSolver();

        public AOESpellComposerBase(CombatEntity owner) : base (owner)
        {
            
        }

        protected abstract IDeferredVar<StateChange> GetStateChange();

        protected override InteractionSolverBase PopulateInteractionSolver()
        {
            SpellInteractionSolver spellInteractionSolver = new SpellInteractionSolver();
            spellInteractionSolver.Attacker = Caster;
            spellInteractionSolver.Target = TargetingSolver;
            spellInteractionSolver.StateChange = GetStateChange();

            return spellInteractionSolver;
        }

        public override ActionComposition Compose(Target target)
        {
            Caster.Set(Owner);

            if (ActionInfo.EffectRange.AreaType == AreaType.Cone)
            {
                EffectSpawnPoint.Set(Owner.transform.position);
            }
            else
            {
                EffectSpawnPoint.Set(target.GetTargetPoint());
            }

            TargetingSolver.Targeting = Caster;
            TargetingSolver.BeingTargeted = new TargetSelection(target, ActionInfo.EffectRange);
            TargetingSolver.Solve();

            return base.Compose(target);
        }

        protected override CompositionNode PopulateComposition()
        {
            return new AnimationComposer()
            {
                // AnimationConstructor
                ToAnimate = new MonoConversion<CombatEntity, ActorAnimator>(Caster),
                AnimationTarget = TargetingSolver,
                AnimationInfo = new ConcreteVar<AnimationInfo>(AnimationFactory.CheckoutAnimation(ActionInfo.AnimationInfo.AnimationId)),

                ChildComposers = new List<CompositionLink<CompositionNode>>()
                {
                    new CompositionLink<CompositionNode>(AllignmentPosition.Interaction, AllignmentPosition.Start,
                    new ParticleEffectComposer()
                    {
                        SpawnPoint = EffectSpawnPoint,
                        Parent = new ConcreteVar<Transform>(Owner.transform),
                        EffectType = ActionInfo.EffectInfo.EffectId,
                        ChildComposers = new List<CompositionLink<CompositionNode>>()
                        {
                            new CompositionLink<CompositionNode>(AllignmentPosition.Interaction, AllignmentPosition.Start,
                            new MultiTargetComposer()
                            {
                                Targeting = TargetingSolver
                                , InteractionSolver = mInteractionSolver
                                , ChainedComposition = new CompositionLink<CompositionNode>(AllignmentPosition.Interaction, AllignmentPosition.Interaction,
                                new AnimationComposer()
                                {
                                    ToAnimate = new MonoConversion<CombatTarget, ActorAnimator>(TargetingSolver),
                                    AnimationTarget = new MonoConversion<CombatEntity, CombatTarget>(Caster),
                                    AnimationInfo = new InteractionResultToAnimation(mInteractionSolver.InteractionResult)

                                    , ChildComposers = new List<CompositionLink<CompositionNode>>()
                                    {
                                        new CompositionLink<CompositionNode>(AllignmentPosition.Interaction, AllignmentPosition.Interaction,
                                        new StateChangeComposer()
                                        {
                                            Target = TargetingSolver,
                                            StateChange = mInteractionSolver.InteractionResult
                                        })
                                    }
                                })
                            })
                        }
                    })
                }
            };
        }
    }
}
