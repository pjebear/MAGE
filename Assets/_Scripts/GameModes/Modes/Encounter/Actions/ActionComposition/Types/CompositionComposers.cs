using MAGE.GameModes.Combat;
using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
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
    class MultiTargetComposer : CompositionNode
    {
        public ConcreteVar<Target> PreviousTarget = new ConcreteVar<Target>();
        public ConcreteVar<Target> CurrentTarget = new ConcreteVar<Target>();
        public TargetingSolverBase Targeting;
        public int OffsetBetweenChildren = 0;

        public CompositionLink<CompositionNode> ChainedComposition;

        protected override CompositionElement OnCompose()
        {
            return new EmptyElement();
        }

        public override List<TimelineElement> Compose()
        {
            List<TimelineElement> chainTimeline = new List<TimelineElement>();

            int accumulatedOffset = 0;
            while (Targeting.HasTarget())
            {
                CurrentTarget.Set(Targeting.Get());

                mComposition = OnCompose();

                List<TimelineElement> childLinkTimeline = ChainedComposition.Child.Compose();

                int childOffset = SyncPoint.GetStartPointOffset(mComposition
                    , ChainedComposition.ParentAllignment
                    , ChainedComposition.Child.mComposition
                    , ChainedComposition.ChildAllignment
                    , 0);

                foreach (TimelineElement timelineElement in childLinkTimeline)
                {
                    timelineElement.StartPointOffset += accumulatedOffset + childOffset;
                }

                chainTimeline.AddRange(childLinkTimeline);

                accumulatedOffset += SyncPoint.GetStartPointOffset(ChainedComposition.Child.mComposition, ChainedComposition.ParentAllignment, 0);

                PreviousTarget.Set(CurrentTarget.Get());

                Targeting.Next();
            }

            return chainTimeline;
        }
    }

    class AnimationComposer : CompositionNode
    {
        public IDeferredVar<ActorAnimator> ToAnimate;
        public IDeferredVar<AnimationInfo> AnimationInfo;
        public IDeferredVar<Vector3> AnimationTarget;

        protected override CompositionElement OnCompose()
        {
            CompositionElement toReturn = null;

            ActorAnimator toAnimate = ToAnimate?.Get()?.GetComponent<ActorAnimator>();
            AnimationInfo animationInfo = AnimationInfo?.Get();

            if (toAnimate != null)
            {
                toReturn = new AnimationElement(toAnimate, animationInfo, AnimationTarget.Get());
            }
            else
            {
                toReturn = new EmptyElement();
            }

            return toReturn;
        }
    }

    class StateChangeComposer : CompositionNode
    {
        public IDeferredVar<Target> Target;
        public IDeferredVar<StateChange> StateChange;

        protected override CompositionElement OnCompose()
        {
            CompositionElement toReturn = null;

            Target target = Target.Get();
            if (Target.Get().FocalTarget != null)
            {
                toReturn = new StateChangeElement(Target.Get().FocalTarget, StateChange.Get());
            }
            else
            {
                toReturn = new EmptyElement();
            }

            return toReturn;
        }
    }

    class InteractionTargetComposer : CompositionNode
    {
        public IDeferredVar<CombatEntity> Caster;
        public IDeferredVar<Target> Target;
        public InteractionSolverBase InteractionSolver;

        protected override CompositionElement OnCompose()
        {
            CompositionElement animationComposition = null;

            if (Target.Get().FocalTarget != null)
            {
                ActorAnimator toAnimate = Target.Get().FocalTarget.GetComponent<ActorAnimator>();

                InteractionSolver.Solve(Caster.Get(), Target.Get());
                InteractionResult interactionResult = InteractionSolver.InteractionResult.Get();
                Transform target = Caster?.Get()?.transform;

                AnimationId animationId = AnimationUtil.InteractionResultTypeToAnimationId(interactionResult);
                AnimationInfo animationInfo = AnimationFactory.CheckoutAnimation(animationId);

                if (toAnimate != null)
                {
                    animationComposition = new AnimationElement(toAnimate, animationInfo, target.position);
                }
                else
                {
                    animationComposition = new EmptyElement();
                }

                CompositionElement stateChangeComposition = new InteractionResultElement(Target.Get().FocalTarget, interactionResult);

                animationComposition.AddLink(stateChangeComposition, AllignmentPosition.Interaction, AllignmentPosition.Interaction);
            }
            else
            {
                animationComposition = new EmptyElement();
            }



            return animationComposition;
        }
    }

    class ProjectileSpawnComposer : CompositionNode
    {
        public IDeferredVar<Vector3> CasterPosition;
        public IDeferredVar<Target> Target;
        public ProjectileId ProjectileType = ProjectileId.Arrow;

        protected override CompositionElement OnCompose()
        {
            CompositionElement toReturn = null;

            Vector3 casterPos = CasterPosition.Get();
            Vector3 targetPos = Target.Get().GetTargetPoint();
            if (targetPos != Vector3.zero)
            {
                ProjectileSpawnParams projectileSpawnParams = ProjectileUtil.GenerateLinearProjectileParams(
                    casterPos,
                    targetPos);
                projectileSpawnParams.ProjectileId = ProjectileType;

                toReturn = new ProjectileSpawnElement(projectileSpawnParams);
            }
            else
            {
                toReturn = new EmptyElement();
            }

            return toReturn;
        }
    }

    class ParticleEffectComposer : CompositionNode
    {
        public IDeferredVar<Vector3> SpawnPoint;
        public IDeferredVar<Transform> Parent;
        public EffectType EffectType = EffectType.FlameStrike;

        protected override CompositionElement OnCompose()
        {
            CompositionElement toReturn = null;

            toReturn = new EffectSpawnElement(EffectType, SFXId.INVALID, SpawnPoint.Get(), Parent.Get());

            return toReturn;
        }
    }

    class SpawnComposer : CompositionNode
    {
        public IDeferredVar<Vector3> SpawnPoint;
        public ActionSummonInfo SummonInfo = null;
        public SummonHeirarchy Owner = null;

        protected override CompositionElement OnCompose()
        {
            CompositionElement toReturn = null;

            toReturn = new SpawnEntityElement(SpawnPoint.Get(), SummonInfo, Owner);

            return toReturn;
        }
    }
}
