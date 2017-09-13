using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EncounterSystem
{
    using Common.AttributeEnums;
    using Common.AttributeTypes;
    using Common.ActionEnums;
    using UnityEngine;
    using Common.ActionTypes;

    namespace Character
    {
        namespace Managers
        {
            class DefenseManager
            {
                private AttributeContainer rAttributes;
                private readonly AttributeIndex FrontalBlockIndex = new AttributeIndex(AttributeType.Stat, (int)TertiaryStat.FrontalBlock);
                private readonly AttributeIndex PeriferalBlockIndex = new AttributeIndex(AttributeType.Stat, (int)TertiaryStat.PeriferalBlock);
                private readonly AttributeIndex FrontalParryIndex = new AttributeIndex(AttributeType.Stat, (int)TertiaryStat.FrontalParry);
                private readonly AttributeIndex PeriferalParryIndex = new AttributeIndex(AttributeType.Stat, (int)TertiaryStat.PeriferalParry);
                private readonly AttributeIndex DodgeIndex = new AttributeIndex(AttributeType.Stat, (int)TertiaryStat.Dodge);

                private const float BLOCKED_BLUNT_DAMAGE_MODIFIER = 1f;
                private const float PIERCE_ARMOR_PENETRATION_MODIFIER = 0.5f;

                public DefenseManager()
                {
                   // Empty
                }

                public void Initialize(AttributeContainer attributes)
                {
                    rAttributes = attributes;
                }

                public void DefendAgainstAction(ref ActionResourceChangeInformation actionInformation, ActionOrientation orientationToAction, ref AvoidanceResult avoidanceResult, ref InteractionResult interactionResult)
                {
                    //TODO: Base miss chance?

                    // calculate crit chance

                    float critChance = actionInformation.CriticalChance;
                    critChance += rAttributes[AttributeType.Stat]
                        [actionInformation.ActionBaseType == ActionBaseType.Physical ? (int)TertiaryStat.PhysicalCritSusceptibility : (int)TertiaryStat.MagicalCritSusceptibility];
                    bool isCrit = Random.Range(0f, 1f) < critChance;

                    if (isCrit)
                    {
                        interactionResult = InteractionResult.Crit;
                        actionInformation.ResourceChange.Value *= actionInformation.CriticalMultiplier;
                    }
                    else
                    {
                        interactionResult = InteractionResult.Hit;
                    }

                    avoidanceResult = AttemptActionAvoidance(actionInformation.ActionBaseType, actionInformation.ActionEffectType, orientationToAction);

                    switch (actionInformation.ActionBaseType)
                    {
                        case (ActionBaseType.Physical):
                            actionInformation.ResourceChange.Value = ApplyPhysicalActionReductions(avoidanceResult, actionInformation.ResourceChange.Value, actionInformation.PhysicalComposition);
                            break;
                        case (ActionBaseType.Magic):
                            actionInformation.ResourceChange.Value = ApplyMagicalReductions(actionInformation.ResourceChange.Value, avoidanceResult);
                            break;
                    }
                }

                private AvoidanceResult AttemptActionAvoidance(ActionBaseType baseType, ActionEffectType effectType, ActionOrientation orientationToAction)
                {
                    float diceRoll = 0;
                    
                    if (baseType == ActionBaseType.Magic)
                    {
                        //TODO figure something out
                        return AvoidanceResult.Invalid;
                    }
                    else if (baseType == ActionBaseType.Physical) // check meele and physical actions, but dont check parry if a projectile
                    {
                        // dodge
                        diceRoll = Random.Range(0f, 1f);
                        if (diceRoll < rAttributes[DodgeIndex]) return AvoidanceResult.Dodge;
                        // if being attacked from behind and didn't successfully dodge, it is a hit
                        if (orientationToAction == ActionOrientation.Rear) return AvoidanceResult.Invalid;

                        // block
                        diceRoll = Random.Range(0f, 1f);
                        if (diceRoll < rAttributes[orientationToAction == ActionOrientation.Peripheral ? PeriferalBlockIndex : FrontalBlockIndex])
                            return AvoidanceResult.Block;

                        // Parry only if not a projectile
                        if (effectType != ActionEffectType.Projectile)
                        {
                            // parry
                            diceRoll = Random.Range(0f, 1f);
                            if (diceRoll < rAttributes[orientationToAction == ActionOrientation.Peripheral ? PeriferalParryIndex : FrontalParryIndex])
                                return AvoidanceResult.Parry;
                        }
                       
                    }
                    return AvoidanceResult.Invalid;
                }

                private float ApplyPhysicalActionReductions(AvoidanceResult result, float initialAmount, Dictionary<PhysicalActionType, float> damageComposition)
                {
                    float newAmount = 0;

                    // No reduction if the effect doesn't hit
                    if (result == AvoidanceResult.Dodge || result == AvoidanceResult.Parry)
                    {
                        return newAmount;
                    }
                    // Allow blunt damage to pass through block
                    else if (result == AvoidanceResult.Block)
                    {
                        // only take blunt component of attack
                        if (damageComposition.ContainsKey(PhysicalActionType.Blunt))
                        {
                            newAmount = initialAmount * damageComposition[PhysicalActionType.Blunt] * BLOCKED_BLUNT_DAMAGE_MODIFIER;
                            newAmount *= 1 - rAttributes[AttributeType.Stat][(int)TertiaryStat.PhysicalResistance];
                        }
                        else // no damage made it past block
                        {
                            newAmount = 0f;
                        }
                    }
                    // there was no avoidance, check if there is armor pen
                    else if (damageComposition.ContainsKey(PhysicalActionType.Pierce))
                    {
                        float piercingDamage = initialAmount * damageComposition[PhysicalActionType.Pierce];
                        float otherDamage = initialAmount - piercingDamage;

                        piercingDamage *= 1 - (rAttributes[AttributeType.Stat][(int)TertiaryStat.PhysicalResistance] * PIERCE_ARMOR_PENETRATION_MODIFIER);
                        otherDamage *= 1 - rAttributes[AttributeType.Stat][(int)TertiaryStat.PhysicalResistance];
                        newAmount = piercingDamage + otherDamage;
                    } 
                    else // just do raw physical reduction calculations
                    {
                        newAmount = initialAmount *= 1 - rAttributes[AttributeType.Stat][(int)TertiaryStat.PhysicalResistance];
                    }
                    return newAmount;
                }

                private float ApplyMagicalReductions(float initialAmount, AvoidanceResult avoidance)
                {

                    switch (avoidance)
                    {
                        case (AvoidanceResult.Invalid):
                            break;
                        case (AvoidanceResult.Resist):
                            initialAmount = 0f;
                            break;
                        case (AvoidanceResult.PartialResist):
                            initialAmount *= 0.5f;
                            break;
                        default:
                            Debug.LogError("Invalid Magical avoidance. Got " + avoidance.ToString());
                            break;
                            
                    }
                    //TODO: figure out what the mechanic for allignments is
                    return initialAmount *=  1 - rAttributes[AttributeType.Stat][(int)TertiaryStat.MagicalResistance];
                }

                
            }
        }
    }
}