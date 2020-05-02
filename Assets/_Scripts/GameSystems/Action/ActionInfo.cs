using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

abstract class ActionInfo
{
    public StateChange ActionCost;
    public ActionRange ActionRange;
    public ActionSource ActionSource;
    public int CastSpeed;
    public RangeInfo CastRange;
    public RangeInfo EffectRange;
    public bool IsSelfCast;

    public ActionInfo(StateChange actionCost, ActionRange actionRange, ActionSource actionSource, int castSpeed, RangeInfo castRange, RangeInfo effectRange, bool isSelfCast)
    {
        ActionCost = actionCost;
        ActionCost.Type = StateChangeType.ActionCost;
        CastSpeed = castSpeed;
        ActionRange = actionRange;
        ActionSource = actionSource;
        CastRange = castRange;
        EffectRange = effectRange;
        IsSelfCast = isSelfCast;
    }

    public abstract StateChange GetTargetStateChange(EncounterCharacter caster, EncounterCharacter target);
}

class MeleeAttackInfo : ActionInfo
{
    public float DamageAmp = 1.0f;

    public MeleeAttackInfo(RangeInfo castRange, RangeInfo effectRange) 
        : base(new StateChange(StateChangeType.ActionCost, 0,0), ActionRange.Meele, ActionSource.Weapon,
            ActionConstants.INSTANT_CAST_SPEED, castRange, effectRange, false)
    {
    }

    public override StateChange GetTargetStateChange(EncounterCharacter caster, EncounterCharacter target)
    {
        AttributeIndex index = new AttributeIndex(AttributeCategory.Stat, (int)PrimaryStat.Might);
        int damage = -(int)(caster.Attributes[index] * DamageAmp);
        return new StateChange(StateChangeType.ActionTarget, damage, 0);
    }
}

class WeaponActionInfoBase : ActionInfo
{
    public HeldEquippable Weapon;

    public WeaponActionInfoBase(HeldEquippable weapon, StateChange actionCost, ActionRange actionRange, int castSpeed, RangeInfo castRange, RangeInfo effectRange)
        : base(actionCost, ActionRange.Meele, ActionSource.Weapon, castSpeed, castRange, effectRange, false)
    {
        Weapon = weapon;
    }

    protected float GetWeaponStrength(EncounterCharacter caster)
    {
        float damage = 0;
        foreach (AttributeScalar scalar in Weapon.EffectivenessScalars)
        {
            damage += caster.Attributes[scalar.AttributeIndex] * scalar.Scalar;
        }
        return damage;
    }

    public override StateChange GetTargetStateChange(EncounterCharacter caster, EncounterCharacter target)
    {
        return new StateChange(StateChangeType.ActionTarget, -(int)GetWeaponStrength(caster), 0);
    }
}

class HealInfo : ActionInfo
{
    public float HealAmp = 1.0f;

    public HealInfo(int castSpeed, RangeInfo castRange, RangeInfo effectRange)
        : base(new StateChange(StateChangeType.ActionCost, 0, 0), ActionRange.AOE, ActionSource.Cast, castSpeed, castRange, effectRange, false)
    {
    }

    public override StateChange GetTargetStateChange(EncounterCharacter caster, EncounterCharacter target)
    {
        AttributeIndex index = new AttributeIndex(AttributeCategory.Stat, (int)PrimaryStat.Magic);
        int heal = (int)(caster.Attributes[index] * HealAmp);
        return new StateChange(StateChangeType.ActionTarget, heal, 0);
    }
}

class ProtectionInfo : ActionInfo
{ 
    public ProtectionInfo(int castSpeed, RangeInfo castRange, RangeInfo effectRange)
        : base(new StateChange(StateChangeType.ActionCost, 0, 0), ActionRange.AOE, ActionSource.Cast, castSpeed, castRange, effectRange, false)
    {
    }

    public override StateChange GetTargetStateChange(EncounterCharacter caster, EncounterCharacter target)
    {
        ProtectionEffect effect = StatusEffectFactory.CheckoutStatusEffect(StatusEffectType.Protection, caster) as ProtectionEffect;

        return new StateChange(StateChangeType.ActionTarget, 0,0,new List<StatusEffect>() { effect });
    }
}

class MightyBlowInfo : WeaponActionInfoBase
{
    int damagePerStack = 3;

    public MightyBlowInfo(HeldEquippable weapon, StateChange actionCost, ActionRange actionRange, int castSpeed, RangeInfo castRange, RangeInfo effectRange)
        : base(weapon, actionCost, actionRange, castSpeed, castRange, effectRange)
    {
    }

    public override StateChange GetTargetStateChange(EncounterCharacter caster, EncounterCharacter target)
    {
        float baseDamage = GetWeaponStrength(caster) * .5f;
        baseDamage += caster.GetStackCountForStatus(StatusEffectType.BloodScent, caster) * damagePerStack;

        return new StateChange(StateChangeType.ActionTarget, -(int)baseDamage, 0);
    }
}
