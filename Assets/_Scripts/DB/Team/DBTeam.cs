using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    class Team
    {
        public List<int> CharacterIds = new List<int>();

        public override string ToString()
        {
            string toString = "Team: ";

            foreach(int id in CharacterIds)
            {
                toString += id + ",";
            }

            toString.Remove(toString.Length - 1);

            return toString;
        }
    }

    class DBTeam : DBEntryBase<Team>
    {
        public override void Copy(Team from, Team to)
        {
            to.CharacterIds.Clear();
            to.CharacterIds.AddRange(from.CharacterIds);
        }
    }
}


