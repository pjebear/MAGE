using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    static class DBHelper
    {
        static string TAG = "DBHelper";

        static ActionDB ActionDB = new ActionDB();
        static AppearnaceDB AppearanceDB = new AppearnaceDB();
        static CharacterDB CharacterDB = new CharacterDB();
        static ConversationDB ConversationDB = new ConversationDB();
        static SpecializationDB SpecializationDB = new SpecializationDB();
        static EquipmentDB EquipmentDB = new EquipmentDB();
        static TeamDB TeamDB = new TeamDB();

        static int sNewCharacterId;
        #region ActionDB
        public static void WriteAction(int actionId, DB.DBAction action)
        {
            ActionDB.Write(actionId, action);
        }

        public static DB.DBAction LoadAction(int actionId)
        {
            return ActionDB.Load(actionId);
        }
        #endregion

        #region CharacterDB
        public static DBCharacter WriteNewCharacter(DBCharacter character, TeamSide teamSide = TeamSide.INVALID)
        {
            character.Id = sNewCharacterId++;

            WriteCharacter(character, teamSide);

            return character;
        }

        public static DBCharacter WriteCharacter(DBCharacter character, TeamSide teamSide = TeamSide.INVALID)
        {
            Logger.Assert(character.Id != -1, LogTag.DB, TAG, "::WriteCharacter() - Attempting to write character with invalid Id", LogLevel.Error);
            if (character.Id == -1)
            {
                WriteNewCharacter(character, teamSide);
            }
            else
            {
                CharacterDB.Write(character.Id, character);

                if (teamSide != TeamSide.INVALID)
                {
                    AddToTeam(character.Id, teamSide);
                }
            }

            return character;
        }

        public static DBCharacter LoadCharacter(int characterId)
        {
            DBCharacter toLoad = CharacterDB.Load(characterId);

            return toLoad;
        }

        public static void RemoveCharacter(int characterId)
        {
            CharacterDB.Clear(characterId);
        }

        public static void AddToTeam(int characterId, TeamSide teamSide)
        {
            if (TeamDB.ContainsEntry(teamSide))
            {
                DBTeam team = TeamDB.Load(teamSide);
                Logger.Assert(!team.CharacterIds.Contains(characterId), LogTag.DB, TAG, string.Format("::AddToTeam() - Team {0} already contains character {1}", teamSide.ToString(), characterId), LogLevel.Warning);
                if (!team.CharacterIds.Contains(characterId))
                {
                    team.CharacterIds.Add(characterId);
                    TeamDB.Write(teamSide, team);
                }
            }
            else
            {
                DBTeam team = TeamDB.EmptyEntry();
                team.CharacterIds.Add(characterId);
                TeamDB.Write(teamSide, team);
            }
        }

        public static List<DBCharacter> LoadCharactersOnTeam(TeamSide teamSide)
        {
            List<DBCharacter> dBCharacters = new List<DBCharacter>();

            if (TeamDB.ContainsEntry(teamSide))
            {
                DBTeam team = TeamDB.Load(teamSide);
                foreach (int characterId in team.CharacterIds)
                {
                    dBCharacters.Add(LoadCharacter(characterId));
                }
            }

            return dBCharacters;
        }

        public static List<int> LoadTeam(TeamSide teamSide)
        {
            List<int> team = new List<int>();

            if (TeamDB.ContainsEntry(teamSide))
            {
                team = TeamDB.Load(teamSide).CharacterIds;
            }

            return team;
        }

        public static void ClearTeam(TeamSide teamSide)
        {
            if (TeamDB.ContainsEntry(teamSide))
            {
                TeamDB.Clear(teamSide);
            }
        }

        public static List<int> GetAllCharacterIds()
        {
            return CharacterDB.Keys;
        }
#endregion //CharacterDB

        #region EquipmentDB
        public static void WriteEquipment(int equipmentId, DB.DBEquipment equipment)
        {
            EquipmentDB.Write(equipmentId, equipment);
        }

        public static DB.DBEquipment LoadEquipment(int equipmentId)
        {
            return EquipmentDB.Load(equipmentId);
        }

        public static void UpdateEquipmentDB()
        {
            EquipmentDB.Save(FileUtil.FolderName.DB.ToString());
        }
        #endregion //EquipmentDB

        #region ConversationDB
        public static void WriteConversation(int conversationId, DBConversation conversation)
        {
            ConversationDB.Write(conversationId, conversation);
        }

        public static DBConversation LoadConversation(int conversationId)
        {
            return ConversationDB.Load(conversationId);
        }

        public static void UpdateConversationDB()
        {
            ConversationDB.Save(FileUtil.FolderName.DB.ToString());
        }
        #endregion //ConversationDB

        #region SpecializationDB
        public static void WriteSpecialization(DBSpecialization dBSpecialization)
        {
            SpecializationDB.Write(dBSpecialization.SpecializationType, dBSpecialization);
        }

        public static DBSpecialization LoadSpecialization(SpecializationType specializationType)
        {
            return SpecializationDB.Load((int)specializationType);
        }

        public static void UpdateSpecializationDB()
        {
            SpecializationDB.Save(FileUtil.FolderName.DB.ToString());
        }
        #endregion SpecializationDB

        public static void Save(string path)
        {
            CharacterDB.Save(path);
        }

        public static void Load(string path)
        {
            CharacterDB.Load(path);
        }
    }
}
