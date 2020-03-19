using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    class EquipmentInfo
    {
        public List<int> EquipmentIds = new List<int>();

        public override string ToString()
        {
            string toString = "EquipmentIds: ";

            foreach(int equipmentId in EquipmentIds)
            {
                toString += equipmentId + ",";
            }

            toString.Remove(toString.Length - 1);

            return toString;
        }
    }

    class DBEquipment : DBEntryBase<EquipmentInfo>
    {
        public override void Copy(EquipmentInfo from, EquipmentInfo to)
        {
            to.EquipmentIds.Clear();
            to.EquipmentIds.AddRange(from.EquipmentIds);
        }
    }
}


