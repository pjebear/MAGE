using MAGE.GameModes.Combat;
using MAGE.GameSystems;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Actions
{
    class ActionProposal
    {
        public CombatEntity Proposer;
        public Target Target;
        public ActionId Action;

        public ActionProposal(CombatEntity proposer, Target target, ActionId action)
        {
            Action = action;
            Proposer = proposer;
            Target = target;
        }
    }

    class ActionProposal_Deprecated
    {
        public Character Proposer;
        public ActionId Action;
        public TargetSelection ActionTarget;

        public ActionProposal_Deprecated(Character proposer, ActionId action, TargetSelection actionTarget)
        {
            Proposer = proposer;
            Action = action;
            ActionTarget = actionTarget;
        }
    }

    static class ActionProposalUtil
    {
        public static bool IsProposalValid(ActionProposal_Deprecated proposal)
        {
            return DoesActionHaveValidTargets(proposal)
                && DoesProposerHaveSufficientResources(proposal.Proposer, proposal.Proposer.GetActionInfo(proposal.Action));
        }

        public static bool DoesProposerHaveSufficientResources(Character owner, ActionInfoBase actionInfo)
        {
            bool isValidState =
                owner.CurrentResources[ResourceType.Health].Current > actionInfo.ActionCost.healthChange
                && owner.CurrentResources[ResourceType.Mana].Current >= actionInfo.ActionCost.resourceChange;

            return isValidState;
        }

        public static bool DoesActionHaveValidTargets(ActionProposal_Deprecated proposal)
        {
            return true;
        }
    }
}
