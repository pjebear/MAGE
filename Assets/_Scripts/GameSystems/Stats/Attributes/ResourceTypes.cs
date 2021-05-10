using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameSystems.Stats
{
    [Serializable]
    class Resource
    {
        public enum ScaleType
        {
            Flat,
            Scale,
        }

        public ResourceType ResourceType;
        public int Current;
        public int Max;

        public float Ratio { get { return Max == 0 ? 1 : Current / (float)Max; } }

        public Resource(ResourceType resourceType, int max)
        {
            ResourceType = resourceType;
            Current = max;
            Max = max;
        }

        public void Set(int current, int max)
        {
            UnityEngine.Debug.Assert(current <= max && max > 0);
            current = Mathf.Clamp(current, 0, max);
            Current = current;
            Max = max;
        }

        public void SetMax(int max, ScaleType scaleType = ScaleType.Scale)
        {
            switch (scaleType)
            {
                case ScaleType.Scale:
                {
                    float currentRatio = Ratio;
                    Max = max;
                    Current = (int)(Max * currentRatio);
                }
                break;
                case ScaleType.Flat:
                {
                    int delta = max - Max;
                    Max += delta;
                    Current += delta;
                }
                break;
                default:
                    Debug.Assert(false);
                    break;
            }

            
        }

        public void SetCurrentToMax()
        {
            Current = Max;
        }

        public void Modify(int delta)
        {
            Current += delta;
            if (Current > Max) Current = Max;
            if (Current < 0) Current = 0;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}/{2}", ResourceType.ToString(), Current, Max);
        }
    }

    class Resources
    {
        private Resource[] mResources;

        public Resources()
        {
            mResources = new Resource[(int)ResourceType.NUM];

            for (int i = 0; i < (int)ResourceType.NUM; ++i)
            {
                mResources[i] = new Resource((ResourceType)i, 0);
            }
        }

        public Resources(int health, int mana, int endurance, int clock, int actions, int movementRange)
            : this()
        {
            mResources[(int)ResourceType.Health].Set(health, health);
            mResources[(int)ResourceType.Mana].Set(0, mana);
            mResources[(int)ResourceType.Endurance].Set(0, endurance);
            mResources[(int)ResourceType.Clock].Set(0, clock); 
            mResources[(int)ResourceType.Actions].Set(0, actions); 
            mResources[(int)ResourceType.MovementRange].Set(0, movementRange);
        }

        public Resource this[ResourceType idx]
        {
            get { return mResources[(int)idx]; }
        }
    }


}
