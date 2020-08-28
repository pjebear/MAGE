using MAGE.GameModes;
using MAGE.GameModes.Encounter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MAGE.GameModes.Encounter
{
    class DebugAnimationRig : MonoBehaviour
    {
        public CharacterActorController ActorController;
        public Text AnimationName;
        public RectTransform ProgressBarRect;
        public Image ProgressBarImage;

        public Vector3 Offset;

        private RuntimeAnimation Animation;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (ActorController != null)
            {
                transform.position = Camera.main.WorldToScreenPoint(ActorController.transform.position) + Offset;

                if (Animation != null)
                {
                    Animation.Elapsed += Time.deltaTime;
                    float percentComplete = Animation.Elapsed / Animation.Duration;
                    if (percentComplete < 1)
                    {
                        ProgressBarRect.localScale = new Vector3(percentComplete, 1, 0);
                        if (Animation.Elapsed > Animation.SyncPoint)
                        {
                            ProgressBarImage.color = Color.green;
                        }
                    }
                    else
                    {
                        gameObject.SetActive(false);
                        Animation = null;
                    }
                }
            }

        }

        public void DisplayAnimation(AnimationInfo animation)
        {
            gameObject.SetActive(true);
            Animation = ConverPlaceholderToAnimation(animation);
            ProgressBarRect.localScale = new Vector3(1, 1, 0);
            ProgressBarImage.color = Color.yellow;
            AnimationName.text = Animation.Name;
        }

        private RuntimeAnimation ConverPlaceholderToAnimation(AnimationInfo animationPh)
        {
            RuntimeAnimation animation = new RuntimeAnimation();
            animation.Name = animationPh.TriggerName;
            animation.Duration = (float)animationPh.NumFrames / AnimationConstants.FRAMES_PER_SECOND;
            animation.SyncPoint = (float)animationPh.SyncedFrame / AnimationConstants.FRAMES_PER_SECOND;

            return animation;
        }
    }

}
