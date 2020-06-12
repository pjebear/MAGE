using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Invector;
using UnityEngine;

class ThirdPersonCamera 
    : MonoBehaviour
    , IInputHandler
{
    public enum CameraState
    {
        Follow,
        Interact,

        NUM
    }

    // 
    [SerializeField] private Vector3 mFollowTargetOffset = new Vector3(0, 2, -5);
    [SerializeField] private Vector3 mFollowFocusOffset = new Vector3(0, 1.5f, 5);
    [SerializeField] private float mFollowFOV = 60;
    [SerializeField] private Vector3 mInteractTargetOffset = new Vector3(2, 2, -1);
    [SerializeField] private Vector3 mInteractFocusOffset = new Vector3(0, 1.5f, 0);
    [SerializeField] private float mInteractFOV = 60;

    private Transform mTarget = null;
    private Transform mFocus = null;

    [SerializeField] private Vector3 mTargetOffset = Vector3.zero;
    [SerializeField] private Vector3 mFocusOffset = Vector3.zero;
    [SerializeField] private float mFOV;

    void OnDestroy()
    {
        transform.SetParent(null);
    }

    void FixedUpdate()
    {
        if (mTarget != null)
        {
            transform.position = mTarget.position 
                + mTarget.forward * mTargetOffset.z
                + mTarget.up * mTargetOffset.y 
                + mTarget.right * mTargetOffset.x;

            Vector3 focusPosition =
                 mFocus.transform.position
                + mFocus.forward * mFocusOffset.z
                + mFocus.up * mFocusOffset.y
                + mFocus.right * mFocusOffset.x;

            transform.LookAt(focusPosition);

            Camera.main.fieldOfView = mFOV;
        }
    }

    public void Follow(Transform target)
    {
        mTarget = target;

        mFocus = target;
        mFOV = mFollowFOV;
        mTargetOffset = mFollowTargetOffset;
        mFocusOffset = mFollowFocusOffset;
    }

    public void Interact(Transform interacting, Transform with)
    {
        mTarget = interacting;
        mFocus = with;
        mFOV = mInteractFOV;
        mTargetOffset = mInteractTargetOffset;
        mFocusOffset = mInteractFocusOffset;
    }

        #region IInputHandler
        // IInputHandler
        public void OnKeyPressed(InputSource source, int key, InputState state)
    {
        if (source == InputSource.Keyboard)
        {
            int inputValue = state == InputState.Up ? 0 : 1;

            switch ((KeyCode)key)
            {
                case (KeyCode.A):
                {
                    
                }
                break;
                case (KeyCode.D):
                {
                    
                }
                break;
            }
        }
    }

    public void OnMouseHoverChange(GameObject mouseHover)
    {
        // empty
    }

    public void OnMouseScrolled(float scrollDelta)
    {
        // empty 
    }
    #endregion // IInputHandler
}

