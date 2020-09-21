using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Actions
{
    class ActionFactory
    {
        static StateChange NO_COST = new StateChange(StateChangeType.ActionCost, 0, 0);
        static StateChange SPELL_COST = new StateChange(StateChangeType.ActionCost, 0, -6);

        public static ActionInfo CreateActionInfoFromId(ActionId actionId, Character character)
        {
            DB.DBAction dbAction = DBService.Get().LoadAction((int)actionId);
            ActionInfo info = null;

            switch (actionId)
            {
                case (ActionId.Anvil):
                {
                    info = new WeaponActionInfoBase();
                    ActionUtil.FromDB(dbAction, info);
                    info.ActionCost = SPELL_COST;
                }
                break;

                case (ActionId.Heal):
                {
                    info = new SpellInfoBase(1, true);
                    ActionUtil.FromDB(dbAction, info);
                    info.ActionCost = SPELL_COST;
                }
                break;

                case (ActionId.HolyLight):
                {
                    info = new SpellInfoBase(.75f, true);
                    ActionUtil.FromDB(dbAction, info);
                    info.ActionCost = SPELL_COST;
                }
                break;

                case (ActionId.FireBall):
                {
                    info = new SpellInfoBase(1, false);
                    ActionUtil.FromDB(dbAction, info);
                    info.ActionCost = SPELL_COST;
                }
                break;

                case (ActionId.Protection):
                {
                    info = new ProtectionInfo();
                    ActionUtil.FromDB(dbAction, info);
                    info.ActionCost = SPELL_COST;
                }
                break;

                case (ActionId.MightyBlow):
                {
                    int bloodScentCount = character.GetStackCountForStatus(StatusEffectId.BloodScent, character);
                    StatusEffect bloodScentCost = StatusEffectFactory.CheckoutStatusEffect(StatusEffectId.BloodScent, character, bloodScentCount);
                    StateChange cost = new StateChange(StateChangeType.ActionCost, 0, 0, new List<StatusEffect>() { bloodScentCost });

                    info = new MightyBlowInfo();
                    ActionUtil.FromDB(dbAction, info);
                    info.ActionCost = cost;
                }
                break;

                case (ActionId.Shackle):
                {
                    info = new SpellInfoBase(StatusEffectId.Shackle);
                    ActionUtil.FromDB(dbAction, info);
                    info.ActionCost = SPELL_COST;
                }
                break;

                case (ActionId.Smite):
                {
                    info = new SpellInfoBase(.75f, false);
                    ActionUtil.FromDB(dbAction, info);
                    info.ActionCost = SPELL_COST;
                }
                break;

                case (ActionId.SummonBear):
                {
                    info = new SummonInfoBase(SpecializationType.Bear);
                    ActionUtil.FromDB(dbAction, info);
                    info.ActionCost = SPELL_COST;
                }
                break;

                case (ActionId.WeaponAttack):
                {
                    WeaponEquippable weapon = (WeaponEquippable)character.Equipment[Equipment.Slot.RightHand];

                    StateChange cost = new StateChange(StateChangeType.ActionCost, 0, 0);
                    ActionRange actionRange = weapon.ProjectileInfo.ProjectileId == ProjectileId.INVALID ? ActionRange.Meele : ActionRange.Projectile;

                    info = new WeaponActionInfoBase();
                    ActionUtil.FromDB(dbAction, info);
                    info.ActionCost = NO_COST;
                    info.ProjectileInfo = weapon.ProjectileInfo;
                    info.AnimationInfo = weapon.AnimationInfo;
                    info.CastRange = weapon.Range;
                    info.ActionRange = weapon.ProjectileInfo.ProjectileId == ProjectileId.INVALID ? ActionRange.Meele : ActionRange.Projectile;
                }
                break;

                default:
                    throw new Exception();
            }

            return info;
        }

        public static ActionResponseInfo CreateActionResponseInfoFromId(ActionResponseId responseId)
        {
            ActionResponseInfo info = null;

            switch (responseId)
            {
                case (ActionResponseId.Riptose):
                {
                    info = new ActionResponseInfo(25, 1);
                }
                break;

                case (ActionResponseId.Avenger):
                {
                    info = new ActionResponseInfo(100, 3);
                }
                break;

                case (ActionResponseId.HealOnHurtListener):
                {
                    info = new ActionResponseInfo(100, 3);
                }
                break;

                case (ActionResponseId.BloodScent):
                {
                    info = new ActionResponseInfo(100, ActionResponseInfo.INFINITE_RANGE);
                }
                break;

                default:
                    throw new Exception();
            }

            return info;
        }
    }
}


