using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class GameSystem
{
    private LocationSystem LocationSystem;
    private PartySystem PartySystem;
    private EncounterSystem EncounterSystem;

    public GameSystem()
    {
        LocationSystem = new LocationSystem();
        PartySystem = new PartySystem();
        EncounterSystem = new EncounterSystem();

        PartySystem.CreateDefaultParty();
    }

    // ! Encounter
    public void PrepareEncounter(EncounterCreateParams encounterParams)
    {
        EncounterSystem.PrepareEncounter(encounterParams);
    }

    public void UpdateOnEncounterEnd(EncounterResultInfo resultInfo)
    {
        PartySystem.UpdatePartyOnEncounterEnd(resultInfo);
        EncounterSystem.CleanupEncounter();
    }

    public EncounterContext GetEncounterContext()
    {
        return EncounterSystem.GetEncounterContext();
    }
    // ! Encounter - End
}

