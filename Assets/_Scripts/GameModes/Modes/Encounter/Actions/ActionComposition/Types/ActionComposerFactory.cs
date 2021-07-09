
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
                case (ActionId.Backstab): { action = new BackstabComposer(owner); } break;
                case (ActionId.Defend): { action = new DefendComposer(owner); } break;
                case (ActionId.DoubleTime): { action = new DoubleTimeComposer(owner); } break;
                case (ActionId.EntanglingRoots): { action = new EntanglingRootsComposer(owner); } break;
                case (ActionId.ChainLightning): { action = new ChainLightningComposer(owner); } break;
                case (ActionId.FireBall): { action = new FireBallComposer(owner); } break;
                case (ActionId.FlameStrike): { action = new FlameStrikeComposer(owner); } break;
                case (ActionId.Heal): { action = new HealComposer(owner); } break;
                case (ActionId.MeleeAttack): { action = new MeleeAttackComposer(owner); } break;
                case (ActionId.PoisonStrike): { action = new PoisonStrikeComposer(owner); } break;
                case (ActionId.Protection): { action = new ProtectComposer(owner); } break;
                case (ActionId.RangedAttack): { action = new RangedAttackComposer(owner); } break;
                case (ActionId.Regen): { action = new RegenComposer(owner); } break;
                case (ActionId.Shackle): { action = new ShackleComposer(owner); } break;
                case (ActionId.ShieldBash): { action = new ShieldBashComposer(owner); } break;
                case (ActionId.Smite): { action = new SmiteComposer(owner); } break;
                case (ActionId.SpotHeal): { action = new SpotHealComposer(owner); } break;
                case (ActionId.Sprout): { action = new SproutComposer(owner); } break;
                case (ActionId.SummonBear): { action = new SummonCompanionComposer(owner); } break;
                case (ActionId.Swipe): { action = new SwipeComposer(owner); } break;
                default:
                    Debug.Assert(false);
                    break;
            }

            ControllableEntity controllableEntity = owner as ControllableEntity;
            if (controllableEntity != null)
            {
                controllableEntity.Character.ModifyAction(action.ActionInfo);
            }

            action.Init();

            return action;
        }
    }
}


