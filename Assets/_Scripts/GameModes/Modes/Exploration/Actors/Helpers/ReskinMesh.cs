using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    class ReskinMesh : MonoBehaviour
    {
        private void Awake()
        {
            SkinnedMeshRenderer toReskin = GetComponent<SkinnedMeshRenderer>();
            Debug.Assert(toReskin != null);
            if (toReskin != null)
            {
                SkinnedMeshRenderer reskinTo = GetComponentInParent<SkinnedMeshRenderer>();
                Debug.Assert(toReskin != null);
                if (reskinTo != null)
                {
                    Reskin(toReskin, reskinTo);
                }
            }

            Destroy(this);
        }

        public static void Reskin(SkinnedMeshRenderer toReskin, SkinnedMeshRenderer reskinTo)
        {
            if (toReskin.bones.Length != reskinTo.bones.Length)
            {
                List<Transform> findable = reskinTo.bones.ToList();
                for (int i = 0; i < toReskin.bones.Length; ++i)
                {
                    if (toReskin.bones[i] != null)
                    {
                        string boneName = toReskin.bones[i].name;

                        if (i >= reskinTo.bones.Length || findable.Find(x => x != null && x.name == boneName) == null)
                        {
                            Debug.LogWarning(string.Format("Failed to find bone {0} in new bone mapping", boneName));
                        }
                    }
                }
            }
            Transform[] remappedBones = new Transform[toReskin.bones.Length];
            for (int i = 0; i < toReskin.bones.Length; ++i)
            {
                string boneName = toReskin.bones[i].name;
                remappedBones[i] = reskinTo.bones.FirstOrDefault(x => x != null && x.name == boneName);
            }
            toReskin.bones = remappedBones;
            toReskin.rootBone = reskinTo.rootBone;
        }
    }
}
