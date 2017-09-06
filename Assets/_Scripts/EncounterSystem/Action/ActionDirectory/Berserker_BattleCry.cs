using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.ActionTypes;
using Common.StatusEnums;
using Common.StatusTypes;

namespace EncounterSystem.Action
{
    using Character;
    using MapTypes;
    using StatusEffects;

    class Berserker_BattleCry : ActionBase
    {
        int BloodStackMultiplier = 10;
        StatusEffectIndex BattleCryIndex = StatusEffectIndex.PHYSICAL_DAMAGE_PERCENT;

        public override IEnumerator ExececuteAction(CharacterManager caster, List<MapTile> targets, System.Action<ActionInteractionResult> _RecordActionResults)
        {
            FinishedExecution = false;
            caster.GetComponent<Animator>().SetTrigger(AnimationInfo.ActionAnimationId);
            int numBloodScentStacks = 0;
            StatusEffect bloodScent = caster.QueryStatusEffect(StatusEffectIndex.BERSERKER_BLOODSCENT);
            if (bloodScent != null)
            {
                numBloodScentStacks = bloodScent.StackCount + 1; // 0 indexed
            }
            mAnimationSystem.SpawnAnimation(caster.transform.position);
            yield return new WaitForSeconds(0.6f);

            foreach (var target in targets)
            {
                CharacterManager unit = target.GetCharacterOnTile();
                if (unit != null && unit.IsAlive)
                {

                    bool success = unit.AttemptStatusApplication(StatusEffectFactory.CheckoutStatusEffect(BattleCryIndex), numBloodScentStacks * BloodStackMultiplier);
                    if (success)
                    {
                        ActionInteractionResult actionResult = new ActionInteractionResult(unit);
                        actionResult.AppliedStatusEffects.Add(BattleCryIndex);
                        _RecordActionResults(actionResult);
                    }
                }
            }

            // remove all stacks from caster
            caster.AttemptRemoveStatusEffect(StatusEffectIndex.BERSERKER_BLOODSCENT, -1);

            FinishedExecution = true;
        }

    }
}
