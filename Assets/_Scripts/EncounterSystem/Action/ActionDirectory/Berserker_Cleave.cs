using System.Collections;
using System.Collections.Generic;

using Common.ActionTypes;
using Common.EquipmentTypes;
using Common.StatusEnums;
using Common.StatusTypes;

using Common.AttributeEnums;
using UnityEngine;

namespace EncounterSystem.Action
{

    using MapTypes;
    using Map;
    using Character;
    class Berserker_Cleave : ActionBase
    {
        float BloodStackMultiplier = .07f;
        float CleaveBaseScale = .3f;
        float MainTargetScale = 0.75f;


        public override IEnumerator ExececuteAction(CharacterManager berserker, List<MapTile> targets, System.Action<ActionInteractionResult> _RecordActionResults)
        {
            FinishedExecution = false;
            berserker.GetComponent<Animator>().SetTrigger(AnimationInfo.ActionAnimationId);
            int numBloodScentStacks = 0;
            StatusEffect bloodScent = berserker.QueryStatusEffect(StatusEffectIndex.BERSERKER_BLOODSCENT);
            if (bloodScent != null)
            {
                numBloodScentStacks = bloodScent.StackCount + 1; // 0 indexed
            }
            yield return new WaitForSeconds(0.4f);

            MapTile frontalTile = MapManager.Instance.GetTileAt(berserker.GetCurrentTile().transform.localPosition + berserker.transform.forward);
            Debug.Assert(frontalTile != null);

            WeaponBase weapon = berserker.GetHeldWeapons()[0];
            Debug.Assert(weapon != null); // 2 handed weapon

            ActionResourceChangeInformation resourceChangeInfo = berserker.GetModifiedActionStrength(ActionInfo.BaseResourceChangeInfo, weapon.DamageModifiers);

            float mainDamage = resourceChangeInfo.ResourceChange.Value * MainTargetScale;
            float cleaveDamage = mainDamage * (CleaveBaseScale + numBloodScentStacks * BloodStackMultiplier); // 0.3f + [0-10] * 0.07f = [0.3-1.0]

            foreach (MapTile tile in targets)
            {
                CharacterManager target = tile.GetCharacterOnTile();
                if (target != null)
                {
                    ActionInteractionResult cleaveResult = new ActionInteractionResult(target);
                    if (tile == frontalTile) // main target
                    {
                        resourceChangeInfo.ResourceChange.Value = mainDamage;
                    }
                    else
                    {
                        resourceChangeInfo.ResourceChange.Value = cleaveDamage;
                    }
                    target.AttemptActionResourceChange(resourceChangeInfo, ref cleaveResult);
                    _RecordActionResults(cleaveResult);
                }

            }

            FinishedExecution = true;
        }

    }


}
