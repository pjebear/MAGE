using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    class Effect : MonoBehaviour
    {
        public EffectDurationInfo DurationInfo = new EffectDurationInfo();

        private void Awake()
        {
            Destroy(gameObject, DurationInfo.DurationSeconds);
        }
    }
}

