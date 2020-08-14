using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.DB
{
    [System.Serializable]
    class DBScenarioInfo : Internal.DBEntryBase
    {
        public int Id = -1;
        public string Name = "INVALID";
        public bool IsActive = false;

        public override void Copy(Internal.DBEntryBase _from, Internal.DBEntryBase _to)
        {
            DBScenarioInfo from = _from as DBScenarioInfo;
            DBScenarioInfo to = _to as DBScenarioInfo;

            to.Id = from.Id;
            to.Name = from.Name;
            to.IsActive = from.IsActive;
        }
    }

}
