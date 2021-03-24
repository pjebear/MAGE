using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements.Editor
{
    class StripSkinnedMeshRenderer : MonoBehaviour
    {
        [ContextMenu("Strip")]
        public void Strip()
        {
            GameObject parent = transform.parent.gameObject;

            Transform newParent = parent.transform.parent;

            Instantiate(gameObject, newParent);

            DestroyImmediate(parent);
        }
    }
}
