using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameServices.Character
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

        public static int DBIdToScenarioId(int dbId)
        {
            return dbId % CharacterConstants.SUB_CATEGORY_RANGE;
        }

        public static CharacterType GetCharacterTypeFromId(int id)
        {
            CharacterType characterType = CharacterType.Temporary;

            if (id >= CharacterConstants.SCENARIO_CHARACTER_ID_OFFSET)
            {
                characterType = CharacterType.Scenario;
            }
            else if (id >= CharacterConstants.STORY_CHARACTER_ID_OFFSET)
            {
                characterType = CharacterType.Story;
            }
            else if (id >= CharacterConstants.CREATE_CHARACTER_ID_OFFSET)
            {
                characterType = CharacterType.Create;
            }
            else
            {
                characterType = CharacterType.Temporary;
            }

            return characterType;
        }
    }
}


