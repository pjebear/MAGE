using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    [CreateAssetMenu(fileName = "New Rig", menuName = "Props/Bodies")]
    class Rig : ScriptableObject
    {
        public Animator Animator;
    }
}
