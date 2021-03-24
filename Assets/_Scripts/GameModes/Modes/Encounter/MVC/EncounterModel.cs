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

        public List<CombatCharacter> Players = new List<CombatCharacter>();
        public List<CombatCharacter> TurnQueue = new List<CombatCharacter>();

        public CombatCharacter CurrentTurn = null;
        public bool HasActed = false;
        public bool HasMoved = false;
    }
}


