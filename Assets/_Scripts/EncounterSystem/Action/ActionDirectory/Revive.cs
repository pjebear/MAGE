using System.Collections;
using System.Collections.Generic;



using Common.AttributeEnums;
using UnityEngine;
using Common.ActionTypes;
namespace EncounterSystem.Action
{
    using MapTypes;
    using Character;
    class Revive : ActionBase
    {
        float MaxHealthPercentage = .10f;
        public override IEnumerator ExececuteAction(CharacterManager caster, List<MapTile> targets, System.Action<ActionInteractionResult> _RecordActionResults)
        {
            FinishedExecution = false;
            caster.GetComponent<Animator>().SetTrigger(AnimationInfo.ActionAnimationId);

            yield return new WaitForSeconds(0.6f);

            foreach (var target in targets)
            {
                CharacterManager toRevive = target.GetCharacterOnTile();
                if (toRevive != null)
                {
                    if (mAnimationSystem != null)
                    {
                        mAnimationSystem.SpawnAnimation(target.transform.position);
                        yield return new WaitForSeconds(0.5f);
                    }
                    if (toRevive.Revive(toRevive.Resources[(int)Resource.MaxHealth] * MaxHealthPercentage))
                    {
                        ActionInteractionResult result = new ActionInteractionResult(toRevive);
                        result.InteractionResult = Common.ActionEnums.InteractionResult.Hit;
                        _RecordActionResults(result);
                    }
                }

            }

            FinishedExecution = true;
        }
    }


}
