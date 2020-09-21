using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameSystems.Characters
{
    static class CharacterUtil
    {
        private static string TAG = "CharacterUtil";

        public static int ScenarioIdToDBId(ScenarioId scenarioId, int characterId)
        {
            return
                CharacterConstants.SCENARIO_CHARACTER_ID_OFFSET
                + CharacterConstants.SUB_CATEGORY_RANGE * (int)scenarioId
                + characterId;
        }

        public static int CreateIdToDBId(int createCharacterId)
        {
            return
                CharacterConstants.CREATE_CHARACTER_ID_OFFSET
                + createCharacterId;
        }

        public static int DBIdToScenarioId(int dbId)
        {
            return dbId % CharacterConstants.SUB_CATEGORY_RANGE;
        }

        public static int GetIdOffsetFromType(CharacterType characterType)
        {
            int offset = 0;

            switch (characterType)
            {
                case CharacterType.Temporary: offset = CharacterConstants.TEMPORARY_CHARACTER_ID_OFFSET; break;
                case CharacterType.Create: offset = CharacterConstants.CREATE_CHARACTER_ID_OFFSET; break;
                case CharacterType.Story: offset = CharacterConstants.STORY_CHARACTER_ID_OFFSET; break;
                case CharacterType.Scenario: offset = CharacterConstants.SCENARIO_CHARACTER_ID_OFFSET; break;
                default:
                    Debug.Assert(false);
                    break;
            }

            return offset;
        }

        public static CharacterType GetCharacterTypeFromId(int id)
        {
            CharacterType characterType = CharacterType.Temporary;

            for (int i = (int)CharacterType.NUM - 1; i >= 0; --i)
            {
                CharacterType type = (CharacterType)i;
                if (id >= GetIdOffsetFromType(type))
                {
                    characterType = type;
                    break;
                }
            }

            return characterType;
        }
    }
}


