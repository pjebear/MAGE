using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DB
{
    class SpecializationDB : DBBase<int, DBSpecializations, SpecializationsInfo>
    {
        public SpecializationDB() : base ("SpecializationDB")
        {

        }
    }
}