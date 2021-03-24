using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes.Combat
{
    class RiptoseResponder : ActionResponderBase
    {
        public RiptoseResponder(CombatEntity responder) : base(responder)
        {

        }

        protected override List<ActionResponseBase> GetResponsesToResult(ActionResult actionResult)
        {
            List<ActionResponseBase> responses = new List<ActionResponseBase>();

            if (!IsResponder(actionResult.Initiator)
                && WasResponderTargeted(actionResult))
            {
                InteractionResult result = actionResult.TargetResults[mResponder.GetComponent<CombatTarget>()];
                ActionInfoBase info = actionResult.ActionInfo;
                if (info.ActionRange == ActionRange.Meele
                    && info.ActionSource == ActionSource.Weapon
                    && result.InteractionResultType == InteractionResultType.Parry)
                {
                    ActionProposal proposal = new ActionProposal(
                        mResponder,
                        new Target(actionResult.Initiator.GetComponent<CombatTarget>()), 
                        ActionId.MeeleWeaponAttack);

                    responses.Add(new ActionProposalResponse(proposal));
                }
            }

            return responses;
        }
    }
}

