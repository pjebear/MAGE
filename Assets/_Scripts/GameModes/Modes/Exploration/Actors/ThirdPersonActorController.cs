using MAGE.GameModes.SceneElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ThirdPersonActorController 
    : MonoBehaviour
{
    public float MovementSpeed = 6f;
    public float TurnSpeed = 6f;
    public float JumpStrength = 6f;
    public float TurnSmoothTime = 0.1f;
    public float GroundDistance = 0.4f;
    public LayerMask GroundMask;

    private CharacterController rCharacterController;

    private float mTurnSmoothVelocity;
    private Vector3 mVelocity;
    private bool mIsGrounded = true;
    private bool mIsJumpPressed = false;

    #region Unity
    private void Awake()
    {
        rCharacterController = gameObject.GetComponent<CharacterController>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        mIsGrounded = Physics.CheckSphere(transform.position, GroundDistance, GroundMask);

        if (mIsGrounded && mVelocity.y < 0)
        {
            mVelocity.y = -2f;
        }

        MAGE.Input.InputManager input = MAGE.Input.InputManager.Instance;
        if (input != null)
        {
            if (mIsGrounded)
            {
                if (!mIsJumpPressed 
                    && (input.GetKeyState(KeyCode.Space) == InputState.Down 
                    || input.GetKeyState(KeyCode.Space) == InputState.Held))
                {
                    mIsJumpPressed = true;
                    mVelocity.y = JumpStrength;
                }
                else if ((input.GetKeyState(KeyCode.Space) == InputState.None
                    || input.GetKeyState(KeyCode.Space) == InputState.Up))
                {
                    mIsJumpPressed = false;
                }
            }

            float horzInput = input.GetAxisInput(InputAxis.Horizontal);
            if (Mathf.Abs(horzInput) > 0.1f)
            {
                float rotationDeg = horzInput * TurnSpeed * Time.deltaTime;
                transform.Rotate(Vector3.up * rotationDeg);
            }

            float vertInput = input.GetAxisInput(InputAxis.Vertical);
            if (Mathf.Abs(vertInput) > 0.1f)
            {
                float movementSpeed = MovementSpeed * vertInput;
                if (vertInput < 0)
                {
                    movementSpeed /= 2;
                }

                Vector3 forwardMovement = transform.forward;
                forwardMovement.y = 0;
                rCharacterController.Move(forwardMovement.normalized * movementSpeed * Time.deltaTime);

                GetComponent<ActorAnimator>().SetCurrentSpeed(movementSpeed);
            }
            else
            {
                GetComponent<ActorAnimator>().SetCurrentSpeed(0);
            }
        }

        mVelocity.y += Physics.gravity.y * Time.deltaTime;
        rCharacterController.Move(mVelocity * Time.deltaTime);

    }
    #endregion //Unity

    public void Enable(bool enabled)
    {
        this.enabled = enabled;
    }
}

//namespace Invector.vCharacterController
//{
//    public class vThirdPersonController : vThirdPersonAnimator
//    {
//        public virtual void ControlAnimatorRootMotion()
//        {
//            if (!this.enabled) return;

//            if (inputSmooth == Vector3.zero)
//            {
//                transform.position = animator.rootPosition;
//                transform.rotation = animator.rootRotation;
//            }

//            if (useRootMotion)
//                MoveCharacter(moveDirection);
//        }

//        public virtual void ControlLocomotionType()
//        {
//            if (lockMovement) return;

//            if (locomotionType.Equals(LocomotionType.FreeWithStrafe) && !isStrafing || locomotionType.Equals(LocomotionType.OnlyFree))
//            {
//                SetControllerMoveSpeed(freeSpeed);
//                SetAnimatorMoveSpeed(freeSpeed);
//            }
//            else if (locomotionType.Equals(LocomotionType.OnlyStrafe) || locomotionType.Equals(LocomotionType.FreeWithStrafe) && isStrafing)
//            {
//                isStrafing = true;
//                SetControllerMoveSpeed(strafeSpeed);
//                SetAnimatorMoveSpeed(strafeSpeed);
//            }

//            if (!useRootMotion)
//                MoveCharacter(moveDirection);
//        }

//        public virtual void ControlRotationType()
//        {
//            if (lockRotation) return;

//            bool validInput = input != Vector3.zero || (isStrafing ? strafeSpeed.rotateWithCamera : freeSpeed.rotateWithCamera);

//            if (validInput)
//            {
//                // calculate input smooth
//                inputSmooth = Vector3.Lerp(inputSmooth, input, (isStrafing ? strafeSpeed.movementSmooth : freeSpeed.movementSmooth) * Time.deltaTime);

//                Vector3 dir = (isStrafing && (!isSprinting || sprintOnlyFree == false) || (freeSpeed.rotateWithCamera && input == Vector3.zero)) && rotateTarget ? rotateTarget.forward : moveDirection;
//                RotateToDirection(dir);
//            }
//        }

//        public virtual void UpdateMoveDirection(Transform referenceTransform = null)
//        {
//            if (input.magnitude <= 0.01)
//            {
//                moveDirection = Vector3.Lerp(moveDirection, Vector3.zero, (isStrafing ? strafeSpeed.movementSmooth : freeSpeed.movementSmooth) * Time.deltaTime);
//                return;
//            }

//            if (referenceTransform && !rotateByWorld)
//            {
//                //get the right-facing direction of the referenceTransform
//                var right = referenceTransform.right;
//                right.y = 0;
//                //get the forward direction relative to referenceTransform Right
//                var forward = Quaternion.AngleAxis(-90, Vector3.up) * right;
//                // determine the direction the player will face based on input and the referenceTransform's right and forward directions
//                moveDirection = (inputSmooth.x * right) + (inputSmooth.z * forward);
//            }
//            else
//            {
//                moveDirection = new Vector3(inputSmooth.x, 0, inputSmooth.z);
//            }
//        }

//        public virtual void Sprint(bool value)
//        {
//            var sprintConditions = (input.sqrMagnitude > 0.1f && isGrounded &&
//                !(isStrafing && !strafeSpeed.walkByDefault && (horizontalSpeed >= 0.5 || horizontalSpeed <= -0.5 || verticalSpeed <= 0.1f)));

//            if (value && sprintConditions)
//            {
//                if (input.sqrMagnitude > 0.1f)
//                {
//                    if (isGrounded && useContinuousSprint)
//                    {
//                        isSprinting = !isSprinting;
//                    }
//                    else if (!isSprinting)
//                    {
//                        isSprinting = true;
//                    }
//                }
//                else if (!useContinuousSprint && isSprinting)
//                {
//                    isSprinting = false;
//                }
//            }
//            else if (isSprinting)
//            {
//                isSprinting = false;
//            }
//        }

//        public virtual void Strafe()
//        {
//            isStrafing = !isStrafing;
//        }

//        public virtual void Jump()
//        {
//            // trigger jump behaviour
//            jumpCounter = jumpTimer;
//            isJumping = true;

//            // trigger jump animations
//            if (input.sqrMagnitude < 0.1f)
//                animator.CrossFadeInFixedTime("Jump", 0.1f);
//            else
//                animator.CrossFadeInFixedTime("JumpMove", .2f);
//        }
//    }
//}

