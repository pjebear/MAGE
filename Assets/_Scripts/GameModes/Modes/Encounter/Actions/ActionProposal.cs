using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class ActionProposal
{
    public EncounterCharacter Owner;
    public ActionId Action;
    public TargetSelection ActionTarget;

    public ActionProposal(EncounterCharacter owner, ActionId action, TargetSelection actionTarget)
    {
        Owner = owner;
        Action = action;
        ActionTarget = actionTarget;
    }
}
