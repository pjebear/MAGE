using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.DB
{
    [System.Serializable]
    class DBAction : Internal.DBEntryBase
    {
        public int Id = -1;
        public string Name = "INVALID";
        public int ActionRange = -1;
        public int ActionSource = -1;
        public int CastSpeed = -1;
        public int AnimationId = -1;
        public int ProjectileId = -1;
        public int EffectId = -1;
        public int NumChainBounces = -1;
        public float ChainFalloff = -1;
        public bool IsSelfCast = false;

        public DBRangeInfo CastRange = new DBRangeInfo();
        public DBRangeInfo EffectRange = new DBRangeInfo();

        public override void Copy(Internal.DBEntryBase _from, Internal.DBEntryBase _to)
        {
            DBAction from = _from as DBAction;
            DBAction to = _to as DBAction;

            to.Id = from.Id;
            to.Name = from.Name;
            to.ActionRange = from.ActionRange;
            to.ActionSource = from.ActionSource;
            to.CastSpeed = from.CastSpeed;
            to.AnimationId = from.AnimationId;
            to.ProjectileId = from.ProjectileId;
            to.EffectId = from.EffectId;
            to.NumChainBounces = from.NumChainBounces;
            to.ChainFalloff = from.ChainFalloff;
            to.IsSelfCast = from.IsSelfCast;

            to.CastRange.Set(from.CastRange);
            to.EffectRange.Set(from.EffectRange);
        }
    }
    
}
