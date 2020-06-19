using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

abstract class ActionInfo
{
    public ActionId ActionId;
    public StateChange ActionCost;
    public ActionRange ActionRange;
    public ActionSource ActionSource;
    public int CastSpeed;
    public RangeInfo CastRange;
    public RangeInfo EffectRange;
    public bool IsSelfCast;

    public ActionInfo(ActionId actionId, StateChange actionCost, ActionRange actionRange, ActionSource actionSource, int castSpeed, RangeInfo castRange, RangeInfo effectRange, bool isSelfCast)
    {
        ActionId = actionId;
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

class WeaponActionInfoBase : ActionInfo
{
    public HeldEquippable Weapon;
    public float DamageAmp = 1;
    public WeaponActionInfoBase(ActionId actionId, HeldEquippable weapon, StateChange actionCost, ActionRange actionRange, int castSpeed, RangeInfo castRange, RangeInfo effectRange)
        : base(actionId, actionCost, ActionRange.Meele, ActionSource.Weapon, castSpeed, castRange, effectRange, false)
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
        int damage = -(int)(GetWeaponStrength(caster) * DamageAmp);
        return new StateChange(StateChangeType.ActionTarget, damage, 0);
    }
}

class HealInfo : ActionInfo
{
    public float HealAmp = 1.0f;

    public HealInfo(int castSpeed, RangeInfo castRange, RangeInfo effectRange)
        : base(ActionId.Heal, new StateChange(StateChangeType.ActionCost, 0, 0), ActionRange.AOE, ActionSource.Cast, castSpeed, castRange, effectRange, false)
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
        : base(ActionId.Protection, new StateChange(StateChangeType.ActionCost, 0, 0), ActionRange.AOE, ActionSource.Cast, castSpeed, castRange, effectRange, false)
    {
    }

    public override StateChange GetTargetStateChange(EncounterCharacter caster, EncounterCharacter target)
    {
        ProtectionEffect effect = StatusEffectFactory.CheckoutStatusEffect(StatusEffectType.Protection, caster) as ProtectionEffect;

        return new StateChange(StateChangeType.ActionTarget, 0,0,new List<StatusEffect>() { effect });
    }
}

class FireballInfo : ActionInfo
{
    public float DamageAmp = 1;
    public FireballInfo(int castSpeed, RangeInfo castRange, RangeInfo effectRange)
        : base(ActionId.FireBall, new StateChange(StateChangeType.ActionCost, 0, -5), ActionRange.Projectile, ActionSource.Cast, castSpeed, castRange, effectRange, false)
    {
    }

    public override StateChange GetTargetStateChange(EncounterCharacter caster, EncounterCharacter target)
    {
        AttributeIndex index = new AttributeIndex(AttributeCategory.Stat, (int)PrimaryStat.Magic);
        int damage = -(int)(caster.Attributes[index] * DamageAmp);
        return new StateChange(StateChangeType.ActionTarget, damage, 0);
    }
}

class MightyBlowInfo : WeaponActionInfoBase
{
    int damagePerStack = 3;

    public MightyBlowInfo(HeldEquippable weapon, StateChange actionCost, ActionRange actionRange, int castSpeed, RangeInfo castRange, RangeInfo effectRange)
        : base(ActionId.MightyBlow, weapon, actionCost, actionRange, castSpeed, castRange, effectRange)
    {
    }

    public override StateChange GetTargetStateChange(EncounterCharacter caster, EncounterCharacter target)
    {
        float baseDamage = GetWeaponStrength(caster) * .5f;
        baseDamage += caster.GetStackCountForStatus(StatusEffectType.BloodScent, caster) * damagePerStack;

        return new StateChange(StateChangeType.ActionTarget, -(int)baseDamage, 0);
    }
}
