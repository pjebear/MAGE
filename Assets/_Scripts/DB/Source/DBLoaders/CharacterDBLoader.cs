using MAGE.GameSystems;
using MAGE.GameSystems.Appearances;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Characters.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.DB.Internal
{
    class CharacterDBLoader
    {
        public static void LoadDB()
        {
            ICharacterService characterService = CharacterService.Get();
            // Story Characters

            { // Rheinhardt
                CharacterCreateParams createParams = new CharacterCreateParams();

                createParams.characterType = CharacterType.Story;
                createParams.characterClass = CharacterClass.MonoSpecialization;
                createParams.id = (int)StoryCharacterId.Rheinhardt;
                createParams.name = StoryCharacterId.Rheinhardt.ToString();
                createParams.currentSpecialization = SpecializationType.Paladin;
                createParams.currentEquipment = new List<EquippableId>() { EquippableId.ChainArmor_0, EquippableId.Shield_0, EquippableId.Mace_0, EquippableId.Relic };
                createParams.appearanceOverrides = new Appearance()
                {
                    PortraitSpriteId = PortraitSpriteId.Rheinhardt,

                    BodyType = BodyType.HumanoidMale,
                    FacialHairType = FacialHairType.ShortBeard,
                    HairColor = HairColor.Brunette,
                    HairType = HairType.MaleShort,
                    SkinToneType = SkinToneType.Base,
                };

                int characterId = characterService.CreateCharacter(createParams);
                characterService.Debug_MaxOutTalents(characterId);
            }

            { // Asmund
                CharacterCreateParams createParams = new CharacterCreateParams();

                createParams.characterType = CharacterType.Story;
                createParams.characterClass = CharacterClass.MultiSpecialization;
                createParams.id = (int)StoryCharacterId.Asmund;
                createParams.name = StoryCharacterId.Asmund.ToString();
                createParams.currentSpecialization = SpecializationType.Monk;
                createParams.currentEquipment = new List<EquippableId>() { EquippableId.ClothArmor_0, EquippableId.INVALID, EquippableId.Staff_0, EquippableId.INVALID };
                createParams.appearanceOverrides = new Appearance()
                {
                    PortraitSpriteId = PortraitSpriteId.Asmund,

                    BodyType = BodyType.HumanoidMale,
                    FacialHairType = FacialHairType.LongBeard,
                    HairColor = HairColor.Grey,
                    HairType = HairType.MaleLong,
                    SkinToneType = SkinToneType.Pale,
                };

                characterService.CreateCharacter(createParams);
            }

            { // Lothar
                CharacterCreateParams createParams = new CharacterCreateParams();

                createParams.characterType = CharacterType.Story;
                createParams.characterClass = CharacterClass.MultiSpecialization;
                createParams.id = (int)StoryCharacterId.Lothar;
                createParams.name = StoryCharacterId.Lothar.ToString();
                createParams.currentSpecialization = SpecializationType.Archer;
                createParams.currentEquipment = new List<EquippableId>() { EquippableId.LeatherArmor_0, EquippableId.INVALID, EquippableId.Bow_0, EquippableId.INVALID };
                createParams.appearanceOverrides = new Appearance()
                {
                    PortraitSpriteId = PortraitSpriteId.Lothar,

                    BodyType = BodyType.HumanoidMale,
                    FacialHairType = FacialHairType.None,
                    HairColor = HairColor.Dark,
                    HairType = HairType.MaleLong,
                    SkinToneType = SkinToneType.Tan,
                };

                characterService.CreateCharacter(createParams);
            }

            { // Balgrid
                CharacterCreateParams createParams = new CharacterCreateParams();

                createParams.characterType = CharacterType.Story;
                createParams.characterClass = CharacterClass.MultiSpecialization;
                createParams.id = (int)StoryCharacterId.Balgrid;
                createParams.name = StoryCharacterId.Balgrid.ToString();
                createParams.currentSpecialization = SpecializationType.Footman;
                createParams.currentEquipment = new List<EquippableId>() { EquippableId.LeatherArmor_0, EquippableId.Shield_0, EquippableId.Sword_0, EquippableId.INVALID };
                createParams.appearanceOverrides = new Appearance()
                {
                    PortraitSpriteId = PortraitSpriteId.Balgrid,

                    BodyType = BodyType.HumanoidMale,
                    FacialHairType = FacialHairType.None,
                    HairColor = HairColor.Brunette,
                    HairType = HairType.MaleShort,
                    SkinToneType = SkinToneType.Base,
                };

                characterService.CreateCharacter(createParams);
            }

            { // Maric
                CharacterCreateParams createParams = new CharacterCreateParams();

                createParams.characterType = CharacterType.Story;
                createParams.characterClass = CharacterClass.MultiSpecialization;
                createParams.id = (int)StoryCharacterId.Maric;
                createParams.name = StoryCharacterId.Maric.ToString();
                createParams.currentSpecialization = SpecializationType.Footman;
                createParams.currentEquipment = new List<EquippableId>() { EquippableId.ChainArmor_0, EquippableId.Shield_0, EquippableId.Sword_0, EquippableId.INVALID };
                createParams.appearanceOverrides = new Appearance()
                {
                    PortraitSpriteId = PortraitSpriteId.Maric,

                    BodyType = BodyType.HumanoidMale,
                    FacialHairType = FacialHairType.None,
                    HairColor = HairColor.Dark,
                    HairType = HairType.MaleBuzz,
                    SkinToneType = SkinToneType.Pale,
                };
                characterService.CreateCharacter(createParams);
            }

            // Party Characters
            { // Francious
                CharacterCreateParams createParams = new CharacterCreateParams();

                createParams.characterType = CharacterType.Create;
                createParams.characterClass = CharacterClass.MultiSpecialization;
                createParams.name = "Francious";
                createParams.currentSpecialization = SpecializationType.Archer;
                createParams.currentEquipment = new List<EquippableId>() { EquippableId.LeatherArmor_0, EquippableId.INVALID, EquippableId.Bow_0, EquippableId.INVALID };

                characterService.CreateCharacter(createParams);
            }

            // Scenario Characters
            int scenarioCharacterId = 0;
            { // Bandit Leader
                CharacterCreateParams createParams = new CharacterCreateParams();

                createParams.characterType = CharacterType.Scenario;
                createParams.characterClass = CharacterClass.MultiSpecialization;
                createParams.id = CharacterUtil.ScenarioIdToDBId(ScenarioId.TheGreatHoldUp, scenarioCharacterId++);
                createParams.name = "BanditLeader";
                createParams.currentSpecialization = SpecializationType.Footman;
                createParams.currentEquipment = new List<EquippableId>() { EquippableId.ChainArmor_0, EquippableId.Shield_0, EquippableId.Sword_0, EquippableId.INVALID };
                createParams.appearanceOverrides = new Appearance() { PortraitSpriteId = PortraitSpriteId.BanditLeader };

                characterService.CreateCharacter(createParams);
            }

            { // Lacky 1
                CharacterCreateParams createParams = new CharacterCreateParams();

                createParams.characterType = CharacterType.Scenario;
                createParams.characterClass = CharacterClass.MultiSpecialization;
                createParams.id = CharacterUtil.ScenarioIdToDBId(ScenarioId.TheGreatHoldUp, scenarioCharacterId++);
                createParams.name = "Bandit";
                createParams.currentSpecialization = SpecializationType.Footman;
                createParams.currentEquipment = new List<EquippableId>() { EquippableId.LeatherArmor_0, EquippableId.Shield_0, EquippableId.Sword_0, EquippableId.INVALID };
                createParams.appearanceOverrides = new Appearance() { PortraitSpriteId = PortraitSpriteId.Bandit_0 };

                characterService.CreateCharacter(createParams);
            }

            { // Lacky 2
                CharacterCreateParams createParams = new CharacterCreateParams();

                createParams.characterType = CharacterType.Scenario;
                createParams.characterClass = CharacterClass.MultiSpecialization;
                createParams.id = CharacterUtil.ScenarioIdToDBId(ScenarioId.TheGreatHoldUp, scenarioCharacterId++);
                createParams.name = "Bandit";
                createParams.currentSpecialization = SpecializationType.Footman;
                createParams.currentEquipment = new List<EquippableId>() { EquippableId.LeatherArmor_0, EquippableId.Shield_0, EquippableId.Sword_0, EquippableId.INVALID };
                createParams.appearanceOverrides = new Appearance() { PortraitSpriteId = PortraitSpriteId.Bandit_0 };

                characterService.CreateCharacter(createParams);
            }
        }
    }
}


