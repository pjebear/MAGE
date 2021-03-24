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
        public List<PathNodeTriggerVolume> Route = new List<PathNodeTriggerVolume>();

        private void Awake()
        {
            Route = GetComponentsInChildren<PathNodeTriggerVolume>(true).ToList();
        }
    }
}
