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
    interface IComposer
    {
        CompositionElement Compose();
    }

    class TimelineElement : ITimelineEvent
    {
        public int StartPointOffset { get; set; }
        public int DurationFrames { get; set; }

        public CompositionElement CompositionElement;
        public void Trigger()
        {
            CompositionElement.Trigger();
        }
    }

    abstract class CompositionElement : ISynchronizable
    {
        public int SyncedFrame { get; set; }
        public int NumFrames { get; set; }

        public List<CompositionLink<CompositionElement>> Children = new List<CompositionLink<CompositionElement>>();

        protected CompositionElement(int syncedFrame, int durationFrames)
        {
            SyncedFrame = syncedFrame;
            NumFrames = durationFrames;
        }

        protected CompositionElement()
        {

        }

        public void AddLink(CompositionElement child, AllignmentPosition childAllignment, AllignmentPosition parentAllignment)
        {
            CompositionLink<CompositionElement> link = new CompositionLink<CompositionElement>(parentAllignment, childAllignment, child);
            link.Child = child;
            link.ChildAllignment = childAllignment;
            link.ParentAllignment = parentAllignment;
            Children.Add(link);
        }

        public abstract void Trigger();
    }

    class EmptyElement : CompositionElement
    {
        public override void Trigger() { }
    }

    class AnimationElement : CompositionElement
    {
        public ActorAnimator BeingAnimated;
        public AnimationInfo Animation;
        public Transform FocusTarget;

        public AnimationElement(ActorAnimator actor, AnimationInfo animation, Transform focusTarget)
            : base(animation.SyncedFrame, animation.NumFrames)
        {
            BeingAnimated = actor;
            Animation = animation;
            FocusTarget = focusTarget;
        }

        public override void Trigger()
        {
            if (Animation.SFXId != SFXId.INVALID)
            {
                BeingAnimated.GetComponent<AudioSource>().PlayOneShot(AudioManager.Instance.GetSFXClip(Animation.SFXId));
            }

            BeingAnimated.Trigger(Animation.TriggerName);
            if (FocusTarget != null)
            {
                BeingAnimated.transform.LookAt(FocusTarget);
            }
        }
    }

    class StateChangeElement : CompositionElement
    {
        public CombatTarget HavingStateChanged;
        public StateChange StateChange;

        public StateChangeElement(CombatTarget target, StateChange stateChange)
            : base(0, 0)
        {
            HavingStateChanged = target;
            StateChange = stateChange;
        }

        public override void Trigger()
        {
            HavingStateChanged.ApplyStateChange(StateChange);
        }
    }
    
    class ProjectileSpawnElement : CompositionElement
    {
        public ProjectileSpawnParams SpawnParams;

        public ProjectileSpawnElement(ProjectileSpawnParams spawnParams)
            : base(AnimationConstants.FRAMES_IN_DURATION(spawnParams.FlightDuration), AnimationConstants.FRAMES_IN_DURATION(spawnParams.FlightDuration))
        {
            SpawnParams = spawnParams;
        }

        public override void Trigger()
        {
            ProjectileController projectile = GameObject.Instantiate(UnityEngine.Resources.Load<GameObject>("Props/Projectiles/" + SpawnParams.ProjectileId.ToString())).GetComponent<ProjectileController>();
            projectile.transform.position = SpawnParams.SpawnPoint;

            projectile.Init(SpawnParams.InitialForward, SpawnParams.InitialVelocity, SpawnParams.PathType == ProjectilePathType.Arc, SpawnParams.FlightDuration);

            if (SpawnParams.ProjectileId == ProjectileId.Arrow)
            {
                AudioClip clip = AudioManager.Instance.GetSFXClip(SFXId.ArrowRelease);

                if (projectile.GetComponent<AudioSource>() == null)
                {
                    projectile.gameObject.AddComponent<AudioSource>();
                }
                projectile.GetComponent<AudioSource>().PlayOneShot(clip);
            }
        }
    }

    class EffectSpawnElement : CompositionElement
    {
        public EffectType EffectType;
        public SFXId SFXId;
        public Vector3 AtPosition;
        public Transform Parent;

        public EffectSpawnElement(EffectType type, SFXId sfxId, Vector3 atPosition, Transform parent = null)
            : base(30, 90)
        {
            EffectType = type;
            AtPosition = atPosition;
            SFXId = sfxId;
            Parent = parent;
        }

        public override void Trigger()
        {
            GameObject effect = GameObject.Instantiate(UnityEngine.Resources.Load<GameObject>("VFX/" + EffectType.ToString()));
            effect.transform.position = AtPosition;
            
            if (Parent != null)
            {
                effect.transform.rotation = Parent.transform.rotation;
            }

            if (SFXId != SFXId.INVALID)
            {
                AudioClip clip = AudioManager.Instance.GetSFXClip(SFXId);
                effect.gameObject.AddComponent<AudioSource>().PlayOneShot(clip);
            }
        }
    }

    class CompositionLink<T> 
    {
        public T Child;
        public AllignmentPosition ParentAllignment;
        public AllignmentPosition ChildAllignment;

        public CompositionLink(T child)
        {
            Child = child;
            ParentAllignment = AllignmentPosition.Start;
            ChildAllignment = AllignmentPosition.Start;
        }

        public CompositionLink(AllignmentPosition parentAllignment, AllignmentPosition childAllignment, T child)
        {
            Child = child;
            ParentAllignment = parentAllignment;
            ChildAllignment = childAllignment;
        }
    }

    abstract class CompositionNode
    {
        public CompositionElement mComposition = null;
        public List<CompositionLink<CompositionNode>> ChildComposers = new List<CompositionLink<CompositionNode>>();

        protected abstract CompositionElement OnCompose();
        public virtual List<TimelineElement> Compose()
        {
            mComposition = OnCompose();

            List<TimelineElement> timeline = ConvertCompositionElementToTimeline(mComposition);
            timeline.AddRange(ComposeChildren());

            return timeline;
        }

        protected static List<TimelineElement> ConvertCompositionElementToTimeline(CompositionElement compositionElement)
        {
            List<TimelineElement> timeline = new List<TimelineElement>();

            TimelineElement rootElement = new TimelineElement();
            rootElement.CompositionElement = compositionElement;
            rootElement.DurationFrames = compositionElement.NumFrames;
            rootElement.StartPointOffset = 0;
            timeline.Add(rootElement);

            foreach (CompositionLink<CompositionElement> childLink in compositionElement.Children)
            {
                TimelineElement childElement = new TimelineElement();
                childElement.CompositionElement = childLink.Child;
                childElement.DurationFrames = childLink.Child.NumFrames;
                childElement.StartPointOffset = SyncPoint.GetStartPointOffset(rootElement.CompositionElement, childLink.ParentAllignment, childLink.Child, childLink.ChildAllignment, 0);

                timeline.Add(childElement);
            }

            return timeline;
        }


        protected virtual List<TimelineElement> ComposeChildren()
        {
            List<TimelineElement> timeline = new List<TimelineElement>();

            foreach (CompositionLink<CompositionNode> childLink in ChildComposers)
            {
                List<TimelineElement> childTimeline = childLink.Child.Compose();
                int childOffset = SyncPoint.GetStartPointOffset(mComposition, childLink.ParentAllignment, childLink.Child.mComposition, childLink.ChildAllignment, 0);

                foreach (TimelineElement timelineElement in childTimeline)
                {
                    timelineElement.StartPointOffset += childOffset;
                }

                timeline.AddRange(childTimeline);
            }

            return timeline;
        }
    }

    class MultiTargetComposer : CompositionNode
    {
        public ConcreteVar<CombatTarget> PreviousTarget = new ConcreteVar<CombatTarget>();
        public ConcreteVar<CombatTarget> CurrentTarget = new ConcreteVar<CombatTarget>();
        public TargetingSolver Targeting;
        public InteractionSolverBase InteractionSolver;
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

                InteractionSolver.Solve();

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

    class EmptyNode : CompositionNode
    {
        protected override CompositionElement OnCompose()
        {
            return new EmptyElement();
        }
    }

    //class CompositionHeirarchy
    //{
    //    public AllignmentPosition ParentAllignmentPoint = AllignmentPosition.Start;
    //    public AllignmentPosition CompositionAllignmentPoint = AllignmentPosition.Start;
    //    public int Offset = 0;

    //    public IComposer Composer = null;
    //    protected CompositionElement mCompositionElement = new EmptyBlock();
    //    public List<CompositionHeirarchy> Children = new List<CompositionHeirarchy>();

    //    public CompositionHeirarchy()
    //    {
    //        // empty 
    //    }

    //    public CompositionHeirarchy(AllignmentPosition parentAllignmentPoint, AllignmentPosition compositionAllignmentPoint, int offset = 0)
    //    {
    //        ParentAllignmentPoint = parentAllignmentPoint;
    //        CompositionAllignmentPoint = compositionAllignmentPoint;
    //        Offset = offset;
    //    }

    //    public ActionEventBlock Compose()
    //    {
    //        if (Composer != null)
    //        {
    //            mActionEventBlock = Composer.Compose();
    //        }
            
    //        ComposeChildren();

    //        return mActionEventBlock;
    //    }

    //    protected virtual void ComposeChildren()
    //    {
    //        foreach (CompositionHeirarchy compositionElement in Children)
    //        {
    //            ComposeAndAppend(compositionElement);
    //        }
    //    }

    //    protected void ComposeAndAppend(CompositionHeirarchy element)
    //    {
    //        ComposeAndAppend(mActionEventBlock, element);
    //    }

    //    protected void ComposeAndAppend(ActionEventBlock appendTo, CompositionHeirarchy toAppend)
    //    {
    //        appendTo.AddLink(
    //            toAppend.Compose()
    //            , toAppend.CompositionAllignmentPoint
    //            , toAppend.ParentAllignmentPoint
    //            , toAppend.Offset);
    //    }
    //}

    //class MultiTargetCompositionElement : CompositionHeirarchy
    //{
    //    public ConcreteVar<CombatTarget> PreviousTarget = new ConcreteVar<CombatTarget>();
    //    public ConcreteVar<CombatTarget> CurrentTarget = new ConcreteVar<CombatTarget>();
    //    public TargetingSolver Targeting;
    //    public InteractionSolverBase InteractionSolver;
    //    public int OffsetBetweenChildren = 0;

    //    public MultiTargetCompositionElement(AllignmentPosition parentAllignmentPoint, AllignmentPosition compositionAllignmentPoint, int offset = 0)
    //        :base(parentAllignmentPoint, compositionAllignmentPoint, offset)
    //    {

    //    }

    //    protected override void ComposeChildren()
    //    {
    //        int accumulatedOffset = 0;
    //        while (Targeting.HasTarget())
    //        {
    //            CurrentTarget.Set(Targeting.Get());

    //            InteractionSolver.Solve();

    //            foreach (CompositionHeirarchy compositionElement in Children)
    //            {
    //                mActionEventBlock.AddLink(
    //                    compositionElement.Compose()
    //                    , compositionElement.CompositionAllignmentPoint
    //                    , compositionElement.ParentAllignmentPoint
    //                    , compositionElement.Offset + accumulatedOffset);
    //            }

    //            accumulatedOffset += OffsetBetweenChildren;

    //            PreviousTarget.Set(Targeting.Get());

    //            Targeting.Next();
    //        }
    //    }
    //}

    class AnimationComposer : CompositionNode
    {
        public IDeferredVar<ActorAnimator> ToAnimate;
        public IDeferredVar<AnimationInfo> AnimationInfo;
        public IDeferredVar<CombatTarget> AnimationTarget;

        protected override CompositionElement OnCompose()
        {
            CompositionElement toReturn = null;

            ActorAnimator toAnimate = ToAnimate?.Get()?.GetComponent<ActorAnimator>();
            AnimationInfo animationInfo = AnimationInfo?.Get();
            Transform target = AnimationTarget?.Get()?.transform;
            if (toAnimate != null)
            {
                toReturn = new AnimationElement(toAnimate, animationInfo, target);
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
        public IDeferredVar<CombatTarget> Target;
        public IDeferredVar<StateChange> StateChange;

        protected override CompositionElement OnCompose()
        {
            CompositionElement toReturn = null;

            CombatTarget target = Target.Get();
            if (Target != null)
            {
                toReturn = new StateChangeElement(target, StateChange.Get());
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
        public IDeferredVar<CombatTarget> Target;
        public IDeferredVar<InteractionResult> InteractionResult;

        protected override CompositionElement OnCompose()
        {
            CompositionElement animationComposition = null;

            ActorAnimator toAnimate = Target.Get().GetComponent<ActorAnimator>();
            InteractionResult interactionResult = InteractionResult.Get();
            Transform target = Caster?.Get()?.transform;

            AnimationId animationId = AnimationUtil.InteractionResultTypeToAnimationId(interactionResult);
            AnimationInfo animationInfo = AnimationFactory.CheckoutAnimation(animationId);
            
            if (toAnimate != null)
            {
                animationComposition = new AnimationElement(toAnimate, animationInfo, target);
            }
            else
            {
                animationComposition = new EmptyElement();
            }

            CompositionElement stateChangeComposition = new StateChangeElement(Target.Get(), interactionResult.StateChange);

            animationComposition.AddLink(stateChangeComposition, AllignmentPosition.Interaction, AllignmentPosition.Interaction);

            return animationComposition;
        }
    }

    class ProjectileSpawnComposer : CompositionNode
    {
        public IDeferredVar<Transform> Caster;
        public IDeferredVar<Transform> Target;
        public ProjectileId ProjectileType = ProjectileId.Arrow;

        protected override CompositionElement OnCompose()
        {
            CompositionElement toReturn = null;

            Transform caster = Caster.Get();
            Transform target = Target.Get();
            if (Target != null)
            {
                ProjectileSpawnParams projectileSpawnParams = ProjectileUtil.GenerateLinearProjectileParams(
                    caster,
                    target);
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

    

    class TargetingSolver 
        : IDeferredVar<CombatTarget>
    {
        public IDeferredVar<CombatEntity> Targeting;
        public TargetSelection BeingTargeted;
        public List<CombatTarget> Targets = new List<CombatTarget>();
        public int TargetIdx = 0;

        public void Solve()
        {
            if (BeingTargeted.SelectionRange.AreaType == AreaType.Point)
            {
                Debug.Assert(BeingTargeted.FocalTarget.TargetType == TargetSelectionType.Focal);
                Targets = new List<CombatTarget>() { BeingTargeted.FocalTarget.FocalTarget };
            }
            else
            {
                Targets = LevelManagementService.Get().GetLoadedLevel().GetTargetsInRange(Targeting.Get(), BeingTargeted);
            }
        }

        public CombatTarget Get()
        {
            if (TargetIdx < Targets.Count)
            {
                return Targets[TargetIdx];
            }
            else
            {
                return null;
            }
        }

        public bool HasTarget()
        {
            return TargetIdx < Targets.Count;
        }

        public bool Next()
        {
            ++TargetIdx;
            return HasTarget();
        }
    }

    abstract class InteractionSolverBase
    {
        public IDeferredVar<CombatEntity> Attacker;
        public IDeferredVar<CombatTarget> Target;
        public IDeferredVar<StateChange> StateChange;
        public DeferredInteractionResult InteractionResult = new DeferredInteractionResult();

        public Dictionary<CombatTarget, InteractionResult> Results = new Dictionary<CombatTarget, InteractionResult>();

        public abstract void Solve();
    }

    class WeaponInteractionSolver : InteractionSolverBase
    {
        public override void Solve()
        {
            InteractionResult result = InteractionResolver.GetWeaponInteractionResult(Attacker.Get(), Target.Get(), StateChange.Get());
            InteractionUtil.GetOwnerResultTypeFromResults(new List<InteractionResult>() { result });
            Results.Add(Target.Get(), result);
            InteractionResult.Set(result);
        }
    }

    class SpellInteractionSolver : InteractionSolverBase
    {
        public override void Solve()
        {
            InteractionResult result = new InteractionResult(InteractionResultType.Hit, StateChange.Get());
            Results.Add(Target.Get(), result);
            InteractionResult.Set(result);
        }
    }

    class SpellEffectivenessCalculator : IDeferredVar<int>
    {
        public bool IsBeneficial = false;
        public float BaseEffectiveness;
        public IDeferredVar<StatsControl> DeferredCaster;

        public int Get()
        {
            float effectiveness = BaseEffectiveness;
            if (DeferredCaster.Get() != null)
            {
                Attributes attributes = DeferredCaster.Get().Attributes;
                effectiveness += attributes[PrimaryStat.Magic];
                effectiveness *= 1 + (attributes[SecondaryStat.Attunement] / 100);
            }
            if (!IsBeneficial)
            {
                effectiveness *= -1;
            }
            if (IsBeneficial) effectiveness = Mathf.Max(effectiveness, 0);
            else effectiveness = Mathf.Min(effectiveness, 0);

            return (int)effectiveness;
        }
    }

    class DeferredStateChange : IDeferredVar<StateChange>
    {
        public IDeferredVar<int> HealthChange;
        public IDeferredVar<int> ResourceChange;
        public IDeferredVar<List<StatusEffectId>> StatusEffects;

        public StateChange Get()
        {
            int healthChange = 0;
            if (HealthChange != null)
            {
                healthChange = HealthChange.Get();
            }

            int resourceChange = 0;
            if (ResourceChange != null)
            {
                resourceChange = ResourceChange.Get();
            }

            List<StatusEffect> statusEffects = new List<StatusEffect>();
            if (StatusEffects != null)
            {
                foreach (StatusEffectId statusEffectId in StatusEffects.Get())
                {
                    statusEffects.Add(StatusEffectFactory.CheckoutStatusEffect(statusEffectId));
                }
            }

            return new StateChange(StateChangeType.ActionTarget, healthChange, resourceChange, statusEffects);
        }
    }

    class WeaponStateChangeCalculator : IDeferredVar<StateChange>
    {
        public IDeferredVar<CombatEntity> DeferredCombatEntity;
        private StateChange mCachedStateChange = null;

        public StateChange Get()
        {
            if (mCachedStateChange == null)
            {
                mCachedStateChange = StateChange.Empty;

                CombatEntity combatEntity = DeferredCombatEntity.Get();
                UnityEngine.Debug.Assert(combatEntity != null);
                if (combatEntity != null)
                {
                    EquipmentControl attackerEquipment = combatEntity.GetComponent<EquipmentControl>();
                    StatsControl attackerStats = combatEntity.GetComponent<StatsControl>();

                    HeldEquippable heldEquippable = (attackerEquipment.Equipment[Equipment.Slot.RightHand] as HeldEquippable);

                    float baseDamage = 0;
                    foreach (AttributeScalar scalar in heldEquippable.EffectivenessScalars)
                    {
                        baseDamage += scalar.GetScalar(attackerStats.Attributes);
                    }

                    mCachedStateChange.healthChange = -(int)baseDamage;
                }
            }

            return mCachedStateChange;
        }
    }

   
}