﻿using MAGE.GameModes.SceneElements;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Resources = MAGE.GameSystems.Stats.Resources;

namespace MAGE.GameModes.Combat
{
    class ResourcesControl : MonoBehaviour
    {
        public Resources Resources = new Resources();

        public void OnStatsUpdated()
        {
            Attributes attributes = GetComponent<StatsControl>().Attributes;
            for (int i = 0; i < (int)ResourceType.NUM; ++i)
            {
                ResourceType resource = (ResourceType)i;
                Resources[resource].SetMax(AttributeUtil.ResourceFromAttribtues(resource, attributes));
            }
        }

        public void ApplyStateChange(StateChange stateChange)
        {
            Resources[ResourceType.Health].Modify(stateChange.healthChange);
            Resources[ResourceType.Mana].Modify(stateChange.resourceChange);
            Resources[ResourceType.Endurance].Modify(stateChange.resourceChange);

            if (stateChange.healthChange != 0)
            {
                Billboard.Params param = new Billboard.Params();
                param.anchor = transform;
                param.offset = new Vector3(1f, 2f, 0);
                param.text = stateChange.healthChange.ToString();
                GetComponent<BillboardEmitter>().Emitt(param, 2f);
            }

            if (Resources[ResourceType.Health].Current <= 0)
            {
                BroadcastMessage("OnDeath");
            }
        }

        public bool IsAlive()
        {
            return Resources[GameSystems.Stats.ResourceType.Health].Current > 0;
        }

        public void OnDeath()
        {
            GetComponent<ActorAnimator>().Trigger("die");
            GetComponent<CapsuleCollider>().enabled = false;
        }
    }
}