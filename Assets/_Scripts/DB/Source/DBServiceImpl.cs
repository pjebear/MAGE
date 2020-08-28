using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.DB.Internal
{
    class DBServiceImpl : GameSystems.IDBService
    {
        private static readonly string TAG = "DBServiceImpl";
        static int sNewCharacterId = 0;

        ActionDB mActionDB = new ActionDB();
        AppearnaceDB mAppearanceDB = new AppearnaceDB();
        CharacterDB mCharacterDB = new CharacterDB();
        ConversationDB mConversationDB = new ConversationDB();
        EquipmentDB mEquipmentDB = new EquipmentDB();
        ItemDB mItemDB = new ItemDB();
        PropDB mPropDB = new PropDB();
        ScenarioDB mScenarioDB = new ScenarioDB();
        SpecializationDB mSpecializationDB = new SpecializationDB();
        StoryDB mStoryDB = new StoryDB();

        TeamDB mTeamDB = new TeamDB();

        public DBServiceImpl()
        {
            
        }

        // IService
        public void Init()
        {
            ActionDBLoader.LoadDB();
            ConversationDBLoader.LoadDB();
            EquipmentDBLoader.LoadDB();
            ItemDBLoader.LoadDB();
            PropDBLoader.LoadDB();
            ScenarioDBLoader.LoadDB();
            SpecializationDBLoader.LoadDB();
            StoryDBLoader.LoadDB();

            CharacterDBLoader.LoadDB();
        }

        public void Takedown()
        {
            // empty
        }

        // Action
        public DB.DBAction LoadAction(int actionId)
        {
            return mActionDB.Load(actionId);
        }

        public void UpdateActionDB()
        {
            mActionDB.Save(FileUtil.FolderName.DB.ToString());
        }

        public void WriteAction(int actionId, DB.DBAction action)
        {
            mActionDB.Write(actionId, action);
        }
        // Action - End

        // Appearance
        public DB.DBAppearance LoadAppearance(int appearanceId)
        {
            return mAppearanceDB.Load(appearanceId);
        }

        public void RegisterForAppearanceUpdates(object listener, DB.DBUpdateCB<int> cb)
        {
            mAppearanceDB.RegisterUpdateListener(listener, cb);
        }

        public void UpdateAppearanceDB()
        {
            mAppearanceDB.Save(FileUtil.FileName.AppearanceDB.ToString());
        }

        public void UnRegisterForAppearanceUpdates(object listener)
        {
            mAppearanceDB.UnRegisterUpdateListener(listener);
        }

        public void WriteAppearance(int appearanceId, DB.DBAppearance appearance)
        {
            mAppearanceDB.Write(appearanceId, appearance);
        }
        // Appearance - End

        // Character
        public List<int> GetAllCharacterIds()
        {
            return mCharacterDB.Keys;
        }

        public DBCharacter LoadCharacter(int characterId)
        {
            DBCharacter toLoad = mCharacterDB.Load(characterId);

            return toLoad;
        }

        public void RegisterForCharacterUpdates(object listener, DB.DBUpdateCB<int> cb)
        {
            mCharacterDB.RegisterUpdateListener(listener, cb);
        }

        public void RemoveCharacter(int characterId)
        {
            mCharacterDB.Clear(characterId);
        }

        public void UnRegisterForCharacterUpdates(object listener)
        {
            mCharacterDB.UnRegisterUpdateListener(listener);
        }

        public DBCharacter WriteCharacter(DBCharacter character, TeamSide teamSide = TeamSide.INVALID)
        {
            Logger.Assert(character.Id != -1, LogTag.DB, TAG, "::WriteCharacter() - Attempting to write character with invalid Id", LogLevel.Error);
            if (character.Id == -1)
            {
                WriteNewCharacter(character, teamSide);
            }
            else
            {
                mCharacterDB.Write(character.Id, character);

                if (teamSide != TeamSide.INVALID)
                {
                    AddToTeam(character.Id, teamSide);
                }
            }

            return character;
        }

        public DBCharacter WriteNewCharacter(DBCharacter character, TeamSide teamSide = TeamSide.INVALID)
        {
            character.Id = sNewCharacterId++;

            WriteCharacter(character, teamSide);

            return character;
        }
        // Character - End

        // Conversation
        public DBConversation LoadConversation(int conversationId)
        {
            return mConversationDB.Load(conversationId);
        }

        public void UpdateConversationDB()
        {
            mConversationDB.Save(FileUtil.FolderName.DB.ToString());
        }

        public void WriteConversation(int conversationId, DBConversation conversation)
        {
            mConversationDB.Write(conversationId, conversation);
        }
        // Conversation - End

        // Equipment
        public DB.DBEquipment LoadEquipment(int equipmentId)
        {
            return mEquipmentDB.Load(equipmentId);
        }

        public void UpdateEquipmentDB()
        {
            mEquipmentDB.Save(FileUtil.FolderName.DB.ToString());
        }

        public void WriteEquipment(int equipmentId, DB.DBEquipment equipment)
        {
            mEquipmentDB.Write(equipmentId, equipment);
        }
        // Equipment - End

        // Item
        public DB.DBItem LoadItem(int itemId)
        {
            return mItemDB.Load(itemId);
        }

        public void UpdateItemDB()
        {
            mItemDB.Save(FileUtil.FileName.ItemDB.ToString());
        }

        public void WriteItem(int itemId, DB.DBItem item)
        {
            mItemDB.Write(itemId, item);
        }
        // Item - End

        // Props
        public DBPropInfo LoadPropInfo(int propId)
        {
            return mPropDB.Load(propId);
        }

        public void RegisterForPropUpdates(object listener, DB.DBUpdateCB<int> cb)
        {
            mPropDB.RegisterUpdateListener(listener, cb);
        }

        public void UpdatePropDB()
        {
            mPropDB.Save(FileUtil.FolderName.DB.ToString());
        }

        public void UnRegisterForPropUpdates(object listener)
        {
            mPropDB.UnRegisterUpdateListener(listener);
        }

        public void WritePropInfo(int propId, DBPropInfo propInfo)
        {
            mPropDB.Write(propId, propInfo);
        }
        // Props - End

        // Scenario
        public DB.DBScenarioInfo LoadScenarioInfo(int scenarioId)
        {
            return mScenarioDB.Load(scenarioId);
        }

        public void UpdateScenarioDB()
        {
            mScenarioDB.Save(FileUtil.FileName.ScenarioDB.ToString());
        }

        public void WriteScenarioInfo(int scenarioId, DB.DBScenarioInfo scenarioInfo)
        {
            mScenarioDB.Write(scenarioId, scenarioInfo);
        }
        // Scenario - End

        // Specialization
        public DBSpecialization LoadSpecialization(MAGE.GameSystems.Characters.SpecializationType specializationType)
        {
            return mSpecializationDB.Load((int)specializationType);
        }

        public void UpdateSpecializationDB()
        {
            mSpecializationDB.Save(FileUtil.FolderName.DB.ToString());
        }

        public void WriteSpecialization(DBSpecialization dBSpecialization)
        {
            mSpecializationDB.Write(dBSpecialization.SpecializationType, dBSpecialization);
        }
        // Specialization - End

        // Story
        public DBStoryArcInfo LoadStoryArcInfo(int storyArcId)
        {
            return mStoryDB.Load(storyArcId);
        }

        public void UpdateStoryDB()
        {
            mStoryDB.Save(FileUtil.FolderName.DB.ToString());
        }

        public void WriteStoryArcInfo(int storyArcId, DBStoryArcInfo storyArcInfo)
        {
            mStoryDB.Write(storyArcId, storyArcInfo);
        }
        // Story - End

        // Team
        public void AddToTeam(int characterId, TeamSide teamSide)
        {
            if (mTeamDB.ContainsEntry(teamSide))
            {
                DBTeam team = mTeamDB.Load(teamSide);
                Logger.Assert(!team.CharacterIds.Contains(characterId), LogTag.DB, TAG, string.Format("::AddToTeam() - Team {0} already contains character {1}", teamSide.ToString(), characterId), LogLevel.Warning);
                if (!team.CharacterIds.Contains(characterId))
                {
                    team.CharacterIds.Add(characterId);
                    mTeamDB.Write(teamSide, team);
                }
            }
            else
            {
                DBTeam team = mTeamDB.EmptyEntry();
                team.CharacterIds.Add(characterId);
                mTeamDB.Write(teamSide, team);
            }
        }

        public List<int> LoadTeam(TeamSide teamSide)
        {
            List<int> team = new List<int>();

            if (mTeamDB.ContainsEntry(teamSide))
            {
                team = mTeamDB.Load(teamSide).CharacterIds;
            }

            return team;
        }

        public void ClearTeam(TeamSide teamSide)
        {
            if (mTeamDB.ContainsEntry(teamSide))
            {
                mTeamDB.Clear(teamSide);
            }
        }
        // Team - End

        // Save Load
        public void Save(string path)
        {
            mCharacterDB.Save(path);
        }

        public void Load(string path)
        {
            mCharacterDB.Load(path);
        }
        // Save Load - End
    }
}
