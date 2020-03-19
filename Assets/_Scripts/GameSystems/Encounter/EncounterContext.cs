using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class EncounterContext
{
    public List<EncounterCondition> WinConditions = new List<EncounterCondition>();
    public List<EncounterCondition> LoseConditions = new List<EncounterCondition>();

    public EncounterContext()
    {
        WinConditions = new List<EncounterCondition>();
        LoseConditions = new List<EncounterCondition>();  
    }
}

