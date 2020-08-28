using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes.Encounter
{
    class EncounterModel
    {
        public EncounterContext EncounterContext;
        public List<TileIdx> AllySpawnPoints = new List<TileIdx>();
        public List<TileIdx> EnemySpawnPoints = new List<TileIdx>();

        public Dictionary<TeamSide, List<Character>> Teams = new Dictionary<TeamSide, List<Character>>();
        public Dictionary<int, Character> Characters = new Dictionary<int, Character>();
        public List<Character> PendingCharacterTurns = new List<Character>();
        public Character CurrrentTurnCharacter = null;
        public bool TurnCompleted = false;
        public int Clock;
        public EncounterState EncounterState = EncounterState.InProgress;
    }
}


