using MAGE.GameServices;
using MAGE.GameServices.Character;
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

        public abstract void Trigger();
    }

    class AnimationEvent : ActionEvent
    {
        public EncounterActorController BeingAnimated;
        public AnimationInfo Animation;
        public Transform FocusTarget;

        public AnimationEvent(EncounterActorController actor, AnimationInfo animation, int startPointOffset, Transform focusTarget)
            : base(startPointOffset, animation.NumFrames)
        {
            BeingAnimated = actor;
            Animation = animation;
            FocusTarget = focusTarget;
        }

        public override void Trigger()
        {
            EncounterModule.AnimationDirector.AnimateActor(BeingAnimated, Animation);

            if (FocusTarget != null)
            {
                float rotationDuration = AnimationConstants.SECONDS_PER_FRAME * Animation.SyncedFrame;
                EncounterModule.AnimationDirector.RotateActorTowards(BeingAnimated, FocusTarget, rotationDuration);
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
            AudioSource.PlayOneShot(GameModesModule.AudioManager.GetSFXClip(Audio));
        }
    }

    class StateChangeEvent : ActionEvent
    {
        public EncounterCharacter HavingStateChanged;
        public StateChange StateChange;

        public StateChangeEvent(EncounterCharacter actor, StateChange stateChange, int startPointOffset)
            : base(startPointOffset, 0)
        {
            HavingStateChanged = actor;
            StateChange = stateChange;
        }

        public override void Trigger()
        {
            EncounterModule.CharacterDirector.CharacterActorLookup[HavingStateChanged].DisplayStateChange(StateChange);
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
            EncounterModule.ProjectileDirector.SpawnProjectile(SpawnParams);
        }
    }

    class EffectSpawnEvent : ActionEvent
    {
        public EffectInfo Info;

        public EffectSpawnEvent(EffectInfo info, int startPointOffset)
            : base(startPointOffset, info.NumFrames)
        {
            Info = info;
        }

        public override void Trigger()
        {
            EncounterModule.EffectSpawner.SpawnEffect(Info);
        }
    }

    abstract class TimelineBlock<T> : ISynchronizable where T : ITimelineEvent
    {
        public List<T> Events = new List<T>();

        public SyncPoint Parent { get; set; }

        public int NumFrames { get; set; }

        public int SyncedFrame { get; set; }

        public void SyncronizeTo(AllignmentPosition allignPos, int offset, ISynchronizable syncronizeTo, AllignmentPosition toAllign)
        {
            Parent = SyncPoint.Syncronize(syncronizeTo, toAllign, this, allignPos, offset);
            int startOffset = Parent.GetAbsoluteOffset(AllignmentPosition.Start);
            foreach (T timelineEvent in Events)
            {
                timelineEvent.StartPointOffset += startOffset;
            }
        }
    }

    class ActorInteractionBlock : TimelineBlock<ActionEvent>
    {
        private EncounterActorController owner;
        private AnimationId animationId;
        private Transform lookAt;
        private object empty;

        public ActorInteractionBlock(EncounterActorController actorController, AnimationId animationId, Transform lookAt, StateChange stateChange)
        {
            AnimationInfo animation = AnimationFactory.CheckoutAnimation(animationId);
            NumFrames = animation.NumFrames;
            SyncedFrame = animation.SyncedFrame;

            Events.Add(new AnimationEvent(actorController, animation, 0, lookAt));
            if (animation.SFXId != SFXId.INVALID)
            {
                Events.Add(new AudioEvent(actorController.Actor.AudioSource, animation.SFXId, 0));
            }
            if (stateChange.Type != StateChangeType.None)
            {
                Events.Add(new StateChangeEvent(actorController.EncounterCharacter, stateChange, animation.SyncedFrame));
            }
        }

        public ActorInteractionBlock(EncounterActorController owner, AnimationId animationId, Transform lookAt, object empty)
        {
            this.owner = owner;
            this.animationId = animationId;
            this.lookAt = lookAt;
            this.empty = empty;
        }
    }

    class ProjectileSpawnBlock : TimelineBlock<ActionEvent>
    {
        public ProjectileSpawnBlock(ProjectileSpawnParams spawnParams)
        {
            NumFrames = AnimationConstants.FRAMES_IN_DURATION(spawnParams.FlightDuration);
            SyncedFrame = NumFrames;

            Events.Add(new ProjectileSpawnEvent(spawnParams, 0));
        }
    }

    class EffectSpawnBlock : TimelineBlock<ActionEvent>
    {
        public EffectSpawnBlock(EffectType effectType, Transform location)
        {
            EffectInfo effectInfo = EffectFactory.GetEffectPlaceholder(effectType, location);
            NumFrames = effectInfo.NumFrames;
            SyncedFrame = effectInfo.SyncedFrame;

            Events.Add(new EffectSpawnEvent(effectInfo, 0));
        }
    }
}
