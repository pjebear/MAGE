using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EncounterSystem
{
    namespace Animation
    {
        public enum AnimationIndex
        {
            FIRE1,
            FIRE2,
            NUM,
            NONE
        }

        public class AnimationFactory : MonoBehaviour
        {
            public static AnimationFactory Instance
            {
                get
                {
                    if (mInstance == null)
                    {
                        GameObject go = new GameObject();
                        go.AddComponent<AnimationFactory>();
                        mInstance = go.GetComponent<AnimationFactory>();
                    }
                    return mInstance;
                }
            }
            private static AnimationFactory mInstance;
            public List<ParticleSystem> mParticleSystems;
            private List<ActionAnimationSystem> mAnimationSystems;

            private void Awake()
            {
                if (mInstance == null)
                {
                    mInstance = this;
                }
                mAnimationSystems = new List<ActionAnimationSystem>();
            }

            public void Start()
            {
                foreach (var ps in mParticleSystems)
                {
                    mAnimationSystems.Add(new ActionAnimationSystem(ps, this.transform));
                }
            }

            public ActionAnimationSystem CheckOutAnimation(AnimationIndex index)
            {
                switch (index)
                {
                    case (AnimationIndex.FIRE1):
                        return mAnimationSystems[(int)index];
                    case (AnimationIndex.FIRE2):
                        return mAnimationSystems[(int)index];
                    default:
                        return null;
                }
            }
        }
    }
}
