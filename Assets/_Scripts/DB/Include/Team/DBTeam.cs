using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.DB
{
    [System.Serializable]
    class DBTeam : Internal.DBEntryBase
    {
        public List<int> CharacterIds = new List<int>();

        public override string ToString()
        {
            string toString = "Team: ";

            foreach (int id in CharacterIds)
            {
                toString += id + ",";
            }

            toString.Remove(toString.Length - 1);

            return toString;
        }

        public override void Copy(Internal.DBEntryBase _from, Internal.DBEntryBase _to)
        {
            DBTeam from = _from as DBTeam;
            DBTeam to = _to as DBTeam;

            to.CharacterIds.Clear();
            to.CharacterIds.AddRange(from.CharacterIds);
        }
    }
}


