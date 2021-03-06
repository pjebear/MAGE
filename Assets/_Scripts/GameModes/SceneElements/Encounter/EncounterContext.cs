﻿using MAGE.GameModes.LevelManagement;
using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using MAGE.GameSystems.Loot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes.SceneElements.Encounters
{
    class EncounterContext
    {
        public int MaxAllyUnits;
        public EncounterType EncounterType;
        public ClaimLootInfo Rewards = new ClaimLootInfo();

        public LevelId LevelId;

        public List<EncounterCondition> WinConditions = new List<EncounterCondition>();
        public List<EncounterCondition> LoseConditions = new List<EncounterCondition>();

        public EncounterContext()
        {

        }
    }
}



