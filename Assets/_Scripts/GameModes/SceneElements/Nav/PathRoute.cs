using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements.Navigation
{
    class PathRoute : MonoBehaviour
    {
        [HideInInspector]
        public List<Transform> Route = new List<Transform>();

        private void Awake()
        {
            Route = GetComponentsInChildren<Transform>(true).ToList();
        }

        private void OnDrawGizmosSelected()
        {
            List<Transform> path = GetComponentsInChildren<Transform>(true).ToList();
            path.Remove(transform);
            if (path.Count > 1)
            {
                for (int i = 0; i < path.Count; ++i)
                {
                    Transform current = path[i];
                    int nextIndex = (i + 1) % path.Count;
                    Transform next = path[nextIndex];
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(current.position, next.position);
                    Gizmos.DrawWireCube(current.position, new Vector3(.1f, 1, .1f));
                }
            }
        }
    }
}
