using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class EncounterModel
{
    public Dictionary<TeamSide, List<EncounterCharacter>> Teams = new Dictionary<TeamSide, List<EncounterCharacter>>(); 
    public Dictionary<int, EncounterCharacter> Characters = new Dictionary<int, EncounterCharacter>();
    public List<EncounterCondition> WinConditions = new List<EncounterCondition>();
    public List<EncounterCondition> LoseConditions = new List<EncounterCondition>();
    public List<EncounterCharacter> TurnOrder = new List<EncounterCharacter>();
    public int Clock;
    public EncounterState EncounterState = EncounterState.InProgress;
}

