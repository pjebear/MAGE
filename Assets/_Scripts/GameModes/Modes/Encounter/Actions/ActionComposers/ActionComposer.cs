
using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Encounter
{
    class ActionComposer
    {

        public static void ComposeAction(ActionProposal proposal, Map map, out ActionResult result, out Timeline<ActionEvent> timeline)
        {
            CharacterActorController proposerController = EncounterModule.CharacterDirector.CharacterActorLookup[proposal.Proposer];
            ActionInfo actionInfo = proposal.Proposer.GetActionInfo(proposal.Action);
            TargetSelection targetSelection = proposal.ActionTarget;

            switch (proposal.Action)
            {
                case (ActionId.WeaponAttack):
                {
                    if (actionInfo.ProjectileInfo.ProjectileId == ProjectileId.INVALID)
                    {
                        MeleeActionComposer.ComposeAction(proposerController, actionInfo, targetSelection, map, out result, out timeline);
                    }
                    else
                    {
                        ProjectileActionComposer.ComposeAction(proposerController, actionInfo, targetSelection, map, out result, out timeline);
                    }
                }
                break;
                case (ActionId.Anvil):
                case (ActionId.MightyBlow):
                case (ActionId.Riptose):
                    MeleeActionComposer.ComposeAction(proposerController, actionInfo, targetSelection, map, out result, out timeline);
                    break;

                case (ActionId.Protection):
                case (ActionId.Heal):
                case (ActionId.HolyLight):
                case (ActionId.Smite):
                case (ActionId.Shackle):
                    EffectActionComposer.ComposeAction(proposerController, actionInfo, targetSelection, map, out result, out timeline);
                    break;

                case (ActionId.FireBall):
                    ProjectileActionComposer.ComposeAction(proposerController, actionInfo, targetSelection, map, out result, out timeline);
                    break;

                default:
                    Debug.Assert(false);
                    result = null;
                    timeline = null;
                    break;
            }
        }
    }
}


