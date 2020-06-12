using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

static class CharacterConstants
{
    public const int LEVEL_UP_THRESHOLD = 100;

    public const int CHARACTER_ID_RANGE = 100000;
    public const int SUB_CATEGORY_RANGE = 100;
    public const int TEMPORARY_CHARACTER_ID_OFFSET = CHARACTER_ID_RANGE * (int)CharacterType.Temporary;
    public const int CREATE_CHARACTER_ID_OFFSET = CHARACTER_ID_RANGE * (int)CharacterType.Create;
    public const int STORY_CHARACTER_ID_OFFSET = CHARACTER_ID_RANGE * (int)CharacterType.Story;
    public const int SCENARIO_CHARACTER_ID_OFFSET = CHARACTER_ID_RANGE * (int)CharacterType.Scenario;
}

enum StoryCharacterId
{
    Rheinhardt = CharacterConstants.STORY_CHARACTER_ID_OFFSET,
    Asmund,
    Lothar,

    Maric,

    NUM
}

enum CharacterType
{
    Temporary, 
    Create,
    Story,
    Scenario,

    NUM
}
