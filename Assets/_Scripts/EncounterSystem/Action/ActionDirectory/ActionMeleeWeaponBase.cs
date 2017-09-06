
using System.Collections.Generic;
using System.Collections;
using System.Linq;



using Common.ActionEnums;
using Common.ActionTypes;
using Common.EquipmentTypes;
using UnityEngine;
namespace EncounterSystem.Action
{

    using MapTypes;
    using Character;
    class ActionMeleeWeaponBase : ActionBase
    {

        public override IEnumerator ExececuteAction(CharacterManager caster, List<MapTile> targets, System.Action<ActionInteractionResult> _RecordActionResults)
        {
            Debug.Assert(targets.Count <= 1, "Recieved more than one target for Melee action");
            FinishedExecution = false;

            List<WeaponBase> weapons = caster.GetHeldWeapons();
            bool dualStrike = (weapons[0] != null && weapons[1] != null) && (weapons[0].WeaponType != Common.EquipmentEnums.WeaponType.Unarmed && weapons[1].WeaponType != Common.EquipmentEnums.WeaponType.Unarmed);

            int counter = 1;
            foreach (var weapon in weapons)
            {
                if (weapon != null)
                {
                    ActionResourceChangeInformation resourceChangeInformation = caster.GetModifiedActionStrength(ActionInfo.BaseResourceChangeInfo, weapon.DamageModifiers);
                    caster.GetComponent<Animator>().SetTrigger(string.Format("Attack{0}Trigger", 3 * counter));

                    yield return new WaitForSeconds(0.5f);

                    if (targets.Count > 0 && targets[0] != null)
                    {
                        CharacterManager target = targets[0].GetCharacterOnTile();
                        if (target != null)
                        {
                            ActionInteractionResult actionResults = new ActionInteractionResult(target);
                            ActionOrientation targetOrientation = ActionUtil.GetActionOrienation(caster.transform.position, actionResults.Target.transform.position, actionResults.Target.transform.forward);
                            actionResults.Target.AttemptActionResourceChange(resourceChangeInformation, ref actionResults, targetOrientation);
                            _RecordActionResults(actionResults);
                        }

                    }
                    
                    yield return new WaitForSeconds(1);
                    if (!dualStrike)
                    {
                        break;
                    }
                }
                counter++;
            }

            FinishedExecution = true;
        }
    }

}
