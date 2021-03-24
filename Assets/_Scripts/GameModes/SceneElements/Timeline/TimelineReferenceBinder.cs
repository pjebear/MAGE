using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace MAGE.GameModes.SceneElements
{
    class TimelineReferenceBinder : MonoBehaviour
    {
        public PlayableDirector Director;

        private TimelineAsset mTimeline;

        private void Start()
        {
            Director.played += RebindAnimationReferences;
        }


        private void RebindAnimationReferences(PlayableDirector playableDirector)
        {
            TimelineAsset timeline = playableDirector.playableAsset as TimelineAsset;
            foreach (PlayableBinding binding in timeline.outputs)
            {
                if (binding.sourceObject is AnimationTrack)
                {
                    AnimationTrack animationTrack = binding.sourceObject as AnimationTrack;
                    if (animationTrack.name == "ActorAnimator")
                    {
                        UnityEngine.Object o = playableDirector.GetGenericBinding(animationTrack);
                        Animator animator = o as Animator;
                        if (animator != null)
                        {
                            GameObject go = animator.gameObject;
                            Actor actor = go.GetComponentInParent<Actor>();

                            playableDirector.SetGenericBinding(animationTrack, actor.GetComponentInChildren<Animator>());
                        }               
                    }
                }
            }
        }
    }
}
