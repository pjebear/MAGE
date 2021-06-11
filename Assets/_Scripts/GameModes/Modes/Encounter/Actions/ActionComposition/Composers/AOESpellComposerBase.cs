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
        public ConcreteVar<Vector3> EffectSpawnPoint = new ConcreteVar<Vector3>();

        public AOESpellComposerBase(CombatEntity owner) : base (owner)
        {
            
        }

        protected abstract IDeferredVar<StateChange> GetStateChange();

        protected override InteractionSolverBase PopulateInteractionSolver()
        {
            SpellInteractionSolver spellInteractionSolver = new SpellInteractionSolver();
            spellInteractionSolver.StateChange = GetStateChange();

            return spellInteractionSolver;
        }

        public override ActionComposition Compose(Target target)
        {
            if (ActionInfo.EffectRange.AreaType == AreaType.Cone)
            {
                EffectSpawnPoint.Set(Owner.transform.position);
            }
            else
            {
                EffectSpawnPoint.Set(target.GetTargetPoint());
            }

            return base.Compose(target);
        }

        protected virtual CompositionNode GetPerTargetComposition()
        {
            return new InteractionTargetComposer()
            {
                Caster = DeferredOwner,
                Target = mTargetingSolver,
                InteractionSolver = mInteractionSolver
            };
        }

        protected virtual CompositionNode GetFocalPointComposition()
        {
            return new ParticleEffectComposer()
            {
                SpawnPoint = EffectSpawnPoint,
                Parent = new ConcreteVar<Transform>(Owner.transform),
                EffectType = ActionInfo.EffectInfo.EffectId,
            };
        }

        protected override CompositionNode PopulateComposition()
        {
            CompositionNode rootComposition = new AnimationComposer()
            {
                // AnimationConstructor
                ToAnimate = new DeferredMonoConversion<CombatEntity, ActorAnimator>(DeferredOwner),
                AnimationTarget = new DeferredTargetPosition(mTargetingSolver),
                AnimationInfo = new ConcreteVar<AnimationInfo>(AnimationFactory.CheckoutAnimation(ActionInfo.AnimationInfo.AnimationId))
            };

            CompositionNode focalComposition = GetFocalPointComposition();
            rootComposition.ChildComposers.Add(new CompositionLink<CompositionNode>(AllignmentPosition.Interaction, AllignmentPosition.Start, focalComposition));

            focalComposition.ChildComposers.Add(new CompositionLink<CompositionNode>(AllignmentPosition.Interaction, AllignmentPosition.Start,
                new MultiTargetComposer()
                {
                    Targeting = mTargetingSolver,
                    ChainedComposition = new CompositionLink<CompositionNode>(AllignmentPosition.Interaction, AllignmentPosition.Interaction,
                    GetPerTargetComposition()
                    )
                })
            );

            return rootComposition;
        }
    }
}
