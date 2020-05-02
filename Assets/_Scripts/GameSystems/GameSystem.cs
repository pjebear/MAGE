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
    }

    // ! Encounter
    public void PrepareEncounter(EncounterCreateParams encounterParams)
    {
        PartySystem.PrepareForEncounter(encounterParams);
        EncounterSystem.PrepareEncounter(encounterParams);
    }

    public void UpdateOnEncounterEnd(EncounterResultInfo resultInfo)
    {
        PartySystem.UpdateOnEncounterEnd(resultInfo);
        EncounterSystem.CleanupEncounter();
    }

    public EncounterContext GetEncounterContext()
    {
        return EncounterSystem.GetEncounterContext();
    }
    // ! Encounter - End

    //! Location
    public LevelId GetCurrentLevel()
    {
        return LocationSystem.PartyLocation;
    }

    // Location - End
    //! Party
    public List<int> GetCharactersInParty()
    {
        return PartySystem.GetCharactersInParty();
    }

    public Inventory GetInventory()
    {
        return PartySystem.GetInventory();
    }

    public void AddToInventory(int itemId)
    {
        PartySystem.AddToInventory(itemId);
    }

    public int GetPartyAvatarId()
    {
        return PartySystem.GetPartyAvatarId();
    }

    //! Party End

    // ! SaveLoad
    public void PrepareNewGame()
    {
        SpecializationDBLoader.LoadDB();
        EquipmentDBLoader.LoadDB();

        PartySystem.CreateDefaultParty();
    }

    public void Save()
    {
        PartySystem.Save();
    }

    public void Load(string saveFileName)
    {
        PartySystem.Load(saveFileName);
    }
    //! SaveLoad End
}

