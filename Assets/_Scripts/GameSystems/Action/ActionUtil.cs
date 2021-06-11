using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Actions
{
    static class ActionUtil
    {
        public static RangeInfo FromDB(DB.DBRangeInfo dbEntry)
        {
            RangeInfo rangeInfo = new RangeInfo();

            rangeInfo.AreaType = (AreaType)dbEntry.AreaType;
            rangeInfo.TargetingType = (TargetingType)dbEntry.TargetingType;
            rangeInfo.MinRange = dbEntry.Min;
            rangeInfo.MaxRange = dbEntry.Max;
            rangeInfo.MaxElevationChange = dbEntry.Elevation;

            return rangeInfo;
        }

        public static void FromDB(DB.DBAction dbAction, ActionInfo fromDB)
        {
            fromDB.ActionId = (ActionId)dbAction.Id;
            fromDB.ActionRange = (ActionRange)dbAction.ActionRange;
            fromDB.ActionSource = (ActionSource)dbAction.ActionSource;
            fromDB.CastSpeed = (CastSpeed)dbAction.CastSpeed;
            fromDB.CastRange = FromDB(dbAction.CastRange);
            fromDB.EffectRange = FromDB(dbAction.EffectRange);
            fromDB.AnimationInfo.AnimationId = (AnimationId)dbAction.AnimationId;
            fromDB.ProjectileInfo.ProjectileId = (ProjectileId)dbAction.ProjectileId;
            fromDB.EffectInfo.EffectId = (EffectType)dbAction.EffectId;
            fromDB.EffectInfo.SFXId = (SFXId)dbAction.SFXId;
            fromDB.ChainInfo.NumChainTargets = dbAction.NumChainBounces;
            fromDB.ChainInfo.ChainEffectFalloff = dbAction.ChainFalloff;
            fromDB.SummonInfo.SummonType = (SummonType)dbAction.SummonType;
            fromDB.SummonInfo.SummonCount = dbAction.SummonCount;
            fromDB.SummonInfo.MaxSummonCount = dbAction.MaxSummonCount;
            fromDB.IsSelfCast = dbAction.IsSelfCast;
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
}


