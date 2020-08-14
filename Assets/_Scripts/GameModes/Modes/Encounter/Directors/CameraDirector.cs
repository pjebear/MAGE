using UnityEngine;
using System.Collections.Generic;

class CameraDirector : MonoBehaviour, IInputHandler
{
    private Transform m_currentTarget;

    [SerializeField]
    private Vector3 mTargetOffset = new Vector3(0,0,10);

    public float MaxFOV = 30;
    public float MinFOV = 10;
    public float FOV = 20;
    public float ZoomSensitivity = 2;

    public float Rotation = 0;
    public float RotationSensitivity = 1;

    public float CameraHeight = 10;
    public float CameraHeightSensitivity = .5f;
    public float MaxCameraHeight = 13;
    public float MinCameraHeight = 7;

    private int m_currentIndex;

    private bool mIsScrollWheelHeld;
    private Vector2 mCursorPosition;

    private void Awake()
    {
        MAGE.Input.InputManager.Instance.RegisterHandler(this, false);
    }

    private void OnDestroy()
    {
        MAGE.Input.InputManager.Instance.ReleaseHandler(this);
    }

    private void Update()
    {
        if (mIsScrollWheelHeld)
        {
            Vector2 mousePosition = Input.mousePosition;

            float deltaX = mousePosition.x - mCursorPosition.x;
            Rotation += deltaX * RotationSensitivity;

            float deltaY = mousePosition.y - mCursorPosition.y;

            CameraHeight += deltaY * CameraHeightSensitivity;
            if (CameraHeight > MaxCameraHeight) CameraHeight = MaxCameraHeight;
            if (CameraHeight < MinCameraHeight) CameraHeight = MinCameraHeight;

            mCursorPosition = mousePosition;
        }
    }

    private void LateUpdate()
    {
        if(m_currentTarget == null) { return; }

        Camera.main.fieldOfView = FOV;

        Vector3 offset = mTargetOffset;
        offset.y += CameraHeight;

        Quaternion currentRot = Quaternion.Euler(0, Rotation, 0);

        transform.position = m_currentTarget.position + currentRot * offset;
        transform.LookAt(m_currentTarget.position + new Vector3(0, mTargetOffset.y, 0));
    }

    public void FocusTarget(Transform target)
    {
        m_currentTarget = target;
    }

    public void ClearFocus()
    {
        m_currentTarget = null;
    }

    public void Zoom(float direction)
    {
        FOV += -direction * ZoomSensitivity;
        if (FOV > MaxFOV) FOV = MaxFOV;
        if (FOV < MinFOV) FOV = MinFOV;
    }

    // Input
    public void OnMouseHoverChange(GameObject mouseHover)
    {
        // empty
    }

    public void OnKeyPressed(InputSource source, int key, InputState state)
    {
        if(source == InputSource.Mouse && key == (int)MouseKey.Middle )
        {
            if (state == InputState.Down)
            {
                mCursorPosition = Input.mousePosition;
            }
            else if (state == InputState.Held)
            {
                mIsScrollWheelHeld = true;
            }
            else if (state == InputState.Up)
            {
                mIsScrollWheelHeld = false;
            }
        }
    }

    public void OnMouseScrolled(float scrollDelta)
    {
        Zoom(scrollDelta);
    }
}
