using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

abstract class ActionInfo
{
    public StateChange ActionCost;
    public ActionMedium ActionMedium;
    public ActionRange ActionRange;
    public ActionSource ActionSource;
    public RangeInfo CastRange;
    public RangeInfo EffectRange;
    public bool IsSelfCast;

    public ActionInfo(StateChange actionCost, ActionMedium actionMedium, ActionRange actionRange, ActionSource actionSource, RangeInfo castRange, RangeInfo effectRange, bool isSelfCast)
    {
        ActionCost = actionCost;
        ActionCost.Type = StateChangeType.ActionCost;

        ActionMedium = actionMedium;
        ActionRange = actionRange;
        ActionSource = actionSource;
        CastRange = castRange;
        EffectRange = effectRange;
        IsSelfCast = isSelfCast;
    }

    public abstract StateChange GetStateChange(EncounterCharacter caster, EncounterCharacter target);
}

class MeleeAttackInfo : ActionInfo
{
    public float DamageAmp = 1.0f;

    public MeleeAttackInfo(RangeInfo castRange, RangeInfo effectRange) 
        : base(new StateChange(StateChangeType.ActionCost, 0,0), ActionMedium.Physical, ActionRange.Meele, ActionSource.Weapon, castRange, effectRange, false)
    {
    }

    public override StateChange GetStateChange(EncounterCharacter caster, EncounterCharacter target)
    {
        AttributeIndex index = new AttributeIndex(AttributeCategory.Stat, (int)PrimaryStat.Might);
        int damage = -(int)(caster.Attributes[index] * DamageAmp) - 99;
        return new StateChange(StateChangeType.ActionTarget, damage, 0);
    }
}

class HealInfo : ActionInfo
{
    public float HealAmp = 1.0f;

    public HealInfo(RangeInfo castRange, RangeInfo effectRange)
        : base(new StateChange(StateChangeType.ActionCost, 0, 0), ActionMedium.Magical, ActionRange.AOE, ActionSource.Cast, castRange, effectRange, false)
    {
    }

    public override StateChange GetStateChange(EncounterCharacter caster, EncounterCharacter target)
    {
        AttributeIndex index = new AttributeIndex(AttributeCategory.Stat, (int)PrimaryStat.Magic);
        int heal = (int)(caster.Attributes[index] * HealAmp);
        return new StateChange(StateChangeType.ActionTarget, heal, 0);
    }
}

class ProtectionInfo : ActionInfo
{ 
    public ProtectionInfo(RangeInfo castRange, RangeInfo effectRange)
        : base(new StateChange(StateChangeType.ActionCost, 0, 0), ActionMedium.Magical, ActionRange.AOE, ActionSource.Cast, castRange, effectRange, false)
    {
    }

    public override StateChange GetStateChange(EncounterCharacter caster, EncounterCharacter target)
    {
        ProtectionEffect effect = StatusEffectFactory.CheckoutStatusEffect(StatusEffectType.Protection, caster) as ProtectionEffect;

        return new StateChange(StateChangeType.ActionTarget, 0,0,new List<StatusEffect>() { effect });
    }
}

class MightyBlowInfo : ActionInfo
{
    int damagePerStack = -3;

    public MightyBlowInfo(RangeInfo castRange, RangeInfo effectRange)
        : base(new StateChange(StateChangeType.ActionCost, 0, 0), ActionMedium.Magical, ActionRange.AOE, ActionSource.Cast, castRange, effectRange, false)
    {
    }

    public override StateChange GetStateChange(EncounterCharacter caster, EncounterCharacter target)
    {
        int healthChange = -1 + caster.GetStackCountForStatus(StatusEffectType.BloodScent, caster) * damagePerStack;
        return new StateChange(StateChangeType.ActionTarget, healthChange, 0);
    }
}
