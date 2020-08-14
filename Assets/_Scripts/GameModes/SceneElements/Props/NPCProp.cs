﻿using MAGE.GameServices;
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

        private Actor mNPCActor;

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
            GetComponent<ActorSpawner>().Actor.Animator.SetTrigger(AnimationId.SwordSwing.ToString());
        }

        public override void OnInteractionStart()
        {
            GetComponent<ActorSpawner>().Actor.Animator.SetTrigger(AnimationId.SwordSwing.ToString());
        }
    }
}
