using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Combat
{
    class DebugCombatVisualizations : MonoBehaviour
    {
        public SphereCollider MovementRangeVisualization;
        public SphereCollider ActionRangeVisualization;

        private void Awake()
        {
            if (MovementRangeVisualization != null) ActionRangeVisualization.gameObject.SetActive(false);
            if (ActionRangeVisualization != null) ActionRangeVisualization.gameObject.SetActive(false);
        }
    }
}
