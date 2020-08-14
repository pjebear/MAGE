using MAGE.GameServices;
using MAGE.GameServices.Character;
using MAGE.GameServices.Character.Internal;
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
                CharacterCreateParams characterCreateParams = new CharacterCreateParams(
                    StoryCharacterId.Rheinhardt
                    , PortraitSpriteId.Rheinhardt
                    , SpecializationType.Paladin
                    , new List<EquippableId>() { EquippableId.ChainArmor_0, EquippableId.Shield_0, EquippableId.Mace_0, EquippableId.Relic });

                int characterId = characterService.CreateCharacter(characterCreateParams);
                characterService.Debug_MaxOutTalents(characterId);
            }

            { // Asmund
                CharacterCreateParams characterCreateParams = new CharacterCreateParams(
                    StoryCharacterId.Asmund
                    , PortraitSpriteId.Asmund
                    , SpecializationType.Monk
                    , new List<EquippableId>() { EquippableId.ClothArmor_0, EquippableId.INVALID, EquippableId.Staff_0, EquippableId.INVALID });

                characterService.CreateCharacter(characterCreateParams);
            }

            { // Lothar
                CharacterCreateParams characterCreateParams = new CharacterCreateParams(
                    StoryCharacterId.Lothar
                    , PortraitSpriteId.Lothar
                    , SpecializationType.Archer
                    , new List<EquippableId>() { EquippableId.LeatherArmor_0, EquippableId.INVALID, EquippableId.Bow_0, EquippableId.INVALID });

                characterService.CreateCharacter(characterCreateParams);
            }

            { // Maric
                CharacterCreateParams characterCreateParams = new CharacterCreateParams(
                    StoryCharacterId.Maric
                    , PortraitSpriteId.Maric
                    , SpecializationType.Footman
                    , new List<EquippableId>() { EquippableId.ChainArmor_0, EquippableId.Shield_0, EquippableId.Sword_0, EquippableId.INVALID });

                characterService.CreateCharacter(characterCreateParams);
            }

            // Party Characters
            { // Francious
                CharacterCreateParams characterCreateParams = new CharacterCreateParams(
                    "Francious"
                    , SpecializationType.Archer
                    , new List<EquippableId>() { EquippableId.LeatherArmor_0, EquippableId.INVALID, EquippableId.Bow_0, EquippableId.INVALID });

                characterService.CreateCharacter(characterCreateParams);
            }

            // Scenario Characters
            int scenarioCharacterId = 0;
            { // Bandit Leader
                CharacterCreateParams characterCreateParams = new CharacterCreateParams(
                    CharacterUtil.ScenarioIdToDBId(ScenarioId.TheGreatHoldUp, scenarioCharacterId++)
                    , "BanditLeader"
                    , SpecializationType.Footman
                    , new List<EquippableId>() { EquippableId.ChainArmor_0, EquippableId.Shield_0, EquippableId.Sword_0, EquippableId.INVALID });

                characterService.CreateCharacter(characterCreateParams);
            }

            { // Lacky 1
                CharacterCreateParams characterCreateParams = new CharacterCreateParams(
                    CharacterUtil.ScenarioIdToDBId(ScenarioId.TheGreatHoldUp, scenarioCharacterId++)
                    , "Bandit"
                    , SpecializationType.Footman
                    , new List<EquippableId>() { EquippableId.LeatherArmor_0, EquippableId.INVALID, EquippableId.Sword_0, EquippableId.INVALID });

                characterService.CreateCharacter(characterCreateParams);
            }

            { // Lacky 2
                CharacterCreateParams characterCreateParams = new CharacterCreateParams(
                    CharacterUtil.ScenarioIdToDBId(ScenarioId.TheGreatHoldUp, scenarioCharacterId++)
                    , "Bandit"
                    , SpecializationType.Footman
                    , new List<EquippableId>() { EquippableId.LeatherArmor_0, EquippableId.INVALID, EquippableId.Sword_0, EquippableId.INVALID });

                characterService.CreateCharacter(characterCreateParams);
            }
        }
    }
}


