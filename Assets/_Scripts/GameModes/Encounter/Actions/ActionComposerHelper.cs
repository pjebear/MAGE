using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


static class ActionComposerHelper
{
    public static bool IsProposalValid(ActionProposal proposal)
    {
        return DoesActionHaveValidTargets(proposal) 
            && DoesOwnerHaveSufficientResources(proposal.Owner, proposal.Owner.GetActionInfo(proposal.Action));
    }

    public static bool DoesOwnerHaveSufficientResources(EncounterCharacter owner, ActionInfo actionInfo)
    {
        bool isValidState =
            owner.Resources[ResourceType.Health].Current > actionInfo.ActionCost.healthChange
            && owner.Resources[ResourceType.Mana].Current >= actionInfo.ActionCost.resourceChange;

        return isValidState;
    }

    public static bool DoesActionHaveValidTargets(ActionProposal proposal)
    {
        return true;
    }

}

