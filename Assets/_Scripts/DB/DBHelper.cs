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

        static CharacterInfoDB CharacterInfoDB = new CharacterInfoDB();
        static EquipmentInfoDB EquipmentDB= new EquipmentInfoDB();
        static SpecializationDB SpecializationDB = new SpecializationDB();
        static TeamDB TeamDB = new TeamDB();

        static int sNewCharacterId;

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
                CharacterInfoDB.Write(character.Id, character.CharacterInfo);
                EquipmentDB.Write(character.Id, character.EquipmentInfo);
                SpecializationDB.Write(character.Id, character.SpecializationsInfo);

                if (teamSide != TeamSide.INVALID)
                {
                    AddToTeam(character.Id, teamSide);
                }
            }

            return character;
        }

        public static DBCharacter LoadCharacter(int characterId)
        {
            DBCharacter toLoad = new DBCharacter();

            toLoad.Id = characterId;
            toLoad.CharacterInfo = CharacterInfoDB.Load(characterId);
            toLoad.EquipmentInfo = EquipmentDB.Load(characterId);
            toLoad.SpecializationsInfo = SpecializationDB.Load(characterId);

            return toLoad;
        }

        public static void RemoveCharacter(int characterId)
        {
            CharacterInfoDB.Clear(characterId);
            EquipmentDB.Clear(characterId);
            SpecializationDB.Clear(characterId);
        }

        public static void AddToTeam(int characterId, TeamSide teamSide)
        {
            if (TeamDB.ContainsEntry(teamSide))
            {
                Team team = TeamDB.Load(teamSide);
                Logger.Assert(!team.CharacterIds.Contains(characterId), LogTag.DB, TAG, string.Format("::AddToTeam() - Team {0} already contains character {1}", teamSide.ToString(), characterId), LogLevel.Warning);
                if (!team.CharacterIds.Contains(characterId))
                {
                    team.CharacterIds.Add(characterId);
                    TeamDB.Write(teamSide, team);
                }
            }
            else
            {
                Team team = TeamDB.EmptyEntry();
                team.CharacterIds.Add(characterId);
                TeamDB.Write(teamSide, team);
            }
        }

        public static List<DBCharacter> LoadCharactersOnTeam(TeamSide teamSide)
        {
            List<DBCharacter> dBCharacters = new List<DBCharacter>();

            if (TeamDB.ContainsEntry(teamSide))
            {
                Team team = TeamDB.Load(teamSide);
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
                    Team team = TeamDB.Load(teamSide);
                    foreach (int characterId in team.CharacterIds)
                    {
                        RemoveCharacter(characterId);
                    }
                }

                TeamDB.Clear(teamSide);
            }
        }
    }
}
