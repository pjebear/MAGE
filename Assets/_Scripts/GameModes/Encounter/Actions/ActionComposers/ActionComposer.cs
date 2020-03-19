using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ActionComposer
{

    public static void ComposeAction(ActionProposal proposal, out ActionResult result, out Timeline<ActionEvent> timeline)
    {
        switch (proposal.Action)
        {
            case (ActionId.MightyBlow):
            case (ActionId.SwordAttack):
            case (ActionId.Riptose):
                SwordAttackComposer.ComposeAction(proposal, out result, out timeline);
                break;

            case (ActionId.Heal):
                HealActionComposer.ComposeAction(proposal, out result, out timeline);
                break;

            case (ActionId.Protection):
                ProtectionActionComposer.ComposeAction(proposal, out result, out timeline);
                break;
            default:
                Debug.Assert(false);
                result = null;
                timeline = null;
                break;
        }
    }
}

