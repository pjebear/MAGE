using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements.Navigation
{
    class WanderCoordinator : NavigationCoordinatorBase
    {
        public float WanderRange = 10;

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.white;

            Gizmos.DrawWireSphere(transform.position, WanderRange);
        }

        public override Vector3 GetNextNavigationPoint(Vector3 currentPosition)
        {
            Vector3 nextWanderLocation = transform.position;

            float angleDegrees = UnityEngine.Random.Range(0f, 360f);
            float length = UnityEngine.Random.Range(0f, WanderRange);

            Vector3 offsetFromCenter = Vector3.forward * length;
            offsetFromCenter = Quaternion.Euler(0, angleDegrees, 0) * offsetFromCenter;

            Ray ray = new Ray(transform.position + offsetFromCenter + Vector3.up * 100, Vector3.down);
            int layerMask = 1 << (int)RayCastLayer.Terrain;
            RaycastHit out_hit;
            if (Physics.Raycast(ray, out out_hit, 500f, layerMask))
            {
                nextWanderLocation = out_hit.point;
            }
            else
            {
                Debug.LogWarning("Failed to find next wander point");
            }

            return nextWanderLocation;
        }
    }
}
