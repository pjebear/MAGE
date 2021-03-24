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
    class HealOnHurtResponder : ActionResponderBase
    {
        public HealOnHurtResponder()
            : base(ActionResponseId.HealOnHurtListener)
        {

        }

        protected override List<ActionResponseBase> GetResponsesToResult(Character responder, ActionResponseInfo responseInfo, ActionResult actionResult, Map map)
        {
            List<ActionResponseBase> responses = new List<ActionResponseBase>();

            // Don't be healing people that we just hurt!
            if (!IsResponder(responder, actionResult.Initiator))
            {
                foreach (var targetResultPair in actionResult.TargetResults)
                {
                    Character target = targetResultPair.Key;
                    InteractionResult result = targetResultPair.Value;
                    if (!IsResponder(responder, target) // responder wasn't the one who got hurt
                        && IsAlly(responder, target) // don't heal enemies
                        && WasHurt(result)
                        && IsAlive(target)
                        && InRange(responder, target, responseInfo.Range, map))
                    {
                        TargetSelection selection = new TargetSelection(new Target(target));
                        responses.Add(new ActionProposalResponse_Deprecated(new ActionProposal_Deprecated(responder, ActionId.Heal, selection)));
                    }
                }
            }

            return responses;
        }
    }
}


