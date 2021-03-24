using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    enum BlendShapeRegion
    {
        Leg,
        Torso,
        UpperArm,
        LowerArm,

        NUM
    }

    enum ApparelSlot
    {
        Leg1,
        Leg2,
        Torso1,
        Torso2,
        Hair,
        Eyebrow,
        FacialHair,

        NUM
    }

    [CreateAssetMenu(fileName ="New Apparel", menuName = "Props/Apparel")]
    class Apparel : ScriptableObject
    {
        public List<BlendShapeRegion> BlendShapeRegions = new List<BlendShapeRegion>();
        public ApparelSlot ApparelSlot = ApparelSlot.Leg1;
        public SkinnedMeshRenderer ApparelMesh;
    }
}
