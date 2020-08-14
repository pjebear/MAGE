using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameServices.Character
{
    static class CharacterConstants
    {
        public const int LEVEL_UP_THRESHOLD = 100;

        public const int INVALID_ID = -1;
        public const int CHARACTER_ID_RANGE = 10000;
        public const int SUB_CATEGORY_RANGE = 100;
        public const int TEMPORARY_CHARACTER_ID_OFFSET =    Common.GameConstants.CHARACTER_ID_OFFSET + CHARACTER_ID_RANGE * (int)CharacterType.Temporary;
        public const int CREATE_CHARACTER_ID_OFFSET =       Common.GameConstants.CHARACTER_ID_OFFSET + CHARACTER_ID_RANGE * (int)CharacterType.Create;
        public const int STORY_CHARACTER_ID_OFFSET =        Common.GameConstants.CHARACTER_ID_OFFSET + CHARACTER_ID_RANGE * (int)CharacterType.Story;
        public const int SCENARIO_CHARACTER_ID_OFFSET =     Common.GameConstants.CHARACTER_ID_OFFSET + CHARACTER_ID_RANGE * (int)CharacterType.Scenario;   
    }

    interface ICharacterUpdateListener
    {
        void OnCharacterUpdated(int characterId);
    }

    enum StoryCharacterId
    {
        INVALID = -1,

        Rheinhardt = CharacterConstants.STORY_CHARACTER_ID_OFFSET,
        Asmund,
        Lothar,

        Maric,

        NUM
    }

    enum CharacterType
    {
        INVALID = -1,

        Temporary,
        Create,
        Story,
        Scenario,

        NUM
    }

    class CharacterCreateParams
    {
        public CharacterType characterType = CharacterType.NUM;
        public int id = -1;
        public string name = "EMPTY";
        public PortraitSpriteId portraitSpriteId = PortraitSpriteId.INVALID;
        public SpecializationType currentSpecialization = SpecializationType.NONE;
        public List<EquippableId> currentEquipment = new List<EquippableId>();

        public CharacterCreateParams(StoryCharacterId storyCharacterId, PortraitSpriteId portraitSpriteId, SpecializationType specializationType, List<EquippableId> equippableIds)
            : this(CharacterType.Story, (int)storyCharacterId, storyCharacterId.ToString(), portraitSpriteId, specializationType, equippableIds)
        {
            // empty
        }

        public CharacterCreateParams(string name, SpecializationType specializationType, List<EquippableId> equippableIds)
            : this(CharacterType.Create, -1, name, PortraitSpriteId.INVALID, specializationType, equippableIds)
        {
            // empty
        }

        public CharacterCreateParams(int scenarioId, string name, SpecializationType specializationType, List<EquippableId> equippableIds)
            : this(CharacterType.Scenario, scenarioId, name, PortraitSpriteId.INVALID, specializationType, equippableIds)
        {
            // empty
        }

        public CharacterCreateParams(CharacterType characterType, int id, string name, PortraitSpriteId portraitSpriteId, SpecializationType currentSpecialization, List<EquippableId> currentEquipment)
        {
            this.characterType = characterType;
            this.id = id;
            this.name = name;
            this.portraitSpriteId = portraitSpriteId;
            this.currentSpecialization = currentSpecialization;
            this.currentEquipment = currentEquipment;
        }
    }

}


