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

    class AOESpellBase : ActionBase
    {

        public override IEnumerator ExececuteAction(CharacterManager caster, List<MapTile> targets, System.Action<ActionInteractionResult> _RecordActionResults)
        {
            FinishedExecution = false;
            caster.GetComponent<Animator>().SetTrigger(AnimationInfo.ActionAnimationId);

            yield return new WaitForSeconds(0.6f);
            ActionResourceChangeInformation resourceChangeInfo = caster.GetModifiedActionStrength(ActionInfo.BaseResourceChangeInfo, ActionInfo.Modifiers);

            targets.Sort((x, y) => (x.transform.localPosition - caster.GetCurrentTile().transform.localPosition).sqrMagnitude.
                CompareTo((y.transform.localPosition - caster.GetCurrentTile().transform.localPosition).sqrMagnitude));

            Dictionary<int, List<CharacterManager>> expandingRingTargets = new Dictionary<int, List<CharacterManager>>();
            foreach (MapTile tile in targets)
            {
                if (tile.GetCharacterOnTile() != null)
                {
                    int distance = Mathf.FloorToInt((tile.transform.localPosition - caster.GetCurrentTile().transform.localPosition).sqrMagnitude);
                    if (!expandingRingTargets.ContainsKey(distance))
                    {
                        expandingRingTargets.Add(distance, new List<CharacterManager>());                    }
                    expandingRingTargets[distance].Add(tile.GetCharacterOnTile());
                }
            }
            int previousDistance = 0;
            foreach (var targetList in expandingRingTargets)
            {
                foreach(CharacterManager aoeTarget in targetList.Value)
                {
                    if  (aoeTarget.IsAlive)
                    {
                        ActionInteractionResult result = new ActionInteractionResult(aoeTarget);
                        if (mAnimationSystem != null)
                        {
                            mAnimationSystem.SpawnAnimation(aoeTarget.transform.position);
                            yield return new WaitForSeconds(0.5f);
                        }
                        if (resourceChangeInfo.ResourceChange.Value != 0)
                        {

                            aoeTarget.AttemptActionResourceChange(resourceChangeInfo, ref result);
                        }

                        foreach (var statusEffect in ActionInfo.StatusEffects)
                        {
                            if (aoeTarget.AttemptStatusApplication(StatusEffectFactory.CheckoutStatusEffect(statusEffect)))
                            {
                                result.AppliedStatusEffects.Add(statusEffect);
                            }
                        }
                        _RecordActionResults(result);
                    }
                }
                yield return new WaitForSeconds(0.1f * (targetList.Key - previousDistance) );
                previousDistance = targetList.Key;
            }

            FinishedExecution = true;
        }
    }
}

