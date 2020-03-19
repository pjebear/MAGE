using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    [System.Serializable]
    class DBEquipment : DBEntryBase
    {
        public List<int> EquipmentIds = new List<int>();

        public override string ToString()
        {
            string toString = "EquipmentIds: ";

            foreach (int equipmentId in EquipmentIds)
            {
                toString += equipmentId + ",";
            }

            toString.Remove(toString.Length - 1);

            return toString;
        }

        public override void Copy(DBEntryBase _from, DBEntryBase _to)
        {
            DBEquipment from = _from as DBEquipment;
            DBEquipment to = _to as DBEquipment;

            to.EquipmentIds.Clear();
            to.EquipmentIds.AddRange(from.EquipmentIds);
        }
    }
}


