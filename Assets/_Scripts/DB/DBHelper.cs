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

        static CharacterDB CharacterDB = new CharacterDB();
        static SpecializationDB SpecializationDB = new SpecializationDB();
        static TeamDB TeamDB = new TeamDB();

        static int sNewCharacterId;

        //! Character DB
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

        public static void ClearTeam(TeamSide teamSide, bool removePlayers)
        {
            if (TeamDB.ContainsEntry(teamSide))
            {
                if (removePlayers)
                {
                    DBTeam team = TeamDB.Load(teamSide);
                    foreach (int characterId in team.CharacterIds)
                    {
                        RemoveCharacter(characterId);
                    }
                }

                TeamDB.Clear(teamSide);
            }
        }

        public static List<int> GetAllCharacterIds()
        {
            return CharacterDB.Keys;
        }

        //! Character DB End

        //! SpecializationDB
        public static void WriteSpecialization(DBSpecialization dBSpecialization)
        {
            SpecializationDB.Write(dBSpecialization.SpecializationType, dBSpecialization);
        }

        public static DBSpecialization LoadSpecialization(SpecializationType specializationType)
        {
            return SpecializationDB.Load((int)specializationType);
        }

        public static void Save(string path)
        {
            CharacterDB.Save(path);
        }

        public static void UpdateSpecializationDB()
        {
            SpecializationDB.Save(FileUtil.FolderName.DB.ToString());
        }

        public static void Load(string path)
        {
            CharacterDB.Load(path);
        }
    }
}
