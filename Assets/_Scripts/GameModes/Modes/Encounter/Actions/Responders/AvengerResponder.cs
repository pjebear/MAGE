

using MAGE.GameModes.Combat;
using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace MAGE.GameModes.Combat
{
    class AvengerResponder : ActionResponderBase
    {
        public AvengerResponder(CombatEntity responder) : base(responder)
        {

        }

        protected override List<ActionResponseBase> GetResponsesToResult(ActionResult actionResult)
        {
            float percentLossToStackCount = 0.5f;

            StateChangeType stateChangeType = StateChangeType.None;
            int stackCount = 0;

            // Is owner the one performing the action, consumer the avenger affect
            if (IsResponder(actionResult.Initiator))
            {
                stateChangeType = StateChangeType.ActionCost;

                Optional<StatusEffect> avengerEffect = mResponder.GetComponent<StatusEffectControl>().GetStatusEffect(StatusEffectId.Avenger, (mResponder as ControllableEntity).Id);
                if (avengerEffect.HasValue)
                {
                    stackCount = avengerEffect.Value.StackCount;
                }
            }
            else
            {
                stateChangeType = StateChangeType.StatusEffect;

                foreach (var targetResultPair in actionResult.TargetResults)
                {
                    CombatTarget target = targetResultPair.Key;
                    CombatEntity targetEntity = target.GetComponent<CombatEntity>();

                    if (!IsResponder(targetEntity)
                        && InRange(target.transform, Range))
                    {
                        InteractionResult result = targetResultPair.Value;
                        if (result.StateChange.healthChange < 0 // character was hurt
                            && IsAlly(targetEntity)) // character is teammate
                        {
                            float percentLoss = Mathf.Abs((result.StateChange.healthChange / (float)target.GetComponent<ResourcesControl>().Resources[ResourceType.Health].Max) * 100);
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


