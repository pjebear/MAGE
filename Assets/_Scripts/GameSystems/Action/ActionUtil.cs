using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


static class ActionUtil
{
    public static void FromDB(DB.DBAction dbAction, ActionInfo fromDB)
    {
        fromDB.ActionId = (ActionId)dbAction.Id;
        fromDB.ActionRange = (ActionRange)dbAction.ActionRange;
        fromDB.ActionSource = (ActionSource)dbAction.ActionSource;
        fromDB.CastSpeed = (CastSpeed)dbAction.CastSpeed;
        fromDB.CastRange = DBUtil.FromDB(dbAction.CastRange);
        fromDB.EffectRange = DBUtil.FromDB(dbAction.EffectRange);
        fromDB.AnimationInfo.AnimationId = (AnimationId)dbAction.AnimationId;
        fromDB.ProjectileInfo.ProjectileId = (ProjectileId)dbAction.ProjectileId;
        fromDB.EffectInfo.EffectId = (EffectType)dbAction.EffectId;
        fromDB.ChainInfo.NumChainTargets = dbAction.NumChainBounces;
        fromDB.ChainInfo.ChainEffectFalloff = dbAction.ChainFalloff;
    }

    public static int GetTurnCountForCastSpeed(CastSpeed castSpeed)
    {
        int turnCount = ActionConstants.INSTANT_CAST_SPEED;
        switch (castSpeed)
        {
            case CastSpeed.Instant: turnCount = ActionConstants.INSTANT_CAST_SPEED; break;
            case CastSpeed.Slow: turnCount = ActionConstants.SLOW_CAST_SPEED; break;
            case CastSpeed.Normal: turnCount = ActionConstants.NORMAL_CAST_SPEED; break;
            case CastSpeed.Fast: turnCount = ActionConstants.FAST_CAST_SPEED; break;

            default:
                UnityEngine.Debug.Assert(false);
                break;
        }

        return turnCount;
    }
}

