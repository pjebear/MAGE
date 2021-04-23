using MAGE.GameModes.Combat;
using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
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
        public Queue<ActionProposal> mActionQueue = new Queue<ActionProposal>();
        public Dictionary<ActionProposal, int> mDelayedActions = new Dictionary<ActionProposal, int>();

        public Dictionary<TeamSide, List<CombatCharacter>> Teams = new Dictionary<TeamSide, List<CombatCharacter>>();
        public Dictionary<int, CombatCharacter> Players = new Dictionary<int, CombatCharacter>();
        public IEnumerable<CombatCharacter> AlivePlayers { get { return Players.Values.Where(x => x.GetComponent<ResourcesControl>().IsAlive()); } }
        
        public List<CombatCharacter> TurnQueue = new List<CombatCharacter>();

        public CombatCharacter CurrentTurn = null;
        public bool TurnComplete = false;
    }
}


