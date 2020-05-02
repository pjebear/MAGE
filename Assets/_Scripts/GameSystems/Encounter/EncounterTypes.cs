using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

enum EncounterType
{
    Random,
    Scenario,

    NUM
}

enum EncounterScenarioId
{
    Random,
    Debug,

    NUM
}

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
    public LevelId LevelId;
    public EncounterScenarioId ScenarioId;

    public TileIdx BottomLeft;
    public TileIdx TopRight;
}

class EncounterResultInfo
{
    public Teams PlayersInEncounter = new Teams();
    public bool DidUserWin = false;
    public int CurrencyReward = 0;
    public List<int> ItemRewards = new List<int>();
}



