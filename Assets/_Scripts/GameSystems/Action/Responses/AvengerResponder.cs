

using MAGE.GameSystems;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameSystems.Actions
{
    class AvengerResponder : ActionResponderBase
    {
        public AvengerResponder()
            : base(ActionResponseId.Avenger)
        {

        }

        protected override List<ActionResponseBase> GetResponsesToResult(Character responder, ActionResponseInfo responseInfo, ActionResult actionResult, Map map)
        {
            float percentLossToStackCount = 0.5f;

            StateChangeType stateChangeType = StateChangeType.None;
            int stackCount = 0;

            if (IsResponder(responder, actionResult.Initiator))
            {
                stateChangeType = StateChangeType.ActionCost;

                Optional<StatusEffect> avengerEffect = responder.GetStatusEffect(StatusEffectId.Avenger, responder.Id);
                if (avengerEffect.HasValue)
                {
                    stackCount = avengerEffect.Value.StackCount;
                }
            }
            else
            {
                stateChangeType = StateChangeType.StatusEffect;

                foreach (var interactionResult in actionResult.TargetResults)
                {
                    Character target = interactionResult.Key;
                    if (!IsResponder(responder, target)
                        && InRange(responder, target, responseInfo.Range, map))
                    {
                        InteractionResult result = interactionResult.Value;
                        if (result.StateChange.healthChange < 0 // character was hurt
                            && IsAlly(responder, target)) // character is teammate
                        {
                            float percentLoss = Mathf.Abs((result.StateChange.healthChange / (float)target.CurrentResources[ResourceType.Health].Max) * 100);
                            stackCount += (int)(percentLoss * percentLossToStackCount);
                        }
                    }
                }
            }

            List<ActionResponseBase> responses = new List<ActionResponseBase>();
            if (stackCount > 0)
            {
                StateChange stateChange = new StateChange(stateChangeType, new List<StatusEffect>() { StatusEffectFactory.CheckoutStatusEffect(StatusEffectId.Avenger, stackCount) });
                responses.Add(new StateChangeResponse(stateChange));
            }
            return responses;
        }
    }
}

