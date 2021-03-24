using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements.Editor
{
    class LoadApparel : MonoBehaviour
    {
        private AssetLoader<Body> mBodyLoader = new AssetLoader<Body>("Props/Bodies");

        [ContextMenu("LoadBody")]
        public void LoadBody()
        {         
            Instantiate(Resources.Load("Props/Bodies/Humanoid/HumanoidMale"), transform);
        }
    }
}
