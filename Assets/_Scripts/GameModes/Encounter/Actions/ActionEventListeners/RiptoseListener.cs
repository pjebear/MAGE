using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class RiptoseListener : ActionResponderBase
{
    public RiptoseListener(EncounterCharacter owner)
        : base(owner, ActionResponseId.Riptose)
    {

    }

    protected override void OnActionResult(ActionResult actionResult)
    {
        if (!IsListener(actionResult.Initiator)
            && WasTargeted(Listener, actionResult))
        {
            InteractionResult result = actionResult.TargetResults[Listener];
            ActionInfo info = actionResult.ActionInfo;
            if (info.ActionMedium == ActionMedium.Physical
                && info.ActionRange == ActionRange.Meele
                && info.ActionSource == ActionSource.Weapon
                && result.InteractionResultType == InteractionResultType.Parry)
            {
                TargetSelection selection = new TargetSelection(new Target(actionResult.Initiator));
                EncounterModule.ActionDirector.DirectAction(new ActionProposal(Listener, ActionId.SwordAttack, selection));
            }
        }
    }
}
