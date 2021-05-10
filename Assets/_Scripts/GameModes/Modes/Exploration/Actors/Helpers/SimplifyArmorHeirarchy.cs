using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements.Editors
{
    class SimplifyArmorHeirarchy : MonoBehaviour
    {
        private void Awake()
        {
            Strip();
        }

        [ContextMenu("Strip")]
        public void Strip()
        {
            SkinnedMeshRenderer reskinTo = GetComponentInParent<SkinnedMeshRenderer>();

            for (int i = 0; i < transform.childCount; ++i)
            {
                Transform child = transform.GetChild(i);
                SkinnedMeshRenderer toReskin = child.GetComponentInChildren<SkinnedMeshRenderer>();
                ReskinMesh.Reskin(toReskin, reskinTo);
            }
        }
    }
}
