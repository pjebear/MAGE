using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

enum TeamSide
{
    INVALID = -1,

    AllyHuman,
    EnemyAI,

    NUM
}

class Teams
{
    List<int>[] mTeams = null;

    public List<int> this[TeamSide side]
    {
        get
        {
            return mTeams[(int)side];
        }

        set
        {
            Debug.Assert(value != null);
            mTeams[(int)side] = value;
        }
    }

    public Teams()
    {
        mTeams = new List<int>[(int)TeamSide.NUM];
        for (int i = 0; i < mTeams.Count(); ++i)
        {
            mTeams[i] = new List<int>();
        }
    }
}

struct EncounterCreateParams
{

}

struct EncounterResultInfo
{
    public Teams PlayersInEncounter;
    public bool DidUserWin;
}



