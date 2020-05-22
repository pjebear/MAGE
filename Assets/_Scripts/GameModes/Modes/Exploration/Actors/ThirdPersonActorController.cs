using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ThirdPersonActorController 
    : MonoBehaviour
    , IInputHandler
{
    [SerializeField]
    private float MoveSpeed = 5;
    [SerializeField]
    private float RotSpeed = 5;
    private Animator mAnimator;

    private Vector3 mMovementInput;
    private Rigidbody mRigidBody;
   

    #region IInputHandler
    // IInputHandler
    public void OnKeyPressed(InputSource source, int key, InputState state)
    {
        if (source == InputSource.Keyboard)
        {
            int inputValue = state == InputState.Up ? 0 : 1;

            switch ((KeyCode)key)
            {
                case (KeyCode.W):
                {
                    mMovementInput.z = inputValue;
                }
                break;
                case (KeyCode.S):
                {
                    mMovementInput.z = -inputValue;
                }
                break;
                case (KeyCode.A):
                {
                    mMovementInput.x = -inputValue;
                }
                break;
                case (KeyCode.D):
                {
                    mMovementInput.x = inputValue;
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

    #region Unity
    private void Start()
    {
        mRigidBody = GetComponent<Rigidbody>();
        mAnimator = GetComponentInChildren<Animator>();
        InputManager.Instance.RegisterHandler(this, false);    
    }

    private void Update()
    {
        MoveCharacter(mMovementInput);
        RotateToDirection(mMovementInput);
    }
    #endregion //Unity

    public virtual void RotateToDirection(Vector3 direction)
    {
        //if (!jumpAndRotate && !isGrounded) return;
        if (direction.sqrMagnitude >= 0.1)
        {
            direction.y = 0f;
            transform.Rotate(0, direction.x * RotSpeed, 0);
        }
        else
        {
            
        }
        mRigidBody.angularVelocity = Vector3.zero;
    }

    public virtual void MoveCharacter(Vector3 _direction)
    {
        // calculate input smooth
        //inputSmooth = Vector3.Lerp(inputSmooth, input, (isStrafing ? strafeSpeed.movementSmooth : freeSpeed.movementSmooth) * Time.deltaTime);

        //if (!isGrounded || isJumping) return;

        _direction.y = 0;
        _direction.x = Mathf.Clamp(_direction.x, -1f, 1f);
        _direction.z = Mathf.Clamp(_direction.z, -1f, 1f);
        // limit the input
        if (_direction.magnitude > 1f)
            _direction.Normalize();

        if (_direction.sqrMagnitude >= 0.1)
        {
            Vector3 targetPosition = mRigidBody.position + transform.forward * MoveSpeed * _direction.z * Time.deltaTime;
            Vector3 targetVelocity = (targetPosition - transform.position) / Time.deltaTime;

            //bool useVerticalVelocity = true;
            targetVelocity.y = mRigidBody.velocity.y;
            mRigidBody.velocity = targetVelocity;
            mAnimator.SetBool("IsWalking", true);
        }
        else
        {
            mRigidBody.velocity = Vector3.up * mRigidBody.velocity.y;
            mAnimator.SetBool("IsWalking", false);
        }

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

