using System.Collections;
using System.Collections.Generic;


using EncounterSystem.Character;
using Common.AttributeEnums;
using UnityEngine;

namespace EncounterSystem.Action
{
    using Common.ActionTypes;
    using MapTypes;
    using StatusEffects;
   
        class SpellBase : ActionBase
        {
            
            public override IEnumerator ExececuteAction(CharacterManager caster, List<MapTile> targets, System.Action<ActionInteractionResult> _RecordActionResults)
            {
                FinishedExecution = false;
                caster.GetComponent<Animator>().SetTrigger(AnimationInfo.ActionAnimationId);
                
                yield return new WaitForSeconds(0.6f);
                ActionResourceChangeInformation resourceChangeInfo = caster.GetModifiedActionStrength(ActionInfo.BaseResourceChangeInfo, ActionInfo.Modifiers);

                foreach (var target in targets)
                {
                    CharacterManager character = target.GetCharacterOnTile();
                    if (character != null && character.IsAlive)
                    {
                        ActionInteractionResult result = new ActionInteractionResult(character);
                        if (mAnimationSystem != null)
                        {
                            mAnimationSystem.SpawnAnimation(character.transform.position);
                            yield return new WaitForSeconds(0.5f);
                        }
                        if (resourceChangeInfo.ResourceChange.Value != 0)
                        {

                            character.AttemptActionResourceChange(resourceChangeInfo, ref result);
                        }

                        foreach (var statusEffect in ActionInfo.StatusEffects)
                        {
                            if (character.AttemptStatusApplication(StatusEffectFactory.CheckoutStatusEffect(statusEffect)))
                            {
                                result.AppliedStatusEffects.Add(statusEffect);
                            }
                        }
                        _RecordActionResults(result);
                    }
                }

                FinishedExecution = true;
            }
        }
    }

