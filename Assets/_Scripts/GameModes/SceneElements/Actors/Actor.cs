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

        public void SetInCombat(bool inCombat)
        {
            mInCombat = inCombat;

            ActorOutfitter actorOutfitter = GetComponent<ActorOutfitter>();
            actorOutfitter.UpdateHeldApparelState(mInCombat ? HumanoidActorConstants.HeldApparelState.Held : HumanoidActorConstants.HeldApparelState.Holstered);

            ActorAnimator actorAnimator = GetComponentInChildren<ActorAnimator>();
            actorAnimator.SetInCombat(mInCombat);
        }
    }
}





