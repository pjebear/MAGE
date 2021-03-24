using MAGE.GameModes.LevelManagement;
using MAGE.GameModes.SceneElements;
using MAGE.GameSystems.Loot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.World
{
    class EncounterContext
    {
        public int MaxAllyUnits;
        public EncounterType EncounterType;
        public ClaimLootInfo Rewards = new ClaimLootInfo();

        public LevelId LevelId;
        public TileIdx BottomLeft;
        public TileIdx TopRight;

        public Dictionary<int, TileIdx> CharacterPositions = new Dictionary<int, TileIdx>();

        public List<EncounterCondition> WinConditions = new List<EncounterCondition>();
        public List<EncounterCondition> LoseConditions = new List<EncounterCondition>();

        public EncounterContext()
        {

        }
    }
}



