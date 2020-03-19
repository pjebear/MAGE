using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class ActionFactory
{
    public static ActionInfo CreateActionInfoFromId(ActionId actionId)
    {
        ActionInfo info = null;

        switch (actionId)
        {
            case (ActionId.Riptose):
            case (ActionId.SwordAttack):
                {
                    int minCastRange = 1, maxCastRange = 1, maxCastElevationChange = 1;
                    RangeInfo castRange = new RangeInfo(minCastRange, maxCastRange, maxCastElevationChange, AreaType.Cross);

                    int minSelectionRange = 0, maxSelectionRange = 0, maxSelectionElevationChange = 0;
                    RangeInfo effectRange = new RangeInfo(minSelectionRange, maxSelectionRange, maxSelectionElevationChange, AreaType.Circle);

                    info = new MeleeAttackInfo(castRange, effectRange);
                }
                break;

            case (ActionId.MightyBlow):
                {
                    int minCastRange = 1, maxCastRange = 1, maxCastElevationChange = 1;
                    RangeInfo castRange = new RangeInfo(minCastRange, maxCastRange, maxCastElevationChange, AreaType.Cross);

                    int minSelectionRange = 0, maxSelectionRange = 0, maxSelectionElevationChange = 0;
                    RangeInfo effectRange = new RangeInfo(minSelectionRange, maxSelectionRange, maxSelectionElevationChange, AreaType.Circle);

                    info = new MightyBlowInfo(castRange, effectRange);
                }
                break;

            case (ActionId.Heal):
                {
                    int minCastRange = 1, maxCastRange = 3, maxCastElevationChange = 1;
                    RangeInfo castRange = new RangeInfo(minCastRange, maxCastRange, maxCastElevationChange, AreaType.Cross);

                    int minSelectionRange = 0, maxSelectionRange = 2, maxSelectionElevationChange = 0;
                    RangeInfo effectRange = new RangeInfo(minSelectionRange, maxSelectionRange, maxSelectionElevationChange, AreaType.Cross);

                    info = new HealInfo(castRange, effectRange);
                }
                break;

            case (ActionId.Protection):
                {
                    int minCastRange = 1, maxCastRange = 3, maxCastElevationChange = 1;
                    RangeInfo castRange = new RangeInfo(minCastRange, maxCastRange, maxCastElevationChange, AreaType.Cross);

                    int minSelectionRange = 0, maxSelectionRange = 1, maxSelectionElevationChange = 0;
                    RangeInfo effectRange = new RangeInfo(minSelectionRange, maxSelectionRange, maxSelectionElevationChange, AreaType.Cross);

                    info = new ProtectionInfo(castRange, effectRange);
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

