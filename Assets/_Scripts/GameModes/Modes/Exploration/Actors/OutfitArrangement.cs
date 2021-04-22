using MAGE.GameSystems.Appearances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    class OutfitArrangement : MonoBehaviour
    {
        public ApparelAssetId OutfitArrangementType = ApparelAssetId.NONE;
        public List<SkinnedMeshRenderer> ColorableMeshes;
        public List<SkinnedMeshRenderer> GetArrangement()
        {
            return GetComponentsInChildren<SkinnedMeshRenderer>().ToList();
        }
    }
}
