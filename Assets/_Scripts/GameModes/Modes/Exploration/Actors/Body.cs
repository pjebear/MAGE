using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    class Body : MonoBehaviour
    {
        public BodyType BodyType = BodyType.HumanoidMale;

        public Transform ShieldHolsterTransform;
        public Transform BackHolsterLTransform;
        public Transform BackHolsterRTransform;
        public Transform RightHandTransform;
        public Transform RightSheidlTransform;
        public Transform LeftHandTransform;
        public Transform LeftShieldTransform;
        public Transform FacialHairTransform;

        public SkinnedMeshRenderer BodyMesh { get { return gameObject.GetComponentInChildren<SkinnedMeshRenderer>(); } }
    }
}
