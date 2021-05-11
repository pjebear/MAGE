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
        public NPCPropId NPCId = NPCPropId.None;

        private void Awake()
        {
            GetComponent<CharacterPickerControl>().SetRootCharacterId((int)NPCId);
        }

        public override void Start()
        {
            base.Start();
        }

        public override int GetPropId()
        {
            return (int)NPCId;
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
