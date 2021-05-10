using MAGE.GameModes.Encounter;
using MAGE.GameSystems.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Stats
{
    abstract class StatusEffect
    {
        public StatusEffectId EffectType { get { return mStatusEffectInfo.Type; } }
        public int StackCount;
        public UI.StatusIconSpriteId SpriteId { get { return mStatusEffectInfo.SpriteId; } }
        public bool Beneficial { get { return mStatusEffectInfo.Beneficial; } }
        public int MaxDuration { get { return mStatusEffectInfo.Duration; } }
        protected int mCurrentDuration;
        protected StatusEffectInfo mStatusEffectInfo;

        protected StatusEffect()
        {
            StackCount = 1;
            mCurrentDuration = 0;
        }

        public void SetInfo(StatusEffectInfo info)
        {
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

            StackCount -= stacks;

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
            switch (mStatusEffectInfo.Duration)
            {
                case StatusEffectConstants.PERMANENT_DURATION:
                case StatusEffectConstants.UNTIL_NEXT_TURN:
                {
                    return false;
                }

                default:
                {
                    return mCurrentDuration >= mStatusEffectInfo.Duration;
                }
            }
        }

        public abstract List<AttributeModifier> GetAttributeModifiers();
        public abstract StateChange GetTurnStartStateChange();

    }

    class ProtectionEffect : StatusEffect
    {
        float stackValue = 0.05f;

        public override List<AttributeModifier> GetAttributeModifiers()
        {
            List<AttributeModifier> modifiers = new List<AttributeModifier>();

            float protectionValue = stackValue * (StackCount);
            modifiers.Add(new AttributeModifier(new AttributeIndex(AttributeCategory.TertiaryStat, (int)TertiaryStat.PhysicalResistance), ModifierType.Increment, protectionValue));

            return modifiers;
        }

        public override StateChange GetTurnStartStateChange()
        {
            return StateChange.Empty;
        }
    }

    class ShackleEffect : StatusEffect
    {
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

    class DefendEffect : StatusEffect
    {
        public float AvoidanceMultiple = 1f; 

        public override List<AttributeModifier> GetAttributeModifiers()
        {
            List<AttributeModifier> modifiers = new List<AttributeModifier>();

            modifiers.Add(new AttributeModifier(TertiaryStat.AvoidanceMultiplier, ModifierType.Increment, AvoidanceMultiple));

            return modifiers;
        }

        public override StateChange GetTurnStartStateChange()
        {
            return StateChange.Empty;
        }
    }

    class DazeEffect : StatusEffect
    {
        public override List<AttributeModifier> GetAttributeModifiers()
        {
            List<AttributeModifier> modifiers = new List<AttributeModifier>();

            modifiers.Add(new AttributeModifier(TertiaryStat.ResourceRecovery, ModifierType.Increment, -.5f));

            return modifiers;
        }

        public override StateChange GetTurnStartStateChange()
        {
            return StateChange.Empty;
        }
    }

    class DoubleTimeEffect : StatusEffect
    {
        public override List<AttributeModifier> GetAttributeModifiers()
        {
            List<AttributeModifier> modifiers = new List<AttributeModifier>();

            modifiers.Add(new AttributeModifier(TertiaryStat.Movement, ModifierType.Multiply, 1));

            return modifiers;
        }

        public override StateChange GetTurnStartStateChange()
        {
            return StateChange.Empty;
        }
    }

    class HamstringEffect : StatusEffect
    {
        public override List<AttributeModifier> GetAttributeModifiers()
        {
            List<AttributeModifier> modifiers = new List<AttributeModifier>();

            modifiers.Add(new AttributeModifier(TertiaryStat.Movement, ModifierType.Multiply, -.5f));

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
        public override List<AttributeModifier> GetAttributeModifiers()
        {
            return new List<AttributeModifier>();
        }

        public override StateChange GetTurnStartStateChange()
        {
            return StateChange.Empty;
        }
    }
}
