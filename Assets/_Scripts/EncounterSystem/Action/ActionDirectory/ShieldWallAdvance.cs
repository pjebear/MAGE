using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Common.AttributeEnums;
using Common.ActionTypes;

namespace EncounterSystem.Action
{
    using Character;
    using MapTypes;

    class ShieldWallAdvance : ActionBase
    {
        public override IEnumerator ExececuteAction(CharacterManager caster, List<MapTile> targets, System.Action<ActionInteractionResult> _RecordActionResults)
        {
            FinishedExecution = false;
            ActionResourceChangeInformation resourceChangeInfo = caster.GetModifiedActionStrength(ActionInfo.BaseResourceChangeInfo, ActionInfo.Modifiers);

            if (targets.Count > 0)
            {
                CharacterManager target = targets[0].GetCharacterOnTile();

                if (target != null && target.IsAlive)
                {
                    bool successfulBash = false;
                    float baseBashChance = 1f;
                    float mightDiff = caster.Stats[(int)PrimaryStat.Might] - target.Stats[(int)PrimaryStat.Might];
                    mightDiff /= caster.Stats[(int)PrimaryStat.Might];

                    baseBashChance += mightDiff;
                    caster.GetComponent<Animator>().SetTrigger(AnimationInfo.ActionAnimationId);
                    baseBashChance *= caster.Stats[(int)SecondaryStat.Fortitude];
                    successfulBash = true;// Random.Range(0, 1) < baseBashChance;
                                          //MapTile knockTile = null;
                                          //if (Random.Range(0, 1) < baseBashChance)
                                          //{
                                          //    MapManager map = MapManager.Instance;
                                          //    knockTile = map.GetTileAt((target.transform.localPosition - caster.transform.localPosition) + target.transform.localPosition);
                                          //    if (knockTile != null && knockTile.GetCharacterOnTile() == null && knockTile.transform.localPosition.y <= target.transform.localPosition.y)
                                          //    {
                                          //        successfulBash = true;
                                          //    }
                                          //}


                    //
                    //float bashActionSpeed = 0.6f;
                    //while (bashActionSpeed > 0)
                    //{
                    //    bashActionSpeed -= Time.deltaTime;
                    //    if (successfulBash)
                    //    {
                    //        Vector3 direction = (target.transform.position - caster.transform.position);
                    //        direction.y = 0f;
                    //        caster.transform.position += direction * Time.deltaTime / bashActionSpeed;
                    //    }
                    //    yield return new WaitForFixedUpdate();
                    //}


                    //if (mAnimationSystem != null)
                    //{
                    //    mAnimationSystem.SpawnAnimation(target.transform.position);
                    //    yield return new WaitForSeconds(0.5f);
                    //}

                    ActionInteractionResult result = new ActionInteractionResult(target);
                    target.AttemptActionResourceChange(resourceChangeInfo, ref result);
                    _RecordActionResults(result);


                    if (successfulBash)
                    {
                        target.GetComponent<MovementController>().KnockBack(target.transform.localPosition - caster.transform.localPosition);
                    }
                }
                yield return new WaitForEndOfFrame();
            }


            FinishedExecution = true;
        }
    }

}