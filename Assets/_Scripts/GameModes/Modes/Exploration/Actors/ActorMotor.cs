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

        private void Awake()
        {
            mMaxSpeed = rNavMeshAgent.speed;
        }

        public void MoveToPoint(Vector3 point, UnityAction moveCompleteCB = null)
        {
            mOnMoveComplete = moveCompleteCB;
            rNavMeshAgent.enabled = true;
            rNavMeshAgent.SetDestination(point);
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

            if (rNavMeshAgent.enabled && !rNavMeshAgent.pathPending && rNavMeshAgent.remainingDistance < .1f)
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

            if (!enabled)
            {
                Stop();
            }
        }

        public bool IsMoving()
        {
            return rNavMeshAgent.enabled && rNavMeshAgent.remainingDistance > .1f;
        }

        private void OnMoveComplete()
        {
            if (mOnMoveComplete != null)
            {
                rNavMeshAgent.enabled = false;

                UnityAction onMoveComplete = mOnMoveComplete;
                mOnMoveComplete = null;
                onMoveComplete();   
            }
        }
    }
}
