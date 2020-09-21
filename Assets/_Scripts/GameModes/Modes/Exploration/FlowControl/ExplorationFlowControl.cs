using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using MAGE.UI;
using MAGE.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.FlowControl
{
    class ExplorationFlowControl
        : MonoBehaviour,
        IInputHandler,
        Messaging.IMessageHandler
    {
        enum FlowState
        {
            Setup,
            Exploring,
            Interacting,
            Cinematic,

            NUM
        }
        private FlowState mState = FlowState.Setup;

        private string TAG = "ExplorationFlowControl";
        private float mMinInteractionDistance = 5;
        private float mDistanceToHovered = 0;
        private ThirdPersonActorController mExplorationActor = null;
        private PropBase mHoveredInteractable = null;
        private Collider mHoveredCollider = null;

        private CinematicMoment mCinematicMoment;
        private InteractionViewControl mInteractionFlowControl;

        public void Init(ThirdPersonActorController exploring)
        {
            Messaging.MessageRouter.Instance.RegisterHandler(this);

            mExplorationActor = exploring;
            mInteractionFlowControl = new InteractionViewControl();
            mInteractionFlowControl.Init(mExplorationActor);
        }

        public void BeginFlow()
        {
            Input.InputManager.Instance.RegisterHandler(this, false);

            UpdateState();
        }

        void UpdateState()
        {
            // Check for cinematics
            Level currentLevel = LevelManagementService.Get().GetLoadedLevel();
            foreach (CinematicMoment cinematicMoment in currentLevel.CinematicContainer.GetComponentsInChildren<CinematicMoment>())
            {
                if (cinematicMoment.CinematicReady)
                {
                    mCinematicMoment = cinematicMoment;
                }
            }

            if (mCinematicMoment != null)
            {
                BeginCinematic();
                return;
            }

            // Check for encounters
            foreach (EncounterContainer encounter in currentLevel.EncounterContainer.GetComponentsInChildren<EncounterContainer>())
            {
                if (encounter.IsEncounterPending)
                {
                    WorldService.Get().PrepareEncounter(new EncounterCreateParams() { ScenarioId = encounter.EncounterScenarioId});
                    GameModesModule.Instance.Encounter();
                    return;
                }
            }

            SetState(FlowState.Exploring);
        }

        public void CleanUp()
        {
            Messaging.MessageRouter.Instance.UnRegisterHandler(this);
            Input.InputManager.Instance.ReleaseHandler(this);
        }

        private void SetState(FlowState flowState)
        {
            mState = flowState;
            mExplorationActor.Enable(mState == FlowState.Exploring);
        }

        //! IInputHandler
        //! IInputHandler
        public void OnKeyPressed(InputSource source, int key, InputState state)
        {
            if (mState == FlowState.Exploring)
            {
                switch (source)
                {
                    case InputSource.Mouse:
                    {
                        if (key == (int)MouseKey.Right
                            && state == InputState.Down
                            && mHoveredInteractable != null
                            && IsInRange()
                            && mHoveredInteractable.IsPropInteractable())
                        {
                            mInteractionFlowControl.BeginInteraction(mHoveredInteractable);
                        }
                    }
                    break;
                }
            }
        }

        void Update()
        {
            if (mHoveredInteractable != null)
            {
                bool wasPreviousInRange = IsInRange();
                mDistanceToHovered = DistanceToHoverTarget();
                bool isNowInRange = IsInRange();

                if (wasPreviousInRange != isNowInRange)
                {
                    UpdateHoverTargetCursor();
                }
            }
        }

        public void OnMouseHoverChange(GameObject mouseHover)
        {
            if (mState == FlowState.Exploring)
            {
                if (mouseHover != null)
                {
                    mHoveredInteractable = mouseHover.GetComponentInParent<PropBase>();
                    if (mHoveredInteractable != null)
                    {
                        mHoveredCollider = mouseHover.GetComponent<Collider>();
                        mDistanceToHovered = DistanceToHoverTarget();
                    }
                }
                else
                {
                    mHoveredInteractable = null;
                }

                UpdateHoverTargetCursor();
            }
        }

        public void OnMouseScrolled(float scrollDelta)
        {
            // empty
        }

        private void UpdateHoverTargetCursor()
        {
            if (mHoveredInteractable != null && mHoveredInteractable.IsPropInteractable())
            {
                float distanceTo = DistanceToHoverTarget();
                if (distanceTo < mMinInteractionDistance)
                {
                    UIManager.Instance.SetCursor(CursorControl.CursorType.Interact_Near);
                }
                else
                {
                    UIManager.Instance.SetCursor(CursorControl.CursorType.Interact_Far);
                }
            }
            else
            {
                UIManager.Instance.SetCursor(CursorControl.CursorType.Default);
            }

        }

        float DistanceToHoverTarget()
        {
            return (mExplorationActor.transform.position - mHoveredCollider.gameObject.transform.position).magnitude;
        }

        bool IsInRange()
        {
            return mDistanceToHovered < mMinInteractionDistance;
        }

        private void BeginCinematic()
        {
            SetState(FlowState.Cinematic);
            mExplorationActor.gameObject.SetActive(false);
            mCinematicMoment.Play();
        }

        public void HandleMessage(Messaging.MessageInfoBase messageInfoBase)
        {
            switch (messageInfoBase.MessageId)
            {
                case Exploration.ExplorationMessage.Id:
                {
                    Exploration.ExplorationMessage message = messageInfoBase as Exploration.ExplorationMessage;

                    switch (message.Type)
                    {
                        case (Exploration.ExplorationMessage.EventType.InteractionEnd):
                        {
                            UpdateState();
                        }
                        break;
                    }
                }
                break;

                case LevelManagement.LevelMessage.Id:
                {
                    LevelManagement.LevelMessage message = messageInfoBase as LevelManagement.LevelMessage;

                    switch (message.Type)
                    {
                        case (LevelManagement.MessageType.CinematicAvailable):
                        {
                            mCinematicMoment = message.Arg<CinematicMoment>();
                            if (mState == FlowState.Exploring)
                            {
                                BeginCinematic();
                            }
                        }
                        break;

                        case (LevelManagement.MessageType.EncounterAvailable):
                        {
                            WorldService.Get().PrepareEncounter(new EncounterCreateParams() { ScenarioId = message.Arg<EncounterContainer>().EncounterScenarioId });
                            GameModesModule.Instance.Encounter();
                        }
                        break;

                        case (LevelManagement.MessageType.CinematicComplete):
                        {
                            mExplorationActor.gameObject.SetActive(true);
                            if (mCinematicMoment.PartyAvatarInCinematic != null)
                            {
                                Vector3 position = mCinematicMoment.PartyAvatarInCinematic.transform.position;
                                Quaternion rotation = mCinematicMoment.PartyAvatarInCinematic.transform.rotation;

                                mExplorationActor.transform.SetPositionAndRotation(position, rotation);
                            }

                            mCinematicMoment = null;

                            UpdateState();
                        }
                        break;
                    }
                }
                break;
            }
        }
    }
}


