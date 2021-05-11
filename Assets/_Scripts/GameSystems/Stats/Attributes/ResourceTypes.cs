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

        public float mCurrent;
        public float mMax;

        public float Ratio { get { return mMax == 0 ? 1 : mCurrent / mMax; } }
        public int Current { get { return Mathf.CeilToInt(mCurrent); } }
        public int Max { get { return Mathf.CeilToInt(mMax); } }

        public Resource(ResourceType resourceType, int max)
        {
            ResourceType = resourceType;
            mCurrent = max;
            mMax = max;
        }

        public void Set(int current, int max)
        {
            UnityEngine.Debug.Assert(current <= max && max > 0);
            current = Mathf.Clamp(current, 0, max);
            mCurrent = current;
            mMax = max;
        }

        public void SetMax(int max, ScaleType scaleType = ScaleType.Scale)
        {
            switch (scaleType)
            {
                case ScaleType.Scale:
                {
                    float currentRatio = Ratio;
                    mMax = max;
                    mCurrent = (int)(mMax * currentRatio);
                }
                break;
                case ScaleType.Flat:
                {
                    float delta = max - mMax;
                    mMax += delta;
                    mCurrent += delta;
                }
                break;
                default:
                    Debug.Assert(false);
                    break;
            }

            
        }

        public void SetCurrentToMax()
        {
            mCurrent = mMax;
        }

        public void SetCurrentToZero()
        {
            mCurrent = 0;
        }

        public void Modify(float delta)
        {
            mCurrent += delta;
            if (mCurrent > mMax) mCurrent = mMax;
            if (mCurrent < 0) mCurrent = 0;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}/{2}", ResourceType.ToString(), mCurrent, mMax);
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
