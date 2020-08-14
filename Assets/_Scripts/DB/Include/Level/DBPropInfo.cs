using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.DB
{
    [System.Serializable]
    class DBPropInfo : Internal.DBEntryBase
    {
        public int Id = -1;
        public string Name = "INVALID";
        public bool IsActive = false;
        public bool IsInteractable = false;
        public int Currency = 0;
        public int AppearanceId = -1;
        public List<int> Inventory = new List<int>();
        public List<int> Conversations = new List<int>();

        public override void Copy(Internal.DBEntryBase _from, Internal.DBEntryBase _to)
        {
            DBPropInfo from = _from as DBPropInfo;
            DBPropInfo to = _to as DBPropInfo;

            to.Id = from.Id;
            to.Name = from.Name;
            to.IsActive = from.IsActive;
            to.IsInteractable = from.IsInteractable;
            to.Currency = from.Currency;
            to.AppearanceId = from.AppearanceId;

            to.Inventory = new List<int>(from.Inventory);
            to.Conversations = new List<int>(from.Conversations);
        }
    }
}
