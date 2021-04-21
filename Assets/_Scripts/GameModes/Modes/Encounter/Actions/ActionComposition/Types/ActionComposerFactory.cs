
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
    static class ActionComposerFactory
    {
        public static ActionComposerBase CheckoutAction(CombatEntity owner, ActionId actionId)
        {
            ActionComposerBase action = null;
            switch (actionId)
            {
                case (ActionId.Anvil): { action = new AnvilComposer(owner); } break;
                case (ActionId.MeeleWeaponAttack): { action = new AttackComposer(owner); } break;
                case (ActionId.RangedWeaponAttack): { action = new RangedAttackComposer(owner); } break;
                case (ActionId.ChainLightning): { action = new ChainLightningComposer(owner); } break;
                case (ActionId.FireBall): { action = new FireBallComposer(owner); } break;
                case (ActionId.FlameStrike): { action = new FlameStrikeComposer(owner); } break;
                case (ActionId.Heal): { action = new HealComposer(owner); } break;
                case (ActionId.Protection): { action = new ProtectComposer(owner); } break;
                case (ActionId.Regen): { action = new RegenComposer(owner); } break;
                case (ActionId.Smite): { action = new SmiteComposer(owner); } break;
                default:
                    Debug.Assert(false);
                    break;
            }
            return action;
        }
    }
}


