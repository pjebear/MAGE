using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    class EncounterTriggerVolume : TriggerVolumeBase<ThirdPersonActorController>
    {
        public bool DeactivateOnTrigger = true;

        protected override int GetLayer()
        {
            return (int)Layer.Default;
        }

        protected override void HandleTriggerEntered(ThirdPersonActorController partyAvatar)
        {
            EncounterContainer encounter = GetComponentInParent<EncounterContainer>();
            Debug.Assert(encounter != null);

            if (encounter != null)
            {
                encounter.EncounterTriggered();
            }

            if (DeactivateOnTrigger)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
