using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

static class CharacterConstants
{
    public const int LEVEL_UP_THRESHOLD = 100;

    public const int CHARACTER_ID_RANGE = 10000;
    public const int TEMPORARY_CHARACTER_ID_OFFSET = CHARACTER_ID_RANGE * (int)CharacterType.Temporary;
    public const int CREATE_CHARACTER_ID_OFFSET = CHARACTER_ID_RANGE * (int)CharacterType.Create;
    public const int STORY_CHARACTER_ID_OFFSET = CHARACTER_ID_RANGE * (int)CharacterType.Story;
}

enum StoryCharacterId
{
    Rheinhardt = CharacterConstants.STORY_CHARACTER_ID_OFFSET,
    Asmund,

    Maric,

    NUM
}

enum CharacterType
{
    Temporary, 
    Create,
    Story,

    NUM
}
