

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
    class AvengerListener : ActionResponderBase
    {
        public AvengerListener(EncounterCharacter owner)
            : base(owner, ActionResponseId.Avenger)
        {

        }

        protected override void OnActionResult(ActionResult actionResult)
        {
            float percentLossToStackCount = 0.5f;
            StatusEffect effect = null;

            StateChangeType stateChangeType = StateChangeType.None;
            int stackCount = 0;

            if (IsListener(actionResult.Initiator))
            {
                stateChangeType = StateChangeType.ActionCost;

                StatusEffect avengerEffect = Listener.StatusEffects.Find(x => x.EffectType == StatusEffectType.Avenger);
                if (avengerEffect != null)
                {
                    stackCount = avengerEffect.StackCount;
                }
            }
            else
            {
                stateChangeType = StateChangeType.StatusEffect;

                foreach (var interactionResult in actionResult.TargetResults)
                {
                    EncounterCharacter target = interactionResult.Key;
                    if (!IsListener(target))
                    {
                        InteractionResult result = interactionResult.Value;
                        if (result.StateChange.healthChange < 0 // character was hurt
                            && target.Team == Listener.Team) // character is teammate
                        {
                            float percentLoss = Mathf.Abs((result.StateChange.healthChange / (float)target.Resources[ResourceType.Health].Max) * 100);
                            stackCount += (int)(percentLoss * percentLossToStackCount);
                        }
                    }
                }
            }

            if (stackCount > 0)
            {
                effect = StatusEffectFactory.CheckoutStatusEffect(StatusEffectType.Avenger, Listener, stackCount);
                EncounterModule.CharacterDirector.CharacterActorLookup[Listener].DisplayStatusApplication(effect);
                Listener.ApplyStateChange(new StateChange(stateChangeType, new List<StatusEffect>() { effect }));
            }
        }
    }
}

