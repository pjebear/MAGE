
using MAGE.GameModes.Combat;
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
        public static ActionComposerBase GetAction(CombatEntity owner, ActionId actionId)
        {
            ActionComposerBase action = null;
            switch (actionId)
            {
                case (ActionId.MeeleWeaponAttack): { action = new AttackComposer(owner); } break;
                case (ActionId.RangedWeaponAttack): { action = new RangedAttackComposer(owner); } break;
                case (ActionId.FlameStrike): { action = new FlameStrikeComposer(owner); } break;
                case (ActionId.ChainLightning): { action = new ChainLightningComposer(owner); } break;
                case (ActionId.Regen): { action = new RegenComposer(owner); } break;
            }
            return action;
        }
        public static void ComposeAction(ActionProposal_Deprecated proposal, Map map, out GameSystems.Actions.ActionResult result, out Timeline<ActionEvent> timeline)
        {
            CharacterActorController proposerController = null;// EncounterFlowControl_Deprecated.CharacterDirector.CharacterActorLookup[proposal.Proposer];
            ActionInfoBase actionInfo = proposal.Proposer.GetActionInfo(proposal.Action);
            TargetSelection targetSelection = proposal.ActionTarget;

            switch (proposal.Action)
            {
                case (ActionId.MeeleWeaponAttack):
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
                case ActionId.SummonBear:
                {
                    SummonActionComposer.ComposeAction(proposerController, actionInfo, targetSelection, map, out result, out timeline);
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


