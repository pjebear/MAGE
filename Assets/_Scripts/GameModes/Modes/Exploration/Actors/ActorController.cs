using MAGE.GameModes.SceneElements.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace MAGE.GameModes.SceneElements
{
    class ActorController 
        : MonoBehaviour
        , ActorSpawner.IRefreshListener
    {
        public enum ControllerState
        {
            None,
            ThirdPerson,
            TopDown,

            NUM
        }
        public ControllerState ControlState = ControllerState.None;

        private Rigidbody mRigidBody;
        private CapsuleCollider mCollider;
        private ActorSpawner mActorSpawner;

        private NavMeshAgent rNavMeshAgent = null;
        private TopDownActorController rTopDownController = null;
        private CharacterController rCharacterController = null;
        private ThirdPersonActorController rThirdPersonController = null;
        public ActorMotor ActorMotor { get { return GetComponent<ActorMotor>(); } }
        public ActorAnimator rActorAnimator { get { return GetComponent<ActorAnimator>(); } }


        bool mIsWalking = false;

        private void Awake()
        {
            rNavMeshAgent = GetComponent<NavMeshAgent>();
            rTopDownController = GetComponent<TopDownActorController>();
            rCharacterController = GetComponent<CharacterController>();
            rThirdPersonController = GetComponent<ThirdPersonActorController>();

            mRigidBody = GetComponent<Rigidbody>();
            mCollider = GetComponent<CapsuleCollider>();
            mActorSpawner = GetComponentInChildren<ActorSpawner>();
            if (mActorSpawner != null)
            {
                mActorSpawner.RegisterListener(this);
            }
        }

        private void OnDestroy()
        {
            if (mActorSpawner != null)
            {
                mActorSpawner.UnRegisterListener(this);
            }
        }

        #region Unity
        private void Start()
        {
            OnActorRefresh();
            SetControllerState(ControlState);
        }
        #endregion //Unity

        public void SetControllerState(ControllerState controllerState)
        {
            switch (controllerState)
            {
                case ControllerState.TopDown:
                {
                    rThirdPersonController.Enable(false);
                    rTopDownController.Enable(true);
                    Camera.main.GetComponent<Cameras.CameraController>().SetTarget(transform, Cameras.CameraType.TopDown);
                    // Update camera
                }
                break;

                case ControllerState.ThirdPerson:
                {
                    rTopDownController.Enable(false);
                    rThirdPersonController.Enable(true);
                    Camera.main.GetComponent<Cameras.CameraController>().SetTarget(transform, Cameras.CameraType.ThirdPerson);
                }
                break;
                case ControllerState.None:
                {
                    rTopDownController.Enable(false);
                    rThirdPersonController.Enable(false);
                }
                break;
            }

            ControlState = controllerState;
        }

        public void SetInCombat(bool inCombat)
        {
            GetComponent<ActorOutfitter>().UpdateHeldApparelState(inCombat ? HumanoidActorConstants.HeldApparelState.Held : HumanoidActorConstants.HeldApparelState.Holstered);
            GetComponent<ActorAnimator>().SetInCombat(inCombat);
        }
        //public virtual void RotateToDirectionOverTime(Vector3 newForward)
        //{
        //    transform.forward = Vector3.Lerp(transform.forward, newForward, Time.deltaTime * RotSpeed);
        //}

        //public virtual void RotateToDirection(Vector3 direction)
        //{
        //    //if (!jumpAndRotate && !isGrounded) return;
        //    if (direction.sqrMagnitude >= 0.1)
        //    {
        //        direction.y = 0f;
        //        transform.Rotate(0, direction.x * RotSpeed * Time.deltaTime, 0);
        //    }
        //    else
        //    {

        //    }
        //    mRigidBody.angularVelocity = Vector3.zero;
        //}

        //public virtual void MoveCharacter(Vector3 _direction)
        //{
        //    // calculate input smooth
        //    //inputSmooth = Vector3.Lerp(inputSmooth, input, (isStrafing ? strafeSpeed.movementSmooth : freeSpeed.movementSmooth) * Time.deltaTime);

        //    //if (!isGrounded || isJumping) return;

        //    _direction.y = 0;
        //    _direction.x = Mathf.Clamp(_direction.x, -1f, 1f);
        //    _direction.z = Mathf.Clamp(_direction.z, -1f, 1f);
        //    // limit the input
        //    if (_direction.magnitude > 1f)
        //        _direction.Normalize();

        //    if (_direction.sqrMagnitude >= 0.1)
        //    {
        //        Vector3 targetPosition = mRigidBody.position + transform.forward * MoveSpeed * _direction.z * Time.deltaTime;
        //        Vector3 targetVelocity = (targetPosition - transform.position) / Time.deltaTime;

        //        //bool useVerticalVelocity = true;
        //        targetVelocity.y = mRigidBody.velocity.y;
        //        mRigidBody.velocity = targetVelocity;
        //        mIsWalking = true;
        //    }
        //    else
        //    {
        //        mRigidBody.velocity = Vector3.up * mRigidBody.velocity.y;
        //        mIsWalking = false;
        //    }

        //    UpdateActorAnimations();
        //}

        private void UpdateActorAnimations()
        {
            if (mActorSpawner != null)
            {
                //mActorSpawner.GetActor().rActorAnimator.SetBool("IsWalking", mIsWalking);
            }
        }

        public void OnActorRefresh()
        {
            UpdateActorAnimations();
        }
    }
}


