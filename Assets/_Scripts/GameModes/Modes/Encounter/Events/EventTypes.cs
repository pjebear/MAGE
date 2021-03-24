using MAGE.GameModes.Combat;
using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Encounter
{

    enum ActionEventType
    {
        Animation,
        Interaction,
        StateChange,
        SpawnProjectile,
        SpawnParticleEffect,

        NUM
    }

    abstract class ActionEvent : ITimelineEvent
    {
        public int StartPointOffset { get; set; }
        public int DurationFrames { get; set; }

        protected ActionEvent(int startPointOffset, int durationFrames)
        {
            StartPointOffset = startPointOffset;
            DurationFrames = durationFrames;
        }

        protected ActionEvent()
        {

        }

        public abstract void Trigger();
    }

    class AnimationEvent : ActionEvent
    {
        public ActorAnimator BeingAnimated;
        public AnimationInfo Animation;
        public Transform FocusTarget;

        public AnimationEvent(ActorAnimator actor, AnimationInfo animation, Transform focusTarget)
            : base(0, animation.NumFrames)
        {
            BeingAnimated = actor;
            Animation = animation;
            FocusTarget = focusTarget;
        }

        public override void Trigger()
        {
            BeingAnimated.Trigger(Animation.TriggerName);
            if (FocusTarget != null)
            {
                BeingAnimated.transform.LookAt(FocusTarget);
            }
        }
    }

    class StateChangeEvent : ActionEvent
    {
        public CombatTarget HavingStateChanged;
        public StateChange StateChange;

        public StateChangeEvent(CombatTarget target, StateChange stateChange)
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

    class AnimationEvent_Deprecated : ActionEvent
    {
        public CharacterActorController BeingAnimated;
        public AnimationInfo Animation;
        public Transform FocusTarget;

        public AnimationEvent_Deprecated(CharacterActorController actor, AnimationInfo animation, int startPointOffset, Transform focusTarget)
            : base(startPointOffset, animation.NumFrames)
        {
            BeingAnimated = actor;
            Animation = animation;
            FocusTarget = focusTarget;
        }

        public override void Trigger()
        {
            EncounterFlowControl_Deprecated.AnimationDirector.AnimateActor(BeingAnimated, Animation);

            if (FocusTarget != null)
            {
                float rotationDuration = AnimationConstants.SECONDS_PER_FRAME * Animation.SyncedFrame;
                EncounterFlowControl_Deprecated.AnimationDirector.RotateActorTowards(BeingAnimated, FocusTarget, rotationDuration);
            }
        }
    }

    class AudioEvent : ActionEvent
    {
        public AudioSource AudioSource;
        public SFXId Audio;

        public AudioEvent(AudioSource source, SFXId audio, int startPointOffset)
            : base(startPointOffset, 0)
        {
            AudioSource = source;
            Audio = audio;
        }

        public override void Trigger()
        {
            AudioSource.PlayOneShot(AudioManager.Instance.GetSFXClip(Audio));
        }
    }

    class StateChangeEvent_Deprecated : ActionEvent
    {
        public Character HavingStateChanged;
        public StateChange StateChange;

        public StateChangeEvent_Deprecated(Character character, StateChange stateChange, int startPointOffset)
            : base(startPointOffset, 0)
        {
            HavingStateChanged = character;
            StateChange = stateChange;
        }

        public override void Trigger()
        {
            //EncounterFlowControl_Deprecated.CharacterDirector.CharacterActorLookup[HavingStateChanged].DisplayStateChange(StateChange);
        }
    }

    class ProjectileSpawnEvent : ActionEvent
    {
        public ProjectileSpawnParams SpawnParams;

        public ProjectileSpawnEvent(ProjectileSpawnParams spawnParams, int startPointOffset)
            : base(startPointOffset, AnimationConstants.FRAMES_IN_DURATION(spawnParams.FlightDuration))
        {
            SpawnParams = spawnParams;
        }

        public override void Trigger()
        {
            ProjectileController projectile = GameObject.Instantiate(UnityEngine.Resources.Load<GameObject>("Props/Projectiles/" + SpawnParams.ProjectileId.ToString())).GetComponent<ProjectileController>();
            projectile.transform.position = SpawnParams.SpawnPoint;

            projectile.Init(SpawnParams.InitialForward, SpawnParams.InitialVelocity, SpawnParams.PathType == ProjectilePathType.Arc, SpawnParams.FlightDuration);
        }
    }

    class EffectSpawnEvent : ActionEvent
    {
        public EffectType EffectType;
        public SFXId SFXId;
        public Transform AtLocation;

        public EffectSpawnEvent(EffectType type, SFXId sfxId, int startPointOffset, int duration, Transform parent)
            : base(startPointOffset, duration)
        {
            EffectType = type;
            AtLocation = parent;
            SFXId = sfxId;
        }

        public override void Trigger()
        {
            GameObject effect = GameObject.Instantiate(UnityEngine.Resources.Load<GameObject>("VFX/" + EffectType.ToString()));
            effect.transform.position = AtLocation.position;
            if (SFXId != SFXId.INVALID)
            {
                AudioClip clip = AudioManager.Instance.GetSFXClip(SFXId);
                effect.gameObject.AddComponent<AudioSource>().PlayOneShot(clip);
            }
        }
    }

    abstract class TimelineBlock<T> : ISynchronizable where T : ITimelineEvent
    {
        public struct Link
        {
            public TimelineBlock<T> child;
            public AllignmentPosition parentAllignment;
            public AllignmentPosition childAllignment;
            public int offset;
        }

        public List<T> Events = new List<T>();

        public SyncPoint Parent { get; set; }

        public List<Link> Children = new List<Link>();

        public int NumFrames { get; set; }

        public int SyncedFrame { get; set; }

        public void SyncronizeTo(AllignmentPosition allignPos, int offset, ISynchronizable syncronizeTo, AllignmentPosition toAllign)
        {
            //Parent = SyncPoint.Syncronize(syncronizeTo, toAllign, this, allignPos);
            //int startOffset = Parent.GetAbsoluteOffset(AllignmentPosition.Start);
            //foreach (T timelineEvent in Events)
            //{
            //    timelineEvent.StartPointOffset += startOffset;
            //}
        }

        public void SyncronizeAndAppend(TimelineBlock<T> timelineBlock, AllignmentPosition beingAddedsAllignementPosition, int offset, AllignmentPosition alignmentPositionOfThis)
        {
            timelineBlock.SyncronizeTo(beingAddedsAllignementPosition, offset, this, alignmentPositionOfThis);
            Events.AddRange(timelineBlock.Events);
        }

        public void AddLink(TimelineBlock<T> child, AllignmentPosition childAllignment, AllignmentPosition parentAllignment, int offset)
        {
            Link link = new Link();
            link.child = child;
            link.childAllignment = childAllignment;
            link.parentAllignment = parentAllignment;
            link.offset = offset;
            Children.Add(link);
        }

        public List<T> GetEvents()
        {
            List<T> events = new List<T>(Events);
            foreach (Link link in Children)
            {
                int offset = SyncPoint.GetStartPointOffset(this, link.parentAllignment, link.child, link.childAllignment, link.offset);
                List<T> linkEvents = link.child.GetEvents();
                foreach (T linkEvent in linkEvents)
                {
                    linkEvent.StartPointOffset += offset;
                }
                events.AddRange(linkEvents);
            }

            return events;
        }
    }

    abstract class ActionEventBlock : TimelineBlock<ActionEvent>
    {
    }

    class EmptyBlock : ActionEventBlock
    {

    }

    class AnimationBlock : ActionEventBlock
    {
        public AnimationBlock(AnimationEvent animationEvent)
        {
            Events.Add(animationEvent);
            SyncedFrame = animationEvent.Animation.SyncedFrame;
            NumFrames = animationEvent.Animation.NumFrames;
        }
    }

    class StateChangeBlock : ActionEventBlock
    {
        public StateChangeBlock(StateChangeEvent stateChangeEvent)
        {
            Events.Add(stateChangeEvent);
            SyncedFrame = 0;
            NumFrames = 0;
        }
    }

    class ActorInteractionBlock : ActionEventBlock
    {
        private CharacterActorController owner;
        private AnimationId animationId;
        private Transform lookAt;
        private object empty;

        public ActorInteractionBlock(CharacterActorController actorController, AnimationId animationId, Transform lookAt, StateChange stateChange)
        {
            AnimationInfo animation = AnimationFactory.CheckoutAnimation(animationId);
            NumFrames = animation.NumFrames;
            SyncedFrame = animation.SyncedFrame;

            Events.Add(new AnimationEvent_Deprecated(actorController, animation, 0, lookAt));
            if (animation.SFXId != SFXId.INVALID)
            {
                Events.Add(new AudioEvent(actorController.GetComponent<AudioSource>(), animation.SFXId, 0));
            }
            if (stateChange.Type != StateChangeType.None)
            {
                Events.Add(new StateChangeEvent_Deprecated(actorController.Character, stateChange, animation.SyncedFrame));
            }
        }

        public ActorInteractionBlock(CharacterActorController owner, AnimationId animationId, Transform lookAt, object empty)
        {
            this.owner = owner;
            this.animationId = animationId;
            this.lookAt = lookAt;
            this.empty = empty;
        }
    }

    class ProjectileSpawnBlock : ActionEventBlock
    {
        public ProjectileSpawnBlock(ProjectileSpawnParams spawnParams)
        {
            NumFrames = AnimationConstants.FRAMES_IN_DURATION(spawnParams.FlightDuration);
            SyncedFrame = NumFrames;

            Events.Add(new ProjectileSpawnEvent(spawnParams, 0));
        }
    }

    class EffectSpawnBlock : ActionEventBlock
    {
        public EffectSpawnBlock(EffectType effectType, SFXId sfxId, Transform location)
        {
            NumFrames = 90;
            SyncedFrame = 30;

            Events.Add(new EffectSpawnEvent(effectType, sfxId, 0, NumFrames, location));
        }
    }
}
