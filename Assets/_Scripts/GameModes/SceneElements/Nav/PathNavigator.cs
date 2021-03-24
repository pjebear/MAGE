using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace MAGE.GameModes.SceneElements.Navigation
{
    class PathNavigator : MonoBehaviour
    {
        public float PathSpeed = ActorConstants.MAX_SPEED;
        public PathRoute Route;
        
        public float IdleTimeAtNodeSeconds = 3;

        private int mCurrentNodeIdx = 0;
        private NavMeshAgent rAgent = null;

        private void Awake()
        {
            rAgent = GetComponent<NavMeshAgent>();
            if (rAgent == null)
            {
                rAgent = gameObject.AddComponent<NavMeshAgent>();
            }
            
            GameObject go = new GameObject("NavCollider");
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector3.up * .9f;
            SphereCollider collider = go.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.gameObject.layer = (int)Layer.Navigation;
        }

        private void Update()
        {
            if (PathSpeed < ActorConstants.WALK_SPEED) PathSpeed = ActorConstants.WALK_SPEED;
            if (PathSpeed > ActorConstants.MAX_SPEED) PathSpeed = ActorConstants.MAX_SPEED;

            float speed = PathSpeed;


            rAgent.speed = PathSpeed;
        }

        private void Start()
        {
            if (HasValidPath())
            {
                ProgressPath();
            }
        }

        IEnumerator _Idle(float seconds)
        {
            while (seconds > 0)
            {
                seconds -= Time.deltaTime;
                yield return null;
            }
            ProgressPath();
        }

        private void ProgressPath()
        {
            if (HasValidPath())
            {
                PathNodeTriggerVolume nextNode = Route.Route[mCurrentNodeIdx];
                rAgent.SetDestination(nextNode.transform.position);
            }
        }

        public void NotifyArrivedAtNode(PathNodeTriggerVolume node)
        {
            if (HasValidPath())
            {
                if (Route.Route[mCurrentNodeIdx] == node)
                {
                    mCurrentNodeIdx++;
                    if (mCurrentNodeIdx >= Route.Route.Count)
                    {
                        mCurrentNodeIdx = 0;
                    }
                    StartCoroutine(_Idle(IdleTimeAtNodeSeconds));
                }
            }
        }

        private bool HasValidPath()
        {
            return Route != null && Route.Route.Count > 0;
        }

    }
}
