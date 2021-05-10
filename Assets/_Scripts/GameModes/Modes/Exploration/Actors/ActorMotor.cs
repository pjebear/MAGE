using MAGE.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace MAGE.GameModes.SceneElements.Navigation
{
    class ActorMotor : MonoBehaviour
    {
        private float mMaxSpeed = 10f;
        private NavMeshAgent rNavMeshAgent { get { return GetComponent<NavMeshAgent>(); } }
        private bool mIsMoving = false;
        private UnityAction mOnMoveComplete = null;
        private Vector3 mMoveToPoint;

        private void Awake()
        {
            mMaxSpeed = rNavMeshAgent.speed;
        }

        public void MoveToPoint(Vector3 point, UnityAction moveCompleteCB = null)
        {
            mOnMoveComplete = moveCompleteCB;
            mIsMoving = true;
            rNavMeshAgent.SetDestination(point);
            mMoveToPoint = point;
        }

        public void Stop()
        {
            if (rNavMeshAgent.isOnNavMesh)
            {
                rNavMeshAgent.ResetPath();
            }
            GetComponent<ActorAnimator>().SetCurrentSpeed(0);

            OnMoveComplete();
        }

        void Update()
        {
            if (rNavMeshAgent.isOnOffMeshLink)
            {
                rNavMeshAgent.speed = 2;
                OffMeshLinkData data = rNavMeshAgent.currentOffMeshLinkData;
            }
            else
            {
                rNavMeshAgent.speed = mMaxSpeed;
            }

            // TEMP
            GetComponent<ActorAnimator>().SetCurrentSpeed(GetCurrentSpeed());

            if (mIsMoving && !rNavMeshAgent.pathPending && !rNavMeshAgent.hasPath)
            {
                Stop();
            }
        }

        public float GetCurrentSpeed()
        {
            return rNavMeshAgent.velocity.magnitude;
        }

        public void Enable(bool enabled)
        {
            this.enabled = enabled;

            if (enabled)
            {
                rNavMeshAgent.enabled = true;
            }
            else
            {
                rNavMeshAgent.enabled = false;
                Stop();
            }  
        }

        private void OnMoveComplete()
        {
            mIsMoving = false;

            if (mOnMoveComplete != null)
            {
                transform.position = mMoveToPoint;
                UnityAction onMoveComplete = mOnMoveComplete;
                mOnMoveComplete = null;
                onMoveComplete();   
            }
        }
    }
}
