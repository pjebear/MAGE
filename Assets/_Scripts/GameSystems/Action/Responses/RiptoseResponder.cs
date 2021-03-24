using MAGE.GameSystems;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Actions
{
    class RiptoseResponder : ActionResponderBase
    {
        public RiptoseResponder()
            : base(ActionResponseId.Riptose)
        {

        }

        protected override List<ActionResponseBase> GetResponsesToResult(Character responder, ActionResponseInfo responseInfo, ActionResult actionResult, Map map)
        {
            List<ActionResponseBase> responses = new List<ActionResponseBase>();

            if (!IsResponder(responder, actionResult.Initiator)
                && WasCharacterATarget(responder, actionResult))
            {
                InteractionResult result = actionResult.TargetResults[responder];
                ActionInfoBase info = actionResult.ActionInfo;
                if (info.ActionRange == ActionRange.Meele
                    && info.ActionSource == ActionSource.Weapon
                    && result.InteractionResultType == InteractionResultType.Parry)
                {
                    TargetSelection selection = new TargetSelection(new Target(actionResult.Initiator));
                    responses.Add(new ActionProposalResponse_Deprecated(new ActionProposal_Deprecated(responder, ActionId.MeeleWeaponAttack, selection)));
                }
            }

            return responses;
        }
    }
}

