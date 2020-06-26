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
        ActionInfo actionInfo = proposal.Owner.GetActionInfo(proposal.Action);
        EncounterActorController ownerController = EncounterModule.CharacterDirector.CharacterActorLookup[proposal.Owner];
        TargetSelection targetSelection = proposal.ActionTarget;

        switch (proposal.Action)
        {
            case (ActionId.WeaponAttack):
            {
                if (actionInfo.ProjectileInfo.ProjectileId == ProjectileId.INVALID)
                {
                    MeleeActionComposer.ComposeAction(ownerController, actionInfo, targetSelection, out result, out timeline);
                }
                else
                {
                    ProjectileActionComposer.ComposeAction(ownerController, actionInfo, targetSelection, out result, out timeline);
                }
            }
            break;
            case (ActionId.MightyBlow):
            case (ActionId.Riptose):
                MeleeActionComposer.ComposeAction(ownerController, actionInfo, targetSelection, out result, out timeline);
                break;

            case (ActionId.Protection):
            case (ActionId.Heal):
                EffectActionComposer.ComposeAction(ownerController, actionInfo, targetSelection, out result, out timeline);
                break;

            case (ActionId.FireBall):
                ProjectileActionComposer.ComposeAction(ownerController, actionInfo, targetSelection, out result, out timeline);
                break;

            default:
                Debug.Assert(false);
                result = null;
                timeline = null;
                break;
        }
    }
}

