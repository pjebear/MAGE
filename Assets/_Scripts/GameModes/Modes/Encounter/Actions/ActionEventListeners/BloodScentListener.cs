
using MAGE.GameServices;
using MAGE.GameServices.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Encounter
{
    class BloodScentListener : ActionResponderBase
    {
        public BloodScentListener(EncounterCharacter owner)
            : base(owner, ActionResponseId.BloodScent)
        {

        }

        protected override void OnActionResult(ActionResult actionResult)
        {
            StatusEffect bloodScent = null;

            if (IsListener(actionResult.Initiator))
            {
                foreach (var targetResultPair in actionResult.TargetResults)
                {
                    InteractionResult result = targetResultPair.Value;
                    if (result.StateChange.healthChange < 0)
                    {
                        bloodScent = StatusEffectFactory.CheckoutStatusEffect(StatusEffectType.BloodScent, Listener);
                    }
                }
            }
            else if (actionResult.TargetResults.ContainsKey(Listener))
            {
                InteractionResult result = actionResult.TargetResults[Listener];
                if (result.StateChange.healthChange < 0)
                {
                    bloodScent = StatusEffectFactory.CheckoutStatusEffect(StatusEffectType.BloodScent, Listener, 2);
                }
            }

            if (bloodScent != null)
            {
                EncounterModule.CharacterDirector.CharacterActorLookup[Listener].DisplayStatusApplication(bloodScent);
                Listener.ApplyStateChange(new StateChange(StateChangeType.StatusEffect, new List<StatusEffect>() { bloodScent }));
            }
        }
    }

}
