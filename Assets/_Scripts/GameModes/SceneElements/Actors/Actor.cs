using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    class Actor : MonoBehaviour
    {
        private bool mInCombat = false;

        public void SetHeldApparelState(HumanoidActorConstants.HeldApparelState heldApparelState)
        {
            ActorOutfitter actorOutfitter = GetComponentInChildren<ActorOutfitter>();
            if (actorOutfitter != null)
            {
                actorOutfitter.UpdateHeldApparelState(heldApparelState, true);
            }
        }

        public void SetInCombat(bool inCombat)
        {
            mInCombat = inCombat;

            ActorOutfitter actorOutfitter = GetComponentInChildren<ActorOutfitter>();
            if (actorOutfitter != null)
            {
                actorOutfitter.UpdateHeldApparelState(mInCombat ? HumanoidActorConstants.HeldApparelState.MeleeHeld : HumanoidActorConstants.HeldApparelState.Holstered, false);
            }

            ActorAnimator actorAnimator = GetComponentInChildren<ActorAnimator>();
            actorAnimator.SetInCombat(mInCombat);
        }
    }
}





