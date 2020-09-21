using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace MAGE.GameModes.SceneElements
{
    [System.Serializable]
    class CinematicDialogueBehaviour : PlayableBehaviour
    {
        public string Content;
        public ActorSpawner Speaker;
        bool HasStarted = false;
        CinematicMoment mCinematicControl = null;

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            var duration = playable.GetDuration();
            var time = playable.GetTime();
            var count = time + info.deltaTime;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            mCinematicControl = playerData as CinematicMoment;
            if (mCinematicControl != null)
            {
                if (!HasStarted)
                {
                    HasStarted = true;
                }
            }
            else
            {
                Debug.Assert(false);
            }
        }
    }
}