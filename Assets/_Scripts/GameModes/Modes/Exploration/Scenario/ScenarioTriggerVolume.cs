using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    class ScenarioTriggerVolume : TriggerVolumeBase<ThirdPersonActorController>
    {
        protected override int GetLayer()
        {
            return (int)Layer.Scenario;
        }

        protected override void HandleTriggerEntered(ThirdPersonActorController entered)
        {
            GetComponentInParent<Scenario>().ScenarioTriggered();
        }
    }

   
}


