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

    //! Party End

    // ! SaveLoad
    public void PrepareNewGame()
    {
        CreateSpecializationDB();

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

    void CreateSpecializationDB()
    {
        // Footman
        {
            DB.DBSpecialization dbSpecialization = new DB.DBSpecialization();

            // Type
            dbSpecialization.SpecializationType = (int)SpecializationType.Footman;
             
            // Proficiencies
            dbSpecialization.Proficiencies = new List<int>()
            {
                (int)ProficiencyType.OneHands,
                (int)ProficiencyType.Sheild,
                (int)ProficiencyType.Chain,
                (int)ProficiencyType.Leather
            };

            // Level up modifiers
            dbSpecialization.LevelUpModifiers = new List<DB.DBAttribute>()
            {
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)PrimaryStat.Might, Value = 4},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)PrimaryStat.Finese, Value = 3},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)PrimaryStat.Magic, Value = 2},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)SecondaryStat.Fortitude, Value = 5},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)SecondaryStat.Attunement, Value = 1}
            };

            // talents
            dbSpecialization.TalentIds = new List<int>()
            {
                (int)TalentId.BlockIncrease,
                (int)TalentId.MightyBlow
            };

            // Actions
            // none

            DB.DBHelper.WriteSpecialization(dbSpecialization);
        }

        // Monk
        {
            DB.DBSpecialization dbSpecialization = new DB.DBSpecialization();

            // Type
            dbSpecialization.SpecializationType = (int)SpecializationType.Monk;

            // Proficiencies
            dbSpecialization.Proficiencies = new List<int>()
            {
                (int)ProficiencyType.Staff,
                (int)ProficiencyType.Cloth
            };

            // Level up modifiers
            dbSpecialization.LevelUpModifiers = new List<DB.DBAttribute>()
            {
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)PrimaryStat.Might, Value = 2},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)PrimaryStat.Finese, Value = 1},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)PrimaryStat.Magic, Value = 4},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)SecondaryStat.Fortitude, Value = 1},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)SecondaryStat.Attunement, Value = 5}
            };

            // talents
            dbSpecialization.TalentIds = new List<int>()
            {
                (int)TalentId.HealIncrease,
                (int)TalentId.HealOnHurt
            };

            // Actions
            dbSpecialization.ActionIds = new List<int>()
            {
                (int)ActionId.Heal,
                (int)ActionId.Protection
            };

            DB.DBHelper.WriteSpecialization(dbSpecialization);
        }

        DB.DBHelper.UpdateSpecializationDB();
    }
}

