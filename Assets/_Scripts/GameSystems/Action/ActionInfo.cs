using MAGE.GameModes.Encounter;

using MAGE.GameServices.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameServices
{
    abstract class ActionInfo
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
        public bool IsSelfCast = false;

        public abstract StateChange GetTargetStateChange(EncounterCharacter caster, EncounterCharacter target);
    }

    class WeaponActionInfoBase : ActionInfo
    {
        public override StateChange GetTargetStateChange(EncounterCharacter caster, EncounterCharacter target)
        {
            float baseEffectiveness = EquipmentUtil.GetHeldEquippableEffectiveness(caster.Attributes, caster.Equipment[Equipment.Slot.RightHand] as HeldEquippable);
            int damage = -(int)(baseEffectiveness * Effectiveness);
            return new StateChange(StateChangeType.ActionTarget, damage, 0);
        }
    }


    class ProtectionInfo : ActionInfo
    {
        public override StateChange GetTargetStateChange(EncounterCharacter caster, EncounterCharacter target)
        {
            ProtectionEffect effect = StatusEffectFactory.CheckoutStatusEffect(StatusEffectType.Protection, caster) as ProtectionEffect;

            return new StateChange(StateChangeType.ActionTarget, 0, 0, new List<StatusEffect>() { effect });
        }
    }

    class SpellInfoBase : ActionInfo
    {
        bool mIsBeneficial = false;
        StatusEffectType mStatusEffectType = StatusEffectType.INVALID;

        public SpellInfoBase(StatusEffectType effectType)
        {
            Effectiveness = 0;
            mStatusEffectType = effectType;
        }

        public SpellInfoBase(float strength, bool isBeneficial)
        {
            Effectiveness = strength;
            mIsBeneficial = isBeneficial;
        }

        public override StateChange GetTargetStateChange(EncounterCharacter caster, EncounterCharacter target)
        {
            float healthChange = 0;
            healthChange += caster.Attributes[PrimaryStat.Magic];
            healthChange *= 1 + (caster.Attributes[SecondaryStat.Attunement] / 100);

            healthChange *= Effectiveness;
            healthChange *= mIsBeneficial ? 1 : -1;

            List<StatusEffect> statusEffects = new List<StatusEffect>();
            if (mStatusEffectType != StatusEffectType.INVALID)
            {
                statusEffects.Add(StatusEffectFactory.CheckoutStatusEffect(mStatusEffectType, caster));
            }

            return new StateChange(StateChangeType.ActionTarget, (int)healthChange, 0, statusEffects);
        }
    }

    class MightyBlowInfo : WeaponActionInfoBase
    {
        float DamagePercentPerStack = .2f;

        public override StateChange GetTargetStateChange(EncounterCharacter caster, EncounterCharacter target)
        {
            float baseEffectiveness = EquipmentUtil.GetHeldEquippableEffectiveness(caster.Attributes, caster.Equipment[Equipment.Slot.RightHand] as HeldEquippable);
            baseEffectiveness *= 1 + (DamagePercentPerStack * caster.GetStackCountForStatus(StatusEffectType.BloodScent, caster));

            return new StateChange(StateChangeType.ActionTarget, -(int)baseEffectiveness, 0);
        }
    }

}