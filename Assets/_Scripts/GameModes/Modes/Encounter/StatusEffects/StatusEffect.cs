using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


enum StatusEffectType
{
    INVALID = -1,

    Avenger,
    Poison,
    Protection,
    Aura_Protection,
    Regen,
    Aura_Regen,
    Shackle,
    BloodScent,
    Aura_RighteousGlory,

    NUM
}

abstract class StatusEffect
{
    public StatusEffectType EffectType { get { return mStatusEffectInfo.Type; } }
    public EncounterCharacter CreatedBy;
    public int StackCount;
    public StatusIconSpriteId SpriteId { get { return mStatusEffectInfo.SpriteId; } }
    public bool Beneficial { get { return mStatusEffectInfo.Beneficial; } }
    protected int mCurrentDuration;
    protected StatusEffectInfo mStatusEffectInfo;

    protected StatusEffect(EncounterCharacter createdBy, StatusEffectInfo info)
    {
        CreatedBy = createdBy;
        StackCount = 1;
        mCurrentDuration = 0;
        mStatusEffectInfo = info;
    }

    public void StackEffect(int stacks)
    {
        StackCount = Math.Min(StackCount + stacks, mStatusEffectInfo.MaxStackCount);
        mCurrentDuration = 0;
    }

    public bool UnStackEffect(int stacks)
    {
        bool effectExpired = false;

        StackCount-= stacks;

        if (StackCount == 0)
        {
            effectExpired = true;
        }

        return effectExpired;
    }

    public bool ProgressDuration()
    {
        mCurrentDuration++;

        return HasExpired();
    }

    public bool HasExpired()
    {
        if (mStatusEffectInfo.Duration == StatusEffectConstants.PERMANENT_DURATION)
        {
            return false;
        }
        else
        {
            return mCurrentDuration >= mStatusEffectInfo.Duration;
        }  
    }

    public abstract List<AttributeModifier> GetAttributeModifiers();
    public abstract StateChange GetTurnStartStateChange();
    
}

class ProtectionEffect : StatusEffect
{
    float stackValue = 0.25f;
    public ProtectionEffect(EncounterCharacter createdBy, StatusEffectInfo info) 
        : base(createdBy, info)
    {
        
    }

    public override List<AttributeModifier> GetAttributeModifiers()
    {
        List<AttributeModifier> modifiers = new List<AttributeModifier>();

        float protectionValue = stackValue * (StackCount);
        modifiers.Add(new AttributeModifier(new AttributeIndex(AttributeCategory.Stat, (int)TertiaryStat.PhysicalResistance), ModifierType.Increment, protectionValue));

        return modifiers;
    }

    public override StateChange GetTurnStartStateChange()
    {
        return StateChange.Empty;
    }
}

class ShackleEffect : StatusEffect
{
    public ShackleEffect(EncounterCharacter createdBy, StatusEffectInfo info)
        : base(createdBy, info)
    {

    }

    public override List<AttributeModifier> GetAttributeModifiers()
    {
        List<AttributeModifier> modifiers = new List<AttributeModifier>();

        modifiers.Add(new AttributeModifier(new AttributeIndex(StatusType.Disarmed), ModifierType.Increment, 1));

        return modifiers;
    }

    public override StateChange GetTurnStartStateChange()
    {
        return StateChange.Empty;
    }
}

class AvengerEffect : StatusEffect
{
    public float StackCountToAttributeMultiplier = 0.01f; // 1 stack = 1 percent
    public AvengerEffect(EncounterCharacter createdBy, StatusEffectInfo info)
        : base(createdBy, info)
    {

    }

    public override List<AttributeModifier> GetAttributeModifiers()
    {
        List<AttributeModifier> modifiers = new List<AttributeModifier>();

        modifiers.Add(new AttributeModifier(PrimaryStat.Might, ModifierType.Multiply, StackCount * StackCountToAttributeMultiplier));
        modifiers.Add(new AttributeModifier(PrimaryStat.Magic, ModifierType.Multiply, StackCount * StackCountToAttributeMultiplier));

        return modifiers;
    }

    public override StateChange GetTurnStartStateChange()
    {
        return StateChange.Empty;
    }
}

class RighteousGloryEffect : StatusEffect
{
    float StackCountToAttributeMultiplier = 0.2f;
    public RighteousGloryEffect(EncounterCharacter createdBy, StatusEffectInfo info)
        : base(createdBy, info)
    {

    }

    public override List<AttributeModifier> GetAttributeModifiers()
    {
        List<AttributeModifier> modifiers = new List<AttributeModifier>();

        modifiers.Add(new AttributeModifier(ResourceType.Health, ModifierType.Multiply, StackCount * StackCountToAttributeMultiplier));
        modifiers.Add(new AttributeModifier(PrimaryStat.Might, ModifierType.Multiply, StackCount * StackCountToAttributeMultiplier));
        modifiers.Add(new AttributeModifier(TertiaryStat.PhysicalResistance, ModifierType.Increment, StackCount * StackCountToAttributeMultiplier));

        return modifiers;
    }

    public override StateChange GetTurnStartStateChange()
    {
        return StateChange.Empty;
    }
}

class PoisonEffect : StatusEffect
{
    float stackValue = -2;
    public PoisonEffect(EncounterCharacter createdBy, StatusEffectInfo info)
        : base(createdBy, info)
    {

    }

    public override List<AttributeModifier> GetAttributeModifiers()
    {
        return new List<AttributeModifier>();
    }

    public override StateChange GetTurnStartStateChange()
    {
        int damage = (int)(stackValue * (StackCount));

        return new StateChange(StateChangeType.StatusEffect, damage, 0);
    }
}

class RegenEffect : StatusEffect
{
    float stackValue = 3;
    public RegenEffect(EncounterCharacter createdBy, StatusEffectInfo info)
        : base(createdBy, info)
    {

    }

    public override List<AttributeModifier> GetAttributeModifiers()
    {
        return new List<AttributeModifier>();
    }

    public override StateChange GetTurnStartStateChange()
    {
        int healthChange = (int)(stackValue * (StackCount));

        return new StateChange(StateChangeType.StatusEffect, healthChange, 0);
    }
}



class BloodScentEffect : StatusEffect
{
    public BloodScentEffect(EncounterCharacter createdBy, StatusEffectInfo info)
        : base(createdBy, info)
    {

    }

    public override List<AttributeModifier> GetAttributeModifiers()
    {
        return new List<AttributeModifier>();
    }

    public override StateChange GetTurnStartStateChange()
    {
        return StateChange.Empty;
    }
}

