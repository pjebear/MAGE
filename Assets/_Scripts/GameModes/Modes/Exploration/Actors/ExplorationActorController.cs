using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

enum ControlType
{
    TopDown,
    ThirdPerson,

    NUM
}

class ExplorationActorController : MonoBehaviour, IInputHandler
{
    [SerializeField] private float m_moveSpeed = 2;
    [SerializeField] private float m_turnSpeed = 200;
    [SerializeField] private float m_jumpForce = 4;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Rigidbody m_rigidBody;

    public bool IsEnabled = true;
   
    private float mForwardInput;
    private float mHorizontalInput;
    private bool mJumpInput;
    public ControlType mControlType = ControlType.TopDown;

    private readonly float m_interpolation = 10;
    private readonly float m_walkScale = 0.33f;
    private readonly float m_backwardsWalkScale = 0.16f;
    private readonly float m_backwardRunScale = 0.66f;

    private Vector3 m_currentDirection = Vector3.zero;


    void Start()
    {
        InputManager.Instance.RegisterHandler(this, false);
    }

    void OnDestroy()
    {
        InputManager.Instance.ReleaseHandler(this);
    }

    private void Update()
    {
        if (mControlType == ControlType.ThirdPerson)
        {
            DirectUpdate();
        }
        else if (mControlType == ControlType.TopDown)
        {
            TopDownUpdate();
        }

        ResetInputs();
    }

    private void ResetInputs()
    {
        mForwardInput = 0;
        mHorizontalInput = 0;
        mJumpInput = false;
    }

    public void Disable()
    {
        IsEnabled = false;
        ResetInputs();
    }

    public void Enable()
    {
        IsEnabled = true;
    }

    public string HandlerName()
    {
        return "PlayerController";
    }

    public void OnMouseHoverChange(GameObject mouseHover)
    {
       // empty
    }

    public void OnMouseScrolled(float delta)
    {
        // empty
    }

    public void OnKeyPressed(InputSource source, int key, InputState state)
    {
        if (!IsEnabled) return;

        if (source == InputSource.Keyboard)
        {
            switch ((KeyCode)key)
            {
                case KeyCode.W:
                    mForwardInput = state == InputState.Up ? 0 : 1f;
                    break;
                case KeyCode.S:
                    mForwardInput = state == InputState.Up ? 0 : -1f;
                    break;
                case KeyCode.A:
                    mHorizontalInput = state == InputState.Up ? 0 : -1f;
                    break;
                case KeyCode.D:
                    mHorizontalInput = state == InputState.Up ? 0 : 1f;
                    break;
                case KeyCode.Space:
                    mJumpInput = state == InputState.Up ? false : true;
                    break;
            }

        }
    }

    private void DirectUpdate()
    {
        transform.Rotate(Vector3.up, mHorizontalInput * m_turnSpeed);

        transform.position += transform.forward * mForwardInput * m_moveSpeed * Time.deltaTime;
    }

    void TopDownUpdate()
    {
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        Vector3 actorForward = cameraForward * mForwardInput + cameraRight * mHorizontalInput;
        if (actorForward.sqrMagnitude > 0)
        {
            actorForward.Normalize();
            transform.position += actorForward * m_moveSpeed * Time.deltaTime;
            
            //transform.forward = Vector3.Lerp(transform.forward, actorForward, m_interpolation);
        }
    }
}

