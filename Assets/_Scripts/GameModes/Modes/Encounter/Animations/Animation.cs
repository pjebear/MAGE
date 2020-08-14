using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAGE.GameModes
{
    class RuntimeAnimation
    {
        public float Duration;
        public float Elapsed;
        public string Name;
        public float SyncPoint;

        // Update is called once per frame
        void Update(float dt)
        {
            Elapsed += dt;
        }
    }


}
