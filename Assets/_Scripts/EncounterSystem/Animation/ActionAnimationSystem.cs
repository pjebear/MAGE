using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EncounterSystem
{
    namespace Animation
    {
        public class ActionAnimationSystem
        {
            private ParticleSystem mAnimationPrefab = null;
            private List<ParticleSystem> mAnimations;
            private const int MaxParticleSystems = 10;

            public ActionAnimationSystem(ParticleSystem animationPrefab, Transform parent)
            {
                mAnimationPrefab = animationPrefab;
                mAnimations = new List<ParticleSystem>(MaxParticleSystems);
                for (int i = 0; i < MaxParticleSystems; ++i)
                {
                    ParticleSystem ps = GameObject.Instantiate(mAnimationPrefab, parent) as ParticleSystem;
                    ps.Stop();
                    mAnimations.Add(ps);
                }
            }

            public void SpawnAnimation(Vector3 location)
            {
                ParticleSystem ps = mAnimations.Find(x => x.isStopped);
                if (ps == null)
                {
                    Debug.Log("AnimationOverflow, adding additional animation to system");
                    ps = GameObject.Instantiate(mAnimationPrefab, mAnimations[0].gameObject.transform.parent) as ParticleSystem;
                    mAnimations.Add(ps);
                }
                ps.transform.position = location;
                ps.Play();
            }
        }
    }

}
