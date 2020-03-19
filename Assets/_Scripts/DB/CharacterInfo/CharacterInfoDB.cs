using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DB
{
    class CharacterInfoDB : DBBase<int, DBCharacterInfo, CharacterInfo>
    {
        public CharacterInfoDB() : base("CharacterInfoDB")
        {
        }
    }
}


