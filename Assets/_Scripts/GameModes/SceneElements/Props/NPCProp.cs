using MAGE.GameSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    class NPCProp : PropBase
    {
        [HideInInspector]
        public NPCPropId NPCId = NPCPropId.None;

        public override int GetPropId()
        {
            return (int)NPCId;
        }

        protected override void Refresh()
        {
            base.Refresh();

            GetComponent<CharacterPickerControl>().CharacterId = PropInfo.AppearanceId;
        }

        public override void OnInteractionEnd()
        {
            GetComponent<ActorAnimator>().Trigger(AnimationId.SwordSwing.ToString());
        }

        public override void OnInteractionStart()
        {
            GetComponent<ActorAnimator>().Trigger(AnimationId.SwordSwing.ToString());
        }
    }
}
