using MAGE.GameSystems;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Actions
{
    class ActionProposal
    {
        public Character Proposer;
        public ActionId Action;
        public TargetSelection ActionTarget;

        public ActionProposal(Character proposer, ActionId action, TargetSelection actionTarget)
        {
            Proposer = proposer;
            Action = action;
            ActionTarget = actionTarget;
        }
    }

    static class ActionProposalUtil
    {
        public static bool IsProposalValid(ActionProposal proposal)
        {
            return DoesActionHaveValidTargets(proposal)
                && DoesProposerHaveSufficientResources(proposal.Proposer, proposal.Proposer.GetActionInfo(proposal.Action));
        }

        public static bool DoesProposerHaveSufficientResources(Character owner, ActionInfo actionInfo)
        {
            bool isValidState =
                owner.CurrentResources[ResourceType.Health].Current > actionInfo.ActionCost.healthChange
                && owner.CurrentResources[ResourceType.Mana].Current >= actionInfo.ActionCost.resourceChange;

            return isValidState;
        }

        public static bool DoesActionHaveValidTargets(ActionProposal proposal)
        {
            return true;
        }
    }
}
