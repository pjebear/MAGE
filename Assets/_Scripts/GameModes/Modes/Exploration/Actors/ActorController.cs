using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ActorController : MonoBehaviour
{
    [SerializeField]
    public float MoveSpeed = 5;
    [SerializeField]
    public float RotSpeed = 150;
    private Animator mAnimator;

    private Vector3 mMovementInput;
    public Rigidbody mRigidBody;

    #region Unity
    private void Start()
    {
        mRigidBody = GetComponent<Rigidbody>();
        mAnimator = GetComponentInChildren<Animator>();
    }
    #endregion //Unity

    public virtual void RotateToDirectionOverTime(Vector3 newForward)
    {
        transform.forward = Vector3.Lerp(transform.forward, newForward, Time.deltaTime * RotSpeed);
    }

    public virtual void RotateToDirection(Vector3 direction)
    {
        //if (!jumpAndRotate && !isGrounded) return;
        if (direction.sqrMagnitude >= 0.1)
        {
            direction.y = 0f;
            transform.Rotate(0, direction.x * RotSpeed * Time.deltaTime, 0);
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

