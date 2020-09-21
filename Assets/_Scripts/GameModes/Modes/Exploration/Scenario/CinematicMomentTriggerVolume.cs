using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    class CinematicMomentTriggerVolume : PartyAvatarTriggerVolumeBase
    {
        public bool DeactivateOnTrigger = true;

        protected override void HandleTriggerEntered()
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
