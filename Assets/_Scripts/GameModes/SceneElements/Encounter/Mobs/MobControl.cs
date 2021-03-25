using MAGE.GameModes.SceneElements.Navigation;
using MAGE.GameSystems.Mobs;
using MAGE.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace MAGE.GameModes.SceneElements.Encounters
{
    class MobControl : MonoBehaviour
    {
        public enum State
        {
            Idle,
            Chase,
            Interacting,

            Num
        }
        private State mState = State.Chase;
        private Transform rPlayer;
        private NavigationCoordinatorBase rIdleCoordinator;
        private float mCurrentWanderDelaySec = 0;

        public int MobChaseRange = 10;
        public int MobInteractRange = 3;
        public int MobWanderRange = 5;
        public float NavigationDelayRangeSec = 3f;

        private void Awake()
        {
            //GameObject go = new GameObject("MobCollider");
            //go.transform.SetParent(transform);
            //go.transform.localPosition = Vector3.zero;
            //MobTriggerVolume mobTriggerVolume = go.AddComponent<MobTriggerVolume>();
            //go.transform.localScale = Vector3.one * MobTriggerRange * 2;

            rPlayer = GameObject.FindGameObjectWithTag("Player").transform;
            rIdleCoordinator = GetComponentInParent<NavigationCoordinatorBase>();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, MobChaseRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, MobInteractRange);
        }

        private void Update()
        {
            State updatedState = GetUpdatedState(mState);

            if (mState != updatedState)
            {
                mState = updatedState;

                if (mState == State.Interacting)
                {
                    MessageRouter.Instance.NotifyMessage(new Exploration.ExplorationMessage(Exploration.ExplorationMessage.EventType.EncounterTriggered, gameObject));
                }
                else if (mState == State.Idle)
                {
                    GetComponent<ActorMotor>().Stop();
                }

                GetComponent<Actor>().SetInCombat(mState != State.Idle);
            }

            if (mState == State.Chase)
            {
                GetComponent<ActorMotor>().MoveToPoint(rPlayer.transform.position);
            }
            else if (mState == State.Idle)
            {
                if (rIdleCoordinator != null)
                {
                    if (!GetComponent<NavMeshAgent>().hasPath)
                    {
                        mCurrentWanderDelaySec += Time.deltaTime;
                        if (mCurrentWanderDelaySec > NavigationDelayRangeSec)
                        {
                            GetComponent<ActorMotor>().MoveToPoint(rIdleCoordinator.GetNextNavigationPoint(transform.position));

                            mCurrentWanderDelaySec = 0;
                        }
                    }
                }
            }
        }

        private State GetUpdatedState(State currentState)
        {
            State updatedState = currentState;

            // Update state
            float distanceToPlayer = Vector3.Distance(transform.position, rPlayer.position);
            if (currentState == State.Chase)
            {
                if (distanceToPlayer < MobInteractRange)
                {
                    updatedState = State.Interacting;
                }
                else if (distanceToPlayer > MobChaseRange)
                {
                    updatedState = State.Idle;
                }
            }
            else if (currentState == State.Idle)
            {
                if (distanceToPlayer < MobChaseRange)
                {
                    updatedState = State.Chase;
                }
            }

            return updatedState;
        }
    }
}
