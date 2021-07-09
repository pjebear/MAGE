using MAGE.GameModes.Encounter;
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
                foreach (InteractionResult result in actionResult.TargetResults[mResponder.GetComponent<CombatTarget>()])
                {
                    ActionInfo info = actionResult.ActionInfo;
                    if (info.ActionRange == ActionRange.Meele
                        && info.ActionSource == ActionSource.Weapon
                        && result.InteractionResultType == InteractionResultType.Parry)
                    {
                        ActionProposal proposal = new ActionProposal(
                            mResponder,
                            new Target(actionResult.Initiator.GetComponent<CombatTarget>()),
                            ActionComposerFactory.CheckoutAction(mResponder, ActionId.MeleeAttack));

                        responses.Add(new ActionProposalResponse(proposal));
                        break;
                    }
                }
            }

            return responses;
        }
    }
}

