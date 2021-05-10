using MAGE.GameSystems.Appearances;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Characters
{
    static class CharacterConstants
    {
        public const int DEFAULT_MOVES_A_TURN = 1;
        public const int DEFAULT_ACTIONS_A_TURN = 1;
        public const int CLOCK_GUAGE_THRESHOLD = 100;
        public const int LEVEL_UP_THRESHOLD = 100;

        public const int INVALID_ID = -1;
        public const int CHARACTER_ID_RANGE = 10000;
        public const int SUB_CATEGORY_RANGE = 100;
        public const int TEMPORARY_CHARACTER_ID_OFFSET =    Common.GameConstants.CHARACTER_ID_OFFSET + CHARACTER_ID_RANGE * (int)CharacterType.Temporary;
        public const int CREATE_CHARACTER_ID_OFFSET =       Common.GameConstants.CHARACTER_ID_OFFSET + CHARACTER_ID_RANGE * (int)CharacterType.Create;
        public const int STORY_CHARACTER_ID_OFFSET =        Common.GameConstants.CHARACTER_ID_OFFSET + CHARACTER_ID_RANGE * (int)CharacterType.Story;
        public const int SCENARIO_CHARACTER_ID_OFFSET =     Common.GameConstants.CHARACTER_ID_OFFSET + CHARACTER_ID_RANGE * (int)CharacterType.Scenario;   
    }

    public enum StoryCharacterId
    {
        INVALID = -1,

        Rheinhardt = CharacterConstants.STORY_CHARACTER_ID_OFFSET,
        Asmund,
        Balgrid,
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

    enum CharacterClass
    {
        INVALID = -1,

        MultiSpecialization,
        MonoSpecialization,

        NUM
    }

    class CharacterCreateParams
    {
        public static List<EquippableId> EMPTY_EQUIPMENT
        {
            get
            {
                return new List<EquippableId>() { EquippableId.INVALID, EquippableId.INVALID, EquippableId.INVALID, EquippableId.INVALID };
            }
        }
    
        public CharacterType characterType = CharacterType.NUM;
        public CharacterClass characterClass = CharacterClass.MonoSpecialization;
        public int id = -1;
        public string name = "EMPTY";
        public Appearance appearanceOverrides = new Appearance();
        public SpecializationType currentSpecialization = SpecializationType.INVALID;
        public List<EquippableId> currentEquipment = EMPTY_EQUIPMENT;
    }

    struct CharacterGrowthInfo
    {
        public int CharacterId;
        public int Xp;
        public int CharacterLevel;

        public int SpecializationXp;
        public int SpecializationLvl;

        public List<AttributeModifier> AttributeModifiers;
    }
}


