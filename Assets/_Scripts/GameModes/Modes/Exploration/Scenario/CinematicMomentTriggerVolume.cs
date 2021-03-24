using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    class CinematicMomentTriggerVolume : TriggerVolumeBase<ThirdPersonActorController>
    {
        public bool DeactivateOnTrigger = true;

        protected override int GetLayer()
        {
            return (int)Layer.Default;
        }

        protected override void HandleTriggerEntered(ThirdPersonActorController partyAvatar)
        {
            CinematicMoment cinematicMoment = GetComponentInParent<CinematicMoment>();
            Debug.Assert(cinematicMoment != null);

            if (cinematicMoment != null)
            {
                cinematicMoment.CinematicTriggered();
            }

            if (DeactivateOnTrigger)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
