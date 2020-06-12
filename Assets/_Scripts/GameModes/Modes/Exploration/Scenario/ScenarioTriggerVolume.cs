using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ScenarioTriggerVolume : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.GetComponent<ThirdPersonActorController>() != null)
        {
            GetComponentInParent<Scenario>().ScenarioTriggered();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.GetComponent<ThirdPersonActorController>() != null)
        {
            GetComponentInParent<Scenario>().ScenarioTriggered();
        }
    }
}

