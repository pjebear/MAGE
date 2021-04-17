using MAGE.GameModes.Exploration;
using MAGE.GameSystems;
using MAGE.Messaging;
using MAGE.UI;
using MAGE.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace MAGE.GameModes.SceneElements
{
    class CinematicMoment
        : MonoBehaviour
        , IMessageHandler
    {
        public CinematicId CinematicId;

        private PlayableDirector mCinematic;
        private CinematicMomentTriggerVolume mTriggerVolume;
        private Transform rPartyAvatarInCinematic;
        private bool mIsCinematicReady = false;

        private void Awake()
        {
            mCinematic = GetComponentInChildren<PlayableDirector>(true);
            mTriggerVolume = GetComponentInChildren<CinematicMomentTriggerVolume>(true);

            EnableHeirarchy(false);
        }

        private void Start()
        {
            Messaging.MessageRouter.Instance.RegisterHandler(this);
        }

        void OnDestroy()
        {
            Messaging.MessageRouter.Instance.UnRegisterHandler(this);
        }

        public bool IsCinematicReady()
        {
            return mIsCinematicReady;
        }

        public Transform GetPartyAvatarInScene()
        {
            return rPartyAvatarInCinematic;
        }

        public void CinematicEnabled()
        {
            if (mTriggerVolume == null)
            {
                CinematicTriggered();
            }
            else
            {
                mTriggerVolume.gameObject.SetActive(true);
            }
        }

        public void CinematicTriggered()
        {
            mIsCinematicReady = true;
            LevelManagement.LevelMessage cinematicAvailableMessage = new LevelManagement.LevelMessage(LevelManagement.MessageType.CinematicAvailable, this);
            Messaging.MessageRouter.Instance.NotifyMessage(cinematicAvailableMessage);
        }

        public void Skip()
        {
            mCinematic.time = mCinematic.duration;
        }

        public void Play()
        {
            mIsCinematicReady = false;
            mCinematic.gameObject.SetActive(true);

            int partyAvatarId = WorldService.Get().GetPartyAvatarId();
            rPartyAvatarInCinematic = GetComponentsInChildren<CharacterPickerControl>().ToList().Find(x => x.CharacterPicker.GetCharacterId() == partyAvatarId)?.transform;

            TimelineAsset timeline = mCinematic.playableAsset as TimelineAsset;
            foreach (PlayableBinding binding in timeline.outputs)
            {
                if (binding.sourceObject is AnimationTrack)
                {
                    AnimationTrack animationTrack = binding.sourceObject as AnimationTrack;

                    UnityEngine.Object o = mCinematic.GetGenericBinding(animationTrack);
                    Animator animator = o as Animator;
                    if (animator != null)
                    {
                        GameObject go = animator.gameObject;
                        if (go.tag == "Model")
                        {
                            ActorSpawner actorSpawner = go.GetComponentInParent<ActorSpawner>();
                            actorSpawner.Refresh();
                            Animator actorAnimator = actorSpawner.GetComponentInChildren<Animator>(true);
                            mCinematic.SetGenericBinding(animationTrack, actorAnimator);

                            Animator hopefulAnimator = (mCinematic.GetGenericBinding(animationTrack) as Animator);
                        }
                    }
                    
                }
                else if (binding.sourceObject is CinemachineTrack)
                {
                    CinemachineTrack cinemachineTrack = binding.sourceObject as CinemachineTrack;
                    mCinematic.SetGenericBinding(cinemachineTrack, Camera.main.GetComponent<Cinemachine.CinemachineBrain>());
                }
                else if (binding.sourceObject is CinematicDialogueTrack)
                {
                    CinematicDialogueTrack dialogueTrack = binding.sourceObject as CinematicDialogueTrack;
                    foreach (TimelineClip clip in dialogueTrack.GetClips())
                    {
                        CinematicDialogueClip dialogueClip = clip.asset as CinematicDialogueClip;
                        if (dialogueClip != null)
                        {
                            
                        }
                    }
                }
            }

            mCinematic.stopped += OnCinematicComplete;
            mCinematic.Play();
        }

        void OnCinematicComplete(PlayableDirector playableDirector)
        {
            LevelManagement.LevelMessage message = new LevelManagement.LevelMessage(LevelManagement.MessageType.CinematicComplete, this);
            Messaging.MessageRouter.Instance.NotifyMessage(message);
            EnableHeirarchy(false);
        }

        public void HandleMessage(MessageInfoBase eventInfoBase)
        {
            if (eventInfoBase.MessageId == LevelManagement.LevelMessage.Id)
            {
                LevelManagement.LevelMessage levelMessage = eventInfoBase as LevelManagement.LevelMessage;
                if (levelMessage.Type == LevelManagement.MessageType.CinematicUpdated)
                {
                    int cinematicId = levelMessage.Arg<int>();
                    if (cinematicId == (int)CinematicId)
                    {
                        if (LevelManagementService.Get().GetCinematicInfo((int)CinematicId).IsActive)
                        {
                            CinematicEnabled();
                        }
                        else
                        {
                            EnableHeirarchy(false);
                        }
                    }
                }
                else if (levelMessage.Type == LevelManagement.MessageType.LevelLoaded)
                {
                    if (LevelManagementService.Get().GetCinematicInfo((int)CinematicId).IsActive)
                    {
                        CinematicEnabled();
                    }
                    else
                    {
                        EnableHeirarchy(false);
                    }
                }
            }
                
        }

        private void EnableHeirarchy(bool enable)
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                transform.GetChild(i).gameObject.SetActive(enable);
            }
        }
    }
}
