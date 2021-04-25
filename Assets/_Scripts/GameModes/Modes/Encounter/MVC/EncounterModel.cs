using MAGE.GameModes.Combat;
using MAGE.GameModes.SceneElements;
using MAGE.GameModes.SceneElements.Encounters;
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
        public List<EncounterCondition> mWinConditions = new List<EncounterCondition>();
        public List<EncounterCondition> mLoseConditions = new List<EncounterCondition>();

        public Queue<ActionProposal> mActionQueue = new Queue<ActionProposal>();
        public Dictionary<ActionProposal, int> mDelayedActions = new Dictionary<ActionProposal, int>();

        public Dictionary<TeamSide, List<CombatCharacter>> Teams = new Dictionary<TeamSide, List<CombatCharacter>>();
        public Dictionary<int, CombatCharacter> Players = new Dictionary<int, CombatCharacter>();
        public IEnumerable<CombatCharacter> AlivePlayers { get { return Players.Values.Where(x => x.GetComponent<ResourcesControl>().IsAlive()); } }
        
        public List<CombatCharacter> TurnQueue = new List<CombatCharacter>();

        public CombatCharacter CurrentTurn = null;
        public bool TurnComplete = false;

        public bool IsEncounterLost()
        {
            bool isLost = true;

            isLost = mLoseConditions.Where(x => x.IsConditionMet(this)).Count() > 0;

            return isLost;
        }

        public bool IsEncounterWon()
        {
            bool isWon = mWinConditions.Where(x => x.IsConditionMet(this)).Count() > 0;

            return isWon;
        }

        public bool IsEncounterOver()
        {
            return IsEncounterLost() || IsEncounterWon();
        }
    }
}


