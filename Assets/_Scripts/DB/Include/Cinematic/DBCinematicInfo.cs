using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.DB
{
    [System.Serializable]
    class DBCinematicInfo : Internal.DBEntryBase
    {
        public int Id;
        public string Name;
        public bool IsActive;
        
        public override void Copy(Internal.DBEntryBase _from, Internal.DBEntryBase _to)
        {
            DBCinematicInfo from = _from as DBCinematicInfo;
            DBCinematicInfo to = _to as DBCinematicInfo;

            to.Id = from.Id;
            to.Name = from.Name;
            to.IsActive = from.IsActive;
        }
    }
}
