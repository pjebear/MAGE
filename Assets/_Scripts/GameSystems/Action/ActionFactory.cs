using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class ActionFactory
{
    public static ActionInfo CreateActionInfoFromId(ActionId actionId, EncounterCharacter character)
    {
        ActionInfo info = null;

        if (actionId == ActionId.WeaponAttack)
        {
            actionId = ((WeaponEquippable)character.Equipment[Equipment.Slot.RightHand]).Action;
        }

        switch (actionId)
        {
            case (ActionId.SwordAttack):
            {
                WeaponEquippable weapon = (WeaponEquippable)character.Equipment[Equipment.Slot.RightHand];

                StateChange cost = new StateChange(StateChangeType.ActionCost, 0, 0);
                
                info = new WeaponActionInfoBase(weapon.Action, weapon, cost, ActionRange.Meele, ActionConstants.INSTANT_CAST_SPEED, weapon.Range, RangeInfo.Unit);
            }
            break;

            case (ActionId.BowAttack):
            {
                WeaponEquippable weapon = (WeaponEquippable)character.Equipment[Equipment.Slot.RightHand];

                StateChange cost = new StateChange(StateChangeType.ActionCost, 0, 0);

                info = new WeaponActionInfoBase(weapon.Action, weapon, cost, ActionRange.Projectile, ActionConstants.INSTANT_CAST_SPEED, weapon.Range, RangeInfo.Unit);
            }
            break;

            case (ActionId.Riptose):
            {
                int minCastRange = 1, maxCastRange = 1, maxCastElevationChange = 1;
                RangeInfo castRange = new RangeInfo(minCastRange, maxCastRange, maxCastElevationChange, AreaType.Circle);

                int minSelectionRange = 0, maxSelectionRange = 0, maxSelectionElevationChange = 0;
                RangeInfo effectRange = new RangeInfo(minSelectionRange, maxSelectionRange, maxSelectionElevationChange, AreaType.Circle);

                HeldEquippable weapon = (HeldEquippable)character.Equipment[Equipment.Slot.RightHand];
                StateChange cost = new StateChange(StateChangeType.ActionCost, 0, 0);

                info = new WeaponActionInfoBase(ActionId.Riptose, weapon, cost, ActionRange.Meele, ActionConstants.INSTANT_CAST_SPEED, castRange, effectRange);
            }
            break;

            case (ActionId.MightyBlow):
            {
                int minCastRange = 1, maxCastRange = 1, maxCastElevationChange = 1;
                RangeInfo castRange = new RangeInfo(minCastRange, maxCastRange, maxCastElevationChange, AreaType.Circle);

                int minSelectionRange = 0, maxSelectionRange = 0, maxSelectionElevationChange = 0;
                RangeInfo effectRange = new RangeInfo(minSelectionRange, maxSelectionRange, maxSelectionElevationChange, AreaType.Circle);

                HeldEquippable weapon = (HeldEquippable)character.Equipment[Equipment.Slot.RightHand];

                int bloodScentCount = character.GetStackCountForStatus(StatusEffectType.BloodScent, character);
                StatusEffect bloodScentCost = StatusEffectFactory.CheckoutStatusEffect(StatusEffectType.BloodScent, character, bloodScentCount);
                StateChange cost = new StateChange(StateChangeType.ActionCost, 0, 0, new List<StatusEffect>() { bloodScentCost } );

                info = new MightyBlowInfo((HeldEquippable)character.Equipment[Equipment.Slot.RightHand], 
                    cost, ActionRange.Meele, ActionConstants.INSTANT_CAST_SPEED, castRange, effectRange);
            }
            break;

            case (ActionId.Heal):
                {
                    int minCastRange = 0, maxCastRange = 2, maxCastElevationChange = 1;
                    RangeInfo castRange = new RangeInfo(minCastRange, maxCastRange, maxCastElevationChange, AreaType.Circle);

                    int minSelectionRange = 0, maxSelectionRange = 1, maxSelectionElevationChange = 0;
                    RangeInfo effectRange = new RangeInfo(minSelectionRange, maxSelectionRange, maxSelectionElevationChange, AreaType.Circle);

                    info = new HealInfo(ActionConstants.FAST_CAST_SPEED, castRange, effectRange);
                }
                break;

            case (ActionId.Protection):
                {
                    int minCastRange = 0, maxCastRange = 2, maxCastElevationChange = 1;
                    RangeInfo castRange = new RangeInfo(minCastRange, maxCastRange, maxCastElevationChange, AreaType.Circle);

                    int minSelectionRange = 0, maxSelectionRange = 1, maxSelectionElevationChange = 0;
                    RangeInfo effectRange = new RangeInfo(minSelectionRange, maxSelectionRange, maxSelectionElevationChange, AreaType.Circle);

                    info = new ProtectionInfo(ActionConstants.FAST_CAST_SPEED, castRange, effectRange);
                }
                break;

            case (ActionId.FireBall):
            {
                int minCastRange = 1, maxCastRange = 4, maxCastElevationChange = 2;
                RangeInfo castRange = new RangeInfo(minCastRange, maxCastRange, maxCastElevationChange, AreaType.Circle);

                int minSelectionRange = 0, maxSelectionRange = 0, maxSelectionElevationChange = 0;
                RangeInfo effectRange = new RangeInfo(minSelectionRange, maxSelectionRange, maxSelectionElevationChange, AreaType.Circle);

                info = new FireballInfo(ActionConstants.FAST_CAST_SPEED, castRange, effectRange);
            }
            break;

            default:
                throw new Exception();
        }

        return info;
    }

    public static ActionResponseInfo CreateActionResponseInfoFromId(ActionResponseId responseId)
    {
        ActionResponseInfo info = null;

        switch (responseId)
        {
            case (ActionResponseId.Riptose):
                {
                    info = new ActionResponseInfo(25, 1);
                }
                break;

            case (ActionResponseId.HealOnHurtListener):
                {
                    info = new ActionResponseInfo(100, 3);
                }
                break;

            case (ActionResponseId.BloodScent):
                {
                    info = new ActionResponseInfo(100, ActionResponseInfo.INFINITE_RANGE);
                }
                break;

            default:
                throw new Exception();
        }

        return info;
    }
}

