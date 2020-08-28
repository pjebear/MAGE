using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameSystems.Characters
{
    class Resource
    {
        public int Current;
        public int Max;

        public float Ratio { get { return Max == 0 ? 0 : Current / (float)Max; } }

        public Resource(int max)
        {
            Current = max;
            Max = max;
        }

        public void Set(int current, int max)
        {
            Debug.Assert(current < max && max > 0);

            Current = current;
            Max = max;
        }

        public void SetMax(int max)
        {
            float currentRatio = Ratio;
            Max = max;
            Current = (int)(Max * currentRatio);
        }

        public void Modify(int delta)
        {
            Current += delta;
            if (Current > Max) Current = Max;
            if (Current < 0) Current = 0;
        }
    }

    class Resources
    {
        private Resource[] mResources;

        public Resources(int health, int mana, int endurance, int clock)
        {
            mResources = new Resource[(int)ResourceType.NUM];

            mResources[(int)ResourceType.Health] = new Resource(health);
            mResources[(int)ResourceType.Mana] = new Resource(mana);
            mResources[(int)ResourceType.Endurance] = new Resource(endurance);
            mResources[(int)ResourceType.Clock] = new Resource(clock);
        }

        public Resource this[ResourceType idx]
        {
            get { return mResources[(int)idx]; }
        }
    }


}
