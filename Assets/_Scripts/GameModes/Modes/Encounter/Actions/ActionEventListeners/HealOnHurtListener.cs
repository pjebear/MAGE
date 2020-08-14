using MAGE.GameServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Encounter
{
    class HealOnHurtListener : ActionResponderBase
    {
        public HealOnHurtListener(EncounterCharacter owner)
            : base(owner, ActionResponseId.HealOnHurtListener)
        {

        }

        protected override void OnActionResult(ActionResult actionResult)
        {
            // Don't be healing people that we just hurt!
            if (IsListener(actionResult.Initiator))
            {
                return;
            }

            foreach (var targetResultPair in actionResult.TargetResults)
            {
                EncounterCharacter actor = targetResultPair.Key;
                InteractionResult result = targetResultPair.Value;
                if (!IsListener(actor)
                    && IsAlly(actor)
                    && WasHurt(result)
                    && IsAlive(actor)
                    && InRange(actor))
                {
                    TargetSelection selection = new TargetSelection(new Target(actor));

                    EncounterModule.ActionDirector.DirectAction(new ActionProposal(Listener, ActionId.Heal, selection));
                }
            }
        }
    }
}


