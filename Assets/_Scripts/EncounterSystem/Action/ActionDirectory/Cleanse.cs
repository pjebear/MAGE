using System.Collections;
using System.Collections.Generic;

using Common.ActionTypes;
using UnityEngine;

namespace EncounterSystem.Action
{
    using Character;
    using MapTypes;

    class Cleanse : ActionBase
    {
        public override IEnumerator ExececuteAction(CharacterManager caster, List<MapTile> targets, System.Action<ActionInteractionResult> _RecordActionResults)
        {
            FinishedExecution = false;
            caster.GetComponent<Animator>().SetTrigger(AnimationInfo.ActionAnimationId);

            yield return new WaitForSeconds(0.6f);

            foreach (var target in targets)
            {
                CharacterManager character = target.GetCharacterOnTile();
                if (character != null)
                {
                    if (mAnimationSystem != null)
                    {
                        mAnimationSystem.SpawnAnimation(target.transform.position);
                        yield return new WaitForSeconds(0.5f);
                    }
                    if (character.DispelStatusEffects(false, true))
                    {
                        ActionInteractionResult result = new ActionInteractionResult(character);
                        result.InteractionResult = Common.ActionEnums.InteractionResult.Hit;
                        _RecordActionResults(result);
                    }
                }
            }

            FinishedExecution = true;
        }
    }
}
