using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using MAGE.GameSystems.Loot;
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
    INVALID = -1,

    Random,
    Scenario,

    // Demo Level
    Demo_TrainingGrounds,

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
    public ClaimLootInfo Rewards = new ClaimLootInfo();
}

class EncounterInfo
{
    public EncounterScenarioId EncounterScenarioId = EncounterScenarioId.Random;
    public bool IsActive = false;
    public LevelId LevelId;
}

class EncounterAwards
{
    public int Currency = 0;
    public Dictionary<int, Item> Items = new Dictionary<int, Item>();
}



