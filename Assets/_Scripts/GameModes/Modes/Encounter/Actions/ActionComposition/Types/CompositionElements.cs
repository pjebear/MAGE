using MAGE.GameModes.Combat;
using MAGE.GameModes.SceneElements;
using MAGE.GameModes.SceneElements.Encounters;
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
        public Vector3 FocusTarget;

        public AnimationElement(ActorAnimator actor, AnimationInfo animation, Vector3 focusTarget)
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
            if (FocusTarget != Vector3.zero)
            {
                BeingAnimated.transform.LookAt(FocusTarget);
            }
        }
    }

    class SpawnEntityElement : CompositionElement
    {
        public Vector3 SpawnPoint;
        public ActionSummonInfo SummonInfo = null;
        public SummonHeirarchy Owner = null;

        public SpawnEntityElement(Vector3 spawnPoint, ActionSummonInfo summonInfo, SummonHeirarchy owner)
            : base(0, 0)
        {
            SpawnPoint = spawnPoint;
            Owner = owner;
            SummonInfo = summonInfo;
        }

        public override void Trigger()
        {
            GameObject summon = null;
            switch (SummonInfo.SummonType)
            {
                case SummonType.Bear:
                {
                    summon = (GameObject.Instantiate(UnityEngine.Resources.Load("Props/ActorSpawner/CombatCharacter"), Owner.transform.parent) as GameObject);
                    MobCharacterControl control = summon.AddComponent<MobCharacterControl>();
                    control.MobId = GameSystems.Mobs.MobId.DEMO_Bear;
                }
                break;

                case SummonType.ScorchedEarth:
                {
                    summon = (GameObject.Instantiate(UnityEngine.Resources.Load("EncounterPrefabs/Summons/ScorchedEarth"), Owner.transform.parent) as GameObject);
                }
                break;

                case SummonType.Tree:
                {
                    summon = (GameObject.Instantiate(UnityEngine.Resources.Load("EncounterPrefabs/Summons/Sprout"), Owner.transform.parent) as GameObject);
                }
                break;
            }
             
            summon.transform.position = SpawnPoint;

            SummonHeirarchy summonsControl = summon.GetComponent<SummonHeirarchy>();
            if (summonsControl != null)
            {
                Owner.Summons.Add(summonsControl);
                summonsControl.Owner = Owner;
            }
        }
    }

    class InteractionResultElement : CompositionElement
    {
        public CombatTarget Target;
        public InteractionResult Result;

        public InteractionResultElement(CombatTarget target, InteractionResult result)
            : base(0, 0)
        {
            Target = target;
            Result = result;
        }

        public override void Trigger()
        {
            Target.ApplyInteractionResult(Result);
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
            : base(0, 0)
        {
            if (sfxId != SFXId.INVALID)
            {
                SyncedFrame = 30;
                NumFrames = 90;
            }
            EffectType = type;
            AtPosition = atPosition;
            SFXId = sfxId;
            Parent = parent;
        }

        public override void Trigger()
        {
            if (EffectType != EffectType.INVALID)
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
    }
}
