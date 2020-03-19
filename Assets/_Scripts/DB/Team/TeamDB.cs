using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DB
{
    class TeamDB : DBBase<TeamSide, DBTeam, Team>
    {
        protected Dictionary<int, TeamSide> mCharacterTeamLookup;

        public TeamDB() : base("TeamDB")
        {
        }
    }
}



