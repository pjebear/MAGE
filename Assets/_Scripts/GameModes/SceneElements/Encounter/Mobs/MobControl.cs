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
            Wander,
            Chase,
            ReturningFromChase,
            Interacting,

            Num
        }

        private State mState = State.Idle;
        private Transform rPlayer;
        private NavigationCoordinatorBase rWanderCoordinator;
        private float mCurrentWanderDelaySec = 0;
        private float mCurrentWanderDurationSec = 0;
        private Vector3 mChaseStartingPoint = Vector3.zero;

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
            rWanderCoordinator = GetComponentInParent<NavigationCoordinatorBase>();
            if (rWanderCoordinator == null)
            {
                rWanderCoordinator = GetComponent<NavigationCoordinatorBase>();
            }

            Debug.Assert(rWanderCoordinator);
        }

        private void OnDisable()
        {
            mState = State.Wander;
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
            State previousState = mState;
            mState = GetUpdatedState(mState);

            if (mState == State.Interacting && previousState != mState)
            {
                MessageRouter.Instance.NotifyMessage(new Exploration.ExplorationMessage(Exploration.ExplorationMessage.EventType.EncounterTriggered, gameObject));
                GetComponent<ActorMotor>().Stop();
                GetComponent<Actor>().SetInCombat(mState != State.Wander);
            }
            else if (mState == State.Chase)
            {
                if (mState != previousState)
                {
                    mChaseStartingPoint = transform.position;
                }
                GetComponent<ActorMotor>().MoveToPoint(rPlayer.transform.position);
            }
            else if (previousState != mState && mState == State.ReturningFromChase)
            {
                GetComponent<ActorMotor>().MoveToPoint(mChaseStartingPoint);
            }
            else if (mState == State.Wander)
            {
                if (rWanderCoordinator != null)
                {
                   if (previousState != mState)
                    {    
                        GetComponent<ActorMotor>().MoveToPoint(rWanderCoordinator.GetNextNavigationPoint(transform.position));   
                    }
                }
            }
            else if (mState == State.Idle)
            {
                if (mState != previousState)
                {
                    GetComponent<ActorMotor>().Stop();
                    mCurrentWanderDelaySec = 0;
                }
                else
                {
                    mCurrentWanderDelaySec += Time.deltaTime;
                }
            }
        }

        private State GetUpdatedState(State currentState)
        {
            State updatedState = currentState;

            // Stuck in interacting until otherwise specified
            if (currentState != State.Interacting)
            {
                if (currentState != State.ReturningFromChase)
                {
                    // Update state
                    float distanceToPlayer = Vector3.Distance(transform.position, rPlayer.position);
                    if (distanceToPlayer < MobInteractRange)
                    {
                        updatedState = State.Interacting;
                    }
                    else if (distanceToPlayer < MobChaseRange)
                    {
                        updatedState = State.Chase;
                    }
                    else if (currentState == State.Chase)
                    {
                        updatedState = State.ReturningFromChase;
                    }
                }
               
                if (currentState == updatedState)
                {
                    if (currentState == State.Wander || currentState == State.ReturningFromChase)
                    {
                        if (!GetComponent<ActorMotor>().IsMoving())
                        {
                            updatedState = State.Idle;
                        }
                    }
                    else if (currentState == State.Idle)
                    {
                        if (mCurrentWanderDelaySec > NavigationDelayRangeSec)
                        {
                            updatedState = State.Wander;
                        }
                    }
                }
            }

            return updatedState;
        }
    }
}
