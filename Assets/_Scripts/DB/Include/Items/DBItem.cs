using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.DB
{
    [System.Serializable]
    class DBItem : Internal.DBEntryBase
    {
        public int Id = -1;
        public string Name = "INVALID";
        public int SpriteId = -1;
        public int Value = -1;
        
        public override void Copy(Internal.DBEntryBase _from, Internal.DBEntryBase _to)
        {
            DBItem from = _from as DBItem;
            DBItem to = _to as DBItem;

            to.Id = from.Id;
            to.Name = from.Name;
            to.SpriteId = from.SpriteId;
            to.Value = from.Value;
        }
    }
}
