using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
    public EncounterCharacter BeingAnimated;
    public AnimationPlaceholder Animation;
    public Transform FocusTarget;

    public AnimationEvent(EncounterCharacter actor, AnimationPlaceholder animation, int startPointOffset, Transform focusTarget)
        : base(startPointOffset, animation.NumFrames)
    {
        BeingAnimated = actor;
        Animation = animation;
        FocusTarget = focusTarget;
    }

    public override void Trigger()
    {
        EncounterActorController controller = EncounterModule.CharacterDirector.GetController(BeingAnimated);
        EncounterModule.AnimationDirector.AnimateActor(controller, Animation);

        if (FocusTarget != null)
        {
            float rotationDuration = AnimationConstants.SECONDS_PER_FRAME * Animation.SyncedFrame;
            EncounterModule.AnimationDirector.RotateActorTowards(controller, FocusTarget, rotationDuration);
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
    public EffectPlaceholder Info;

    public EffectSpawnEvent(EffectPlaceholder info, int startPointOffset)
        : base(startPointOffset, info.NumFrames)
    {
        Info = info;
    }

    public override void Trigger()
    {
        EncounterModule.EffectSpawner.SpawnEffect(Info);
    }
}