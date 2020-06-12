using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;



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
                spawner.Spawn();
                NPCs.Add(spawner.ActorId, spawner.Actor);
            }
        }
    }

    public void ScenarioTriggered()
    {
        ExplorationEventRouter.Instance.NotifyEvent(new ExplorationEvent(ExplorationEvent.EventType.ScenarioTriggered, this));
    }
}

