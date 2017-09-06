using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using Common.AttributeEnums;
using Common.AttributeTypes;
using Common.StatusTypes;
using Common.StatusEnums;

//------------------------------------------------------------
namespace Common.StatusEnums
{
    public enum StatusEffectIndex
    {
        PROTECT_1,
        SHELL_1,

        FOOTMAN_FRONTAL_BLOCK,
        FOOTMAN_SPEED_INCREASE,
        FRONTAL_DEFEND,
        PERIPHERAL_DEFEND,
        POISON,
        MIGHT_BUFF,
        MIGHT_INCREASE,
        ROOT,
        DISARM,
        AURA_HEALTH_INCREASE,
        FOOTMAN_AURA_PHYSICAL_DAMAGE_RESISTANCE,
        ADEPT_AURA_MAGIC_DAMAGE_INCREASE,
        MONK_AURA_HEALTH_REGEN,
        ARCHER_AURA_SPEED_INCREASE,
        AURA_PHYSICAL_CRITICAL_PERCENT,
        INTERPUPT,
        CAST_SLOW,
        CAST_HASTE,
        BERSERKER_BLOODSCENT,
        PHYSICAL_DAMAGE_PERCENT,
        KO,
        //Equipment Status Effects handled in Equipment.cs
        EQUIPPED_SHIELD_FRONTAL_BLOCK,
        EQUIPPED_SHIELD_PERIFERAL_BLOCK,
        EQUIPPED_ARMOR_PHYSICAL_RESISTANCE,

        NUM
    }

    public enum AuraIndex
    {
        HEALTH_INCREASE,
        PHYSICAL_CRITICAL_PERCENT,
        FOOTMAN_PHYSICAL_DAMAGE_RESISTANCE,
        ADEPT_MAGIC_DAMAGE_INCREASE,
        MONK_HEALTH_REGEN,
        ARCHER_SPEED_INCREASE
    }
}

namespace EncounterSystem.StatusEffects
{

    using Character;


    class StatusEffectFactory
    {
        
        public static StatusEffectFactory Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new StatusEffectFactory();

                }
                return mInstance;
            }
        }

        private static StatusEffectFactory mInstance;

        public static StatusEffect CheckoutStatusEffect(StatusEffectIndex toCheckout, CharacterManager owner = null)
        {
            StatusEffect statusEffect = null;
            switch(toCheckout)
            {
                // Defend is multiplicative because it relies on block already being there so will be missed on persistant effect rebuild when additive
                // TODO: create a new ordering system besides addative/ multiplicative?
                case (StatusEffectIndex.FRONTAL_DEFEND): // double frontal block chance
                    statusEffect = new StatusEffect(toCheckout, "Defend", StatusEffectProgression.TurnBased, 1, true, false, false,
                        (stackCount) =>
                        {
                            return new AttributeModifier(new AttributeIndex(AttributeType.Stat, (int)TertiaryStat.FrontalBlock), ModifierType.Multiplicative,
                                (AttributeContainer container) => { return 2f; });
                        });
                    break;

                case (StatusEffectIndex.PERIPHERAL_DEFEND):// double peripheral block chance
                    statusEffect = new StatusEffect(toCheckout, "", StatusEffectProgression.TurnBased, 1, true, false, false,
                        (stackCount) =>
                        {
                            return new AttributeModifier(new AttributeIndex(AttributeType.Stat, (int)TertiaryStat.PeriferalBlock), ModifierType.Multiplicative,
                                (AttributeContainer container) => { return 2f; });
                        }, null, null, null, true);
                    break;

                // Needs prerequisite for talent
                case (StatusEffectIndex.FOOTMAN_FRONTAL_BLOCK): 
                    statusEffect = new StatusEffect(toCheckout, toCheckout.ToString(), StatusEffectProgression.INVALID, -1, true, true, false,
                        (stackCount) =>
                        {
                            return new AttributeModifier(new AttributeIndex(AttributeType.Stat, (int)TertiaryStat.FrontalBlock), ModifierType.Additive,
                                (AttributeContainer container) => { return 0.05f * (stackCount + 1); });
                        });
                    break;

                // Needs prerequisite for talent
                case (StatusEffectIndex.FOOTMAN_SPEED_INCREASE):
                    statusEffect = new StatusEffect(toCheckout, toCheckout.ToString(), StatusEffectProgression.INVALID, -1, true, true, false,
                        (stackCount) =>
                        {
                            return new AttributeModifier(new AttributeIndex(AttributeType.Stat, (int)TertiaryStat.Speed), ModifierType.Additive,
                                (AttributeContainer container) => { return 1 * (stackCount + 1); });
                        });
                    break;

                case (StatusEffectIndex.POISON):
                    statusEffect = new StatusEffect(toCheckout, toCheckout.ToString(), StatusEffectProgression.ClockBased, 32, false, false, true,
                        null,
                        (stackCount) =>{ // poison tick
                            return new AttributeModifier(new AttributeIndex(AttributeType.Resource, (int)Resource.Health), ModifierType.Additive,
                                (AttributeContainer container) => { return container[AttributeType.Resource][(int)Resource.MaxHealth] * -0.1f; });});
                    break;

                case (StatusEffectIndex.MIGHT_BUFF):
                    statusEffect = new StatusEffect(toCheckout, toCheckout.ToString(), StatusEffectProgression.ClockBased, 32, true, false, true,
                        (stackCount) =>
                        {
                            return new AttributeModifier(new AttributeIndex(AttributeType.Stat, (int)PrimaryStat.Might), ModifierType.Multiplicative,
                                (AttributeContainer container) => { return 1.5f; });});

                    break;

                case (StatusEffectIndex.MIGHT_INCREASE):
                    statusEffect = new StatusEffect(toCheckout, toCheckout.ToString(), StatusEffectProgression.ClockBased, 32, true, false, true,
                        (stackCount) =>{
                            return new AttributeModifier(new AttributeIndex(AttributeType.Stat, (int)PrimaryStat.Might), ModifierType.Additive,
                                (AttributeContainer container) => { return 10f; });});
                    break;

                case (StatusEffectIndex.ROOT):
                    statusEffect = new StatusEffect(toCheckout, toCheckout.ToString(), StatusEffectProgression.ClockBased, 32, false, false, true,
                     (stackCount) =>
                     {
                         return new AttributeModifier(new AttributeIndex(AttributeType.Status, (int)StatusType.Rooted), ModifierType.Additive,
                             (AttributeContainer container) => { return 1f; });
                     });
                    break;

                case (StatusEffectIndex.DISARM):
                    statusEffect = new StatusEffect(toCheckout, toCheckout.ToString(), StatusEffectProgression.ClockBased, 32, false, false, true,
                      (stackCount) =>
                      {
                          return new AttributeModifier(new AttributeIndex(AttributeType.Status, (int)StatusType.Disarmed), ModifierType.Additive,
                              (AttributeContainer container) => { return 1f; });
                      });
                    break;

                case (StatusEffectIndex.AURA_HEALTH_INCREASE):
                    statusEffect = new StatusEffect(toCheckout, toCheckout.ToString(), StatusEffectProgression.INVALID, -1, true, true, false,
                       (stackCount) =>
                       {
                           return new AttributeModifier(new AttributeIndex(AttributeType.Resource, (int)Resource.MaxHealth), ModifierType.Multiplicative,
                               (AttributeContainer container) => { return 1.15f + stackCount * 0.05f; });
                       });
                    
                    break;
                case (StatusEffectIndex.INTERPUPT):
                    statusEffect = new StatusEffect(toCheckout, toCheckout.ToString(), StatusEffectProgression.INVALID, -1, false, false, false,
                       (stackCount) =>
                       {
                           return new AttributeModifier(new AttributeIndex(AttributeType.Status, (int)StatusType.Interupt), ModifierType.Additive,
                               (AttributeContainer container) => { return 1; });
                       });

                    break;
                case (StatusEffectIndex.CAST_HASTE):
                    statusEffect = new StatusEffect(toCheckout, toCheckout.ToString(), StatusEffectProgression.ClockBased, 16, true, true, false,
                       (stackCount) =>
                       {
                           return new AttributeModifier(new AttributeIndex(AttributeType.Stat, (int)TertiaryStat.CastSpeed), ModifierType.Additive,
                               (AttributeContainer container) => { return .5f; });
                       });

                    break;
                case (StatusEffectIndex.CAST_SLOW):
                    statusEffect = new StatusEffect(toCheckout, toCheckout.ToString(), StatusEffectProgression.ClockBased, 16, false, false, false,
                       (stackCount) =>
                       {
                           return new AttributeModifier(new AttributeIndex(AttributeType.Stat, (int)TertiaryStat.CastSpeed), ModifierType.Additive,
                               (AttributeContainer container) => { return -.5f; });
                       });

                    break;
                case (StatusEffectIndex.BERSERKER_BLOODSCENT):
                    statusEffect = new StatusEffect(toCheckout, toCheckout.ToString(), StatusEffectProgression.INVALID, -1, true, true, false);
                    break;

                case (StatusEffectIndex.PHYSICAL_DAMAGE_PERCENT):
                    statusEffect = new StatusEffect(toCheckout, toCheckout.ToString(), StatusEffectProgression.ClockBased, 16, true, true, false,
                      (stackCount) =>
                      {
                          statusEffect.Beneficial = stackCount >= 0;
                          return new AttributeModifier(new AttributeIndex(AttributeType.Stat, (int)TertiaryStat.PhysicalMultiplier), ModifierType.Additive,
                              (AttributeContainer container) => { return stackCount / 100f; });
                      });

                    break;

                case (StatusEffectIndex.AURA_PHYSICAL_CRITICAL_PERCENT):
                    statusEffect = new StatusEffect(toCheckout, toCheckout.ToString(), StatusEffectProgression.INVALID, -1, true, true, false,
                      (stackCount) =>
                      {
                          return new AttributeModifier(new AttributeIndex(AttributeType.Stat, (int)TertiaryStat.PhysicalCritChance), ModifierType.Additive,
                              (AttributeContainer container) => { return .20f + stackCount *  .1f; });
                      });

                    break;

                case (StatusEffectIndex.FOOTMAN_AURA_PHYSICAL_DAMAGE_RESISTANCE):
                    statusEffect = new StatusEffect(toCheckout, toCheckout.ToString(), StatusEffectProgression.INVALID, -1, true, true, false,
                      (stackCount) =>
                      {
                          return new AttributeModifier(new AttributeIndex(AttributeType.Stat, (int)TertiaryStat.PhysicalResistance), ModifierType.Additive,
                              (AttributeContainer container) => { return .1f + stackCount * .05f; });
                      });
                    break;

                case (StatusEffectIndex.MONK_AURA_HEALTH_REGEN):
                    statusEffect = new StatusEffect(toCheckout, toCheckout.ToString(), StatusEffectProgression.INVALID, -1, true, true, false,
                        null,
                      (stackCount) => { // health tick
                          return new AttributeModifier(new AttributeIndex(AttributeType.Resource, (int)Resource.Health), ModifierType.Additive,
                              (AttributeContainer container) => { return 5 + stackCount * 2f; });
                      });
                    break;

                case (StatusEffectIndex.ADEPT_AURA_MAGIC_DAMAGE_INCREASE):
                    statusEffect = new StatusEffect(toCheckout, toCheckout.ToString(), StatusEffectProgression.INVALID, -1, true, true, false,
                      (stackCount) =>
                      {
                          return new AttributeModifier(new AttributeIndex(AttributeType.Stat, (int)TertiaryStat.MagicalMultiplier), ModifierType.Additive,
                              (AttributeContainer container) => { return .1f + stackCount * .05f; });
                      });
                    break;

                case (StatusEffectIndex.ARCHER_AURA_SPEED_INCREASE):
                    statusEffect = new StatusEffect(toCheckout, toCheckout.ToString(), StatusEffectProgression.INVALID, -1, true, true, false,
                      (stackCount) =>
                      {
                          return new AttributeModifier(new AttributeIndex(AttributeType.Stat, (int)TertiaryStat.Speed), ModifierType.Additive,
                              (AttributeContainer container) => { return 1 + stackCount; });
                      });
                    break;

                case (StatusEffectIndex.PROTECT_1):
                    statusEffect = new StatusEffect(toCheckout, toCheckout.ToString(), StatusEffectProgression.ClockBased, 32, true, false, true,
                      (stackCount) =>
                      {
                          return new AttributeModifier(new AttributeIndex(AttributeType.Stat, (int)TertiaryStat.PhysicalResistance), ModifierType.Additive,
                              (AttributeContainer container) => { return .2f; });
                      });
                    break;

                case (StatusEffectIndex.SHELL_1):
                    statusEffect = new StatusEffect(toCheckout, toCheckout.ToString(), StatusEffectProgression.ClockBased, 32, true, false, true,
                      (stackCount) =>
                      {
                          return new AttributeModifier(new AttributeIndex(AttributeType.Stat, (int)TertiaryStat.MagicalResistance), ModifierType.Additive,
                              (AttributeContainer container) => { return .2f; });
                      });
                    break;

                case (StatusEffectIndex.KO):
                    statusEffect = new StatusEffect(toCheckout, "Turns Till Death", StatusEffectProgression.ClockBased, -1, false, false, false);
                    break;

                default:
                    Debug.LogError("Undefined Status!");
                    break;

            }
            return statusEffect;
        }

        public static Aura CheckoutStatusAura(AuraIndex toCheckout, CharacterManager owner)
        {
            Aura auraObject = new GameObject(toCheckout.ToString()).AddComponent<Aura>();
            auraObject.gameObject.AddComponent<SphereCollider>().isTrigger = true;
            auraObject.transform.parent = owner.gameObject.transform;
            auraObject.transform.localPosition = Vector3.zero;
            StatusEffect effect = null;
            int range = 2;
            bool beneficial = true;
            switch (toCheckout)
            {
                case (AuraIndex.HEALTH_INCREASE):
                    effect = CheckoutStatusEffect(StatusEffectIndex.AURA_HEALTH_INCREASE);
                    break;

                case (AuraIndex.PHYSICAL_CRITICAL_PERCENT):
                    effect = CheckoutStatusEffect(StatusEffectIndex.AURA_PHYSICAL_CRITICAL_PERCENT);
                    break;

                case (AuraIndex.FOOTMAN_PHYSICAL_DAMAGE_RESISTANCE):
                    effect = CheckoutStatusEffect(StatusEffectIndex.FOOTMAN_AURA_PHYSICAL_DAMAGE_RESISTANCE);
                    break;

                case (AuraIndex.ADEPT_MAGIC_DAMAGE_INCREASE):
                    effect = CheckoutStatusEffect(StatusEffectIndex.ADEPT_AURA_MAGIC_DAMAGE_INCREASE);
                    break;

                case (AuraIndex.MONK_HEALTH_REGEN):
                    effect = CheckoutStatusEffect(StatusEffectIndex.MONK_AURA_HEALTH_REGEN);
                    break;

                case (AuraIndex.ARCHER_SPEED_INCREASE):
                    effect = CheckoutStatusEffect(StatusEffectIndex.ARCHER_AURA_SPEED_INCREASE);
                    break;

                default:
                    Debug.LogError("Error checking out aura " + toCheckout.ToString());
                    break;
            }

            auraObject.Initialize( owner, effect, range, beneficial);
            return auraObject;
        }
        


    }
}
