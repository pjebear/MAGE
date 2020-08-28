
using MAGE.GameSystems;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameSystems.Actions
{
    class BloodScentResponder : ActionResponderBase
    {
        public BloodScentResponder()
            : base(ActionResponseId.BloodScent)
        {

        }

        protected override List<ActionResponseBase> GetResponsesToResult(Character responder, ActionResponseInfo responseInfo, ActionResult actionResult, Map map)
        {
            StatusEffect bloodScent = null;

            // Did the responder initiate the action
            if (IsResponder(responder, actionResult.Initiator))
            {
                foreach (var targetResultPair in actionResult.TargetResults)
                {
                    InteractionResult result = targetResultPair.Value;
                    if (result.StateChange.healthChange < 0)
                    {
                        bloodScent = StatusEffectFactory.CheckoutStatusEffect(StatusEffectId.BloodScent, responder);
                    }
                }
            }
            // Was the responder a target of the action
            else if (WasCharacterATarget(responder, actionResult))
            {
                InteractionResult result = actionResult.TargetResults[responder];
                if (result.StateChange.healthChange < 0)
                {
                    bloodScent = StatusEffectFactory.CheckoutStatusEffect(StatusEffectId.BloodScent, responder, 2);
                }
            }

            List<ActionResponseBase> responses = new List<ActionResponseBase>();
            if (bloodScent != null)
            {
                responses.Add(new StateChangeResponse(new StateChange(StateChangeType.StatusEffect, new List<StatusEffect>() { bloodScent })));
            }
            return responses;
        }
    }

}
