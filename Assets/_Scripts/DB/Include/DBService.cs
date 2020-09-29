using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems
{
    interface IDBService : Services.IService
    {
        // Actions
        DB.DBAction LoadAction(int actionId);
        void UpdateActionDB();
        void WriteAction(int actionId, DB.DBAction action);
        // Actions - End

        // Appearance
        DB.DBAppearance LoadAppearance(int appearanceId);
        void RegisterForAppearanceUpdates(object listener, DB.DBUpdateCB<int> cb);
        void UpdateAppearanceDB();
        void UnRegisterForAppearanceUpdates(object listener);
        void WriteAppearance(int appearanceId, DB.DBAppearance appearance);
        // Appearance - End

        // Character
        void ClearCharacter(int characterId);
        List<int> GetAllCharacterIds();
        DB.DBCharacter LoadCharacter(int characterId);
        void RegisterForCharacterUpdates(object listener, DB.DBUpdateCB<int> cb);
        void RemoveCharacter(int characterId);
        void UnRegisterForCharacterUpdates(object listener);
        DB.DBCharacter WriteCharacter(DB.DBCharacter character, TeamSide teamSide = TeamSide.INVALID);
        DB.DBCharacter WriteNewCharacter(DB.DBCharacter character, TeamSide teamSide = TeamSide.INVALID);
        // Character - End

        // Cinematic
        DB.DBCinematicInfo LoadCinematicInfo(int cinematicId);
        void RegisterForCinematicUpdates(object listener, DB.DBUpdateCB<int> cb);
        void UnRegisterForCinematicUpdates(object listener);
        void UpdateCinematicDB();
        void WriteCinematicInfo(int cinematicId, DB.DBCinematicInfo cinematicInfo);
        // Cinematic - End

        // Conversation
        DB.DBConversation LoadConversation(int conversationId);
        void UpdateConversationDB();
        void WriteConversation(int conversationId, DB.DBConversation conversation);
        // Conversation - End

        // Encounter
        DB.DBEncounterInfo LoadEncounterInfo(int encounterId);
        void RegisterForEncounterUpdates(object listener, DB.DBUpdateCB<int> cb);
        void UnRegisterForEncounterUpdates(object listener);
        void UpdateEncounterDB();
        void WriteEncounterInfo(int encounterId, DB.DBEncounterInfo encounterInfo);
        // Encounter - End

        // Equipment
        DB.DBEquipment LoadEquipment(int equipmentId);
        void UpdateEquipmentDB();
        void WriteEquipment(int equipmentId, DB.DBEquipment equipment);
        // Equipment - End

        // Item
        DB.DBItem LoadItem(int itemId);
        void UpdateItemDB();
        void WriteItem(int itemId, DB.DBItem item);
        // Item - End

        // Props
        DB.DBPropInfo LoadPropInfo(int propId);
        void RegisterForPropUpdates(object listener, DB.DBUpdateCB<int> cb);
        void UnRegisterForPropUpdates(object listener);
        void UpdatePropDB();
        void WritePropInfo(int propId, DB.DBPropInfo propInfo);
        // Props - End

        // Scenario
        DB.DBScenarioInfo LoadScenarioInfo(int scenarioId);
        void UpdateScenarioDB();
        void WriteScenarioInfo(int scenarioId, DB.DBScenarioInfo scnearioInfo);
        // Scenario - End

        // Specialization
        DB.DBSpecialization LoadSpecialization(MAGE.GameSystems.Characters.SpecializationType specializationType);
        void UpdateSpecializationDB();
        void WriteSpecialization(DB.DBSpecialization dBSpecialization);
        // Specialization - End

        // Story
        DB.DBStoryArcInfo LoadStoryArcInfo(int storyArcId);
        void UpdateStoryDB();
        void WriteStoryArcInfo(int storyArcId, DB.DBStoryArcInfo storyArcInfo);
        // Story - End

        // Team 
        void AddToTeam(int characterId, TeamSide teamSide);
        void ClearTeam(TeamSide teamSide);
        List<int> LoadTeam(TeamSide teamSide);
        // Team - End

        // Save Load
        void Load(string path);
        void Save(string path);
        // Save Load - End
    }

    abstract class DBService : Services.ServiceBase<IDBService> { };
}
