using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.DB
{
    [System.Serializable]
    class DBEncounterInfo : Internal.DBEntryBase
    {
        public int Id;
        public string Name;
        public bool IsActive;
        public bool IsVisible;
        public int LevelId;
        
        public override void Copy(Internal.DBEntryBase _from, Internal.DBEntryBase _to)
        {
            DBEncounterInfo from = _from as DBEncounterInfo;
            DBEncounterInfo to = _to as DBEncounterInfo;

            to.Id = from.Id;
            to.Name = from.Name;
            to.IsActive = from.IsActive;
            to.IsVisible = from.IsVisible;
            to.LevelId = from.LevelId;
        }
    }
}
