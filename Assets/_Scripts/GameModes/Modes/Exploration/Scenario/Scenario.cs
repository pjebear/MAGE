﻿using MAGE.GameModes.Exploration;
using MAGE.GameModes.SceneElements;
using MAGE.GameServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace MAGE.GameModes.SceneElements
{
    class Scenario : MonoBehaviour
    {
        public ScenarioId ScenarioId = ScenarioId.INVALID;
        public Transform NPCContainer;
        public Dictionary<int, Actor> NPCs = new Dictionary<int, Actor>();

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            for (int i = 0; i < NPCContainer.childCount; ++i)
            {
                ActorSpawner spawner = NPCContainer.GetChild(i).GetComponent<ActorSpawner>();
                if (spawner != null)
                {
                    spawner.Refresh();
                    NPCs.Add(spawner.GetActorId(), spawner.Actor);
                }
            }
        }

        public void ScenarioTriggered()
        {
            Messaging.MessageRouter.Instance.NotifyMessage(new ExplorationMessage(ExplorationMessage.EventType.ScenarioTriggered, this));
        }
    }
}


