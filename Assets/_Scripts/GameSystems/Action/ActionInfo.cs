using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        float baseEffectiveness = EquipmentUtil.GetHeldEquippableEffectiveness(caster, caster.Equipment[Equipment.Slot.RightHand] as HeldEquippable);
        int damage = -(int)(baseEffectiveness * Effectiveness);
        return new StateChange(StateChangeType.ActionTarget, damage, 0);
    }
}

class HealInfo : ActionInfo
{
    public override StateChange GetTargetStateChange(EncounterCharacter caster, EncounterCharacter target)
    {
        AttributeIndex index = new AttributeIndex(AttributeCategory.Stat, (int)PrimaryStat.Magic);
        int heal = (int)(caster.Attributes[index] * Effectiveness);
        return new StateChange(StateChangeType.ActionTarget, heal, 0);
    }
}

class ProtectionInfo : ActionInfo
{ 
    public override StateChange GetTargetStateChange(EncounterCharacter caster, EncounterCharacter target)
    {
        ProtectionEffect effect = StatusEffectFactory.CheckoutStatusEffect(StatusEffectType.Protection, caster) as ProtectionEffect;

        return new StateChange(StateChangeType.ActionTarget, 0,0,new List<StatusEffect>() { effect });
    }
}

class FireballInfo : ActionInfo
{
    public override StateChange GetTargetStateChange(EncounterCharacter caster, EncounterCharacter target)
    {
        AttributeIndex index = new AttributeIndex(AttributeCategory.Stat, (int)PrimaryStat.Magic);
        int damage = -(int)(caster.Attributes[index] * Effectiveness);
        return new StateChange(StateChangeType.ActionTarget, damage, 0);
    }
}

class MightyBlowInfo : WeaponActionInfoBase
{
    float DamagePercentPerStack = .2f;
    
    public override StateChange GetTargetStateChange(EncounterCharacter caster, EncounterCharacter target)
    {
        float baseEffectiveness = EquipmentUtil.GetHeldEquippableEffectiveness(caster, caster.Equipment[Equipment.Slot.RightHand] as HeldEquippable);
        baseEffectiveness *= 1 + (DamagePercentPerStack * caster.GetStackCountForStatus(StatusEffectType.BloodScent, caster));

        return new StateChange(StateChangeType.ActionTarget, -(int)baseEffectiveness, 0);
    }
}
