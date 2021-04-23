﻿using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using MAGE.GameSystems.Loot;
using MAGE.GameSystems.Mobs;
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
    Demo_LotharUnderAttack,

    NUM
}

enum TeamSide
{
    INVALID = -1,

    AllyHuman,
    EnemyAI,

    NUM
}

class EncounterResultInfo
{
    public EncounterScenarioId EncounterScenarioId = EncounterScenarioId.Random;
    public List<int> PlayersInEncounter = new List<int>();
    public bool DidUserWin = false;
    public ClaimLootInfo Rewards = new ClaimLootInfo();
}

class EncounterInfo
{
    public EncounterScenarioId EncounterScenarioId = EncounterScenarioId.Random;
    public bool IsActive = false;
    public bool IsVisible = false;
    public LevelId LevelId;
}

class EncounterAwards
{
    public int Currency = 0;
    public Dictionary<int, Item> Items = new Dictionary<int, Item>();
}



