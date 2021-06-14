using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements.Navigation
{
    class PathCoordinator : NavigationCoordinatorBase
    {
        public PathRoute Route;

        public override Vector3 GetNextNavigationPoint(Vector3 currentPosition)
        {
            Vector3 nextPoint = currentPosition;

            if (Route != null && Route.Route.Count > 0)
            {
                int closestNodeIdx = 0;
                float closestNodeDistance = float.MaxValue;
                for (int i = 0; i < Route.Route.Count; ++i)
                {
                    float distance = Vector3.Distance(currentPosition, Route.Route[i].transform.position);
                    if (distance < closestNodeDistance)
                    {
                        closestNodeDistance = distance;
                        closestNodeIdx = i;
                    }
                }

                nextPoint = Route.Route[(closestNodeIdx + 1) % Route.Route.Count].transform.position;
            }

            return nextPoint;
        }
    }
}
