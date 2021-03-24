using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    enum HeldRegion
    {
        Hand,
        Arm,

        NUM
    }

    enum HolsterRegion
    {
        BackSide,
        BackCenter
    }

    [CreateAssetMenu(fileName = "New HeldApparel", menuName = "Props/HeldApparel")]
    class HeldApparel : ScriptableObject
    {
        public HeldRegion HeldRegion;
        public HolsterRegion HolsterRegion;
        public GameObject ApparelObj;
    }
}
