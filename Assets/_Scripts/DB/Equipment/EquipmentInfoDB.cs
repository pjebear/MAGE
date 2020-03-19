using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DB
{
    class EquipmentInfoDB : DBBase<int, DBEquipment, EquipmentInfo>
    {
        public EquipmentInfoDB() : base("EquipmentInfoDB")
        {
        }
    }
}



