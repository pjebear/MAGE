using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;

namespace MAGE.GameModes.SceneElements
{
    class ActorAnimator : MonoBehaviour
    {
        private float mSpeedPercent = 0f;

        private void Update()
        {   
            float animatorParam = GetLocomotionParamFromSpeed(mSpeedPercent);
            GetAnimator().SetFloat(
                ActorAnimationContstants.ANIM_MOVE_SPEED
                , animatorParam
                , ActorAnimationContstants.ANIM_MOVE_DAMP_TIME
                , Time.deltaTime);
        }

        public Animator GetAnimator()
        {
            return GetComponentInChildren<Animator>();
        }

        public void SetCurrentSpeed(float speed)
        {
            mSpeedPercent = speed / ActorConstants.MAX_SPEED;
        }

        public void Trigger(string triggerName)
        {
            GetAnimator().SetTrigger(triggerName);
        }

        public void SetInCombat(bool inCombat)
        {
            GetAnimator().SetBool("bInCombat", inCombat);
        }

        public void SetHandGraspState(HumanoidActorConstants.Hand handSide, bool isGrasped)
        {
            string paramName = "bHandGrasp" + handSide.ToString();

            GetAnimator().SetBool(paramName, isGrasped);
        }

        public void AnimateHoldItem(HumanoidActorConstants.Hand handSide, bool isHeld)
        {
            string heldAnimationParam = string.Format("{0}Weapon{1}", isHeld ? "grab" : "away", handSide.ToString());
            GetAnimator().SetTrigger(heldAnimationParam);

            SetHandGraspState(handSide, isHeld);
        }

        public float GetLocomotionParamFromSpeed(float speedPercent)
        {
            float locomotionParam = 1;

            if (speedPercent < 0)
            {
                locomotionParam = 0;
            }
            else
            {
                locomotionParam = 1 + 2 * speedPercent;
            }

            return locomotionParam;
        }
    }
}


