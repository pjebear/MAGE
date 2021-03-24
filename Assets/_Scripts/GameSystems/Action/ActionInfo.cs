using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Items;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Actions
{
    abstract class ActionInfoBase
    {
        public float Effectiveness = 1f;
        public ActionId ActionId = ActionId.INVALID;
        public StateChange ActionCost = StateChange.Empty;
        public ActionRange ActionRange = ActionRange.NUM;
        public ActionSource ActionSource = ActionSource.NUM;
        public CastSpeed CastSpeed = CastSpeed.INVALID;
        public RangeInfo CastRange = RangeInfo.Unit;
        public RangeInfo EffectRange = RangeInfo.Unit;
        public ActionAnimationInfo AnimationInfo = new ActionAnimationInfo();
        public ActionProjectileInfo ProjectileInfo = new ActionProjectileInfo();
        public ActionEffectInfo EffectInfo = new ActionEffectInfo();
        public ActionChainInfo ChainInfo = new ActionChainInfo();
        public ActionSummonInfo SummonInfo = new ActionSummonInfo();
        public bool IsSelfCast = false;

        public abstract StateChange GetTargetStateChange(Character caster, Character target);

        public bool CanCast(Character caster)
        {
            bool canCast = caster.HasResourcesForAction(ActionCost);

            if (ActionSource == ActionSource.Weapon)
            {
                canCast &= caster.CurrentAttributes[StatusType.Disarmed] == 0;
            }
            else
            {
                canCast &= caster.CurrentAttributes[StatusType.Silenced] == 0;
            }

            if (SummonInfo.SummonType != SpecializationType.INVALID)
            {
                int summonCount = caster.Children.FindAll(x => x.CurrentSpecializationType == SummonInfo.SummonType).Count;
                canCast &= summonCount <= SummonInfo.MaxSummonCount;
            }

            return canCast;
        }
    }

    [System.Serializable]
    class ActionInfo : ActionInfoBase
    {
        public override StateChange GetTargetStateChange(Character caster, Character target)
        {
            throw new NotImplementedException();
        }
    }

    class WeaponActionInfoBase : ActionInfoBase
    {
        public override StateChange GetTargetStateChange(Character caster, Character target)
        {
            float baseEffectiveness = EquipmentUtil.GetHeldEquippableEffectiveness(caster.CurrentAttributes, caster.Equipment[Equipment.Slot.RightHand] as HeldEquippable);
            int damage = -(int)(baseEffectiveness * Effectiveness);
            return new StateChange(StateChangeType.ActionTarget, damage, 0);
        }
    }


    class ProtectionInfo : ActionInfoBase
    {
        public override StateChange GetTargetStateChange(Character caster, Character target)
        {
            ProtectionEffect effect = StatusEffectFactory.CheckoutStatusEffect(StatusEffectId.Protection) as ProtectionEffect;

            return new StateChange(StateChangeType.ActionTarget, 0, 0, new List<StatusEffect>() { effect });
        }
    }

    class SummonInfoBase : ActionInfoBase
    {
        public SpecializationType SummonType = SpecializationType.Bear;

        public SummonInfoBase(SpecializationType summonType)
            : base()
        {
            SummonType = summonType;
        }

        public override StateChange GetTargetStateChange(Character caster, Character target)
        {
            return StateChange.Empty;
        }
    }

    class SpellInfoBase : ActionInfoBase
    {
        bool mIsBeneficial = false;
        StatusEffectId mStatusEffectType = StatusEffectId.INVALID;

        public SpellInfoBase(StatusEffectId effectType)
        {
            Effectiveness = 0;
            mStatusEffectType = effectType;
        }

        public SpellInfoBase(float strength, bool isBeneficial)
        {
            Effectiveness = strength;
            mIsBeneficial = isBeneficial;
        }

        public override StateChange GetTargetStateChange(Character caster, Character target)
        {
            float healthChange = 0;
            healthChange += caster.CurrentAttributes[PrimaryStat.Magic];
            healthChange *= 1 + (caster.CurrentAttributes[SecondaryStat.Attunement] / 100);

            healthChange *= Effectiveness;
            healthChange *= mIsBeneficial ? 1 : -1;

            List<StatusEffect> statusEffects = new List<StatusEffect>();
            if (mStatusEffectType != StatusEffectId.INVALID)
            {
                statusEffects.Add(StatusEffectFactory.CheckoutStatusEffect(mStatusEffectType));
            }

            return new StateChange(StateChangeType.ActionTarget, (int)healthChange, 0, statusEffects);
        }
    }

    class MightyBlowInfo : WeaponActionInfoBase
    {
        float DamagePercentPerStack = .2f;

        public override StateChange GetTargetStateChange(Character caster, Character target)
        {
            float baseEffectiveness = EquipmentUtil.GetHeldEquippableEffectiveness(caster.CurrentAttributes, caster.Equipment[Equipment.Slot.RightHand] as HeldEquippable);
            baseEffectiveness *= 1 + (DamagePercentPerStack * caster.GetStackCountForStatus(StatusEffectId.BloodScent, caster));

            return new StateChange(StateChangeType.ActionTarget, -(int)baseEffectiveness, 0);
        }
    }

}