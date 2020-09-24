using MAGE.GameModes.FlowControl;
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

namespace MAGE.GameModes.Exploration
{
    class RoamFlowControl
        : FlowControlBase
    {
        private string TAG = "RoamFlowControl";
        private float mMinInteractionDistance = 5;
        private float mDistanceToHovered = 0;
        private ThirdPersonActorController mExplorationActor = null;
        private PropBase mHoveredInteractable = null;
        private Collider mHoveredCollider = null;

        private InteractionFlowControl mInteractionFlowControl;

        public override FlowControlId GetFlowControlId()
        {
            return FlowControlId.ExplorationRoamFlowControl;
        }

        protected override void Setup()
        {
            mExplorationActor = ExplorationModel.Instance.PartyAvatar;
        }

        protected override void Cleanup()
        {
            UIManager.Instance.SetCursor(CursorControl.CursorType.Default);
        }

        // IInputHandler
        public override void OnKeyPressed(InputSource source, int key, InputState state)
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
                        ExplorationModel.Instance.InteractionTarget = mHoveredInteractable;
                        SendFlowMessage("interact");
                    }
                }
                break;
            }
        }

        public override void OnMouseHoverChange(GameObject mouseHover)
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
        
        public override void OnMouseScrolled(float scrollDelta)
        {
            // empty
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
    }
}


