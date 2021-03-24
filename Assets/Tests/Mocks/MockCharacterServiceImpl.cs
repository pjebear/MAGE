using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAGE.GameSystems.Appearances;
using MAGE.GameSystems.Items;
using MAGE.Services;

namespace MAGE.GameSystems.Characters.Internal
{
    class MockCharacterServiceImpl : ICharacterService
    {
        public Dictionary<int, Character> mMockCharacters = new Dictionary<int, Character>();

        public void Init()
        {
            // Empty
        }

        public void Takedown()
        {
            // Empty
        }

        // DB Updates
        public void OnCharacterDBUpdated(int characterId)
        {
            // Empty
        }

        public void AssignExperience(int characterId, int experience)
        {
            // Empty
        }

        public void AssignTalentPoint(int characterId, TalentId talentId)
        {
            // Empty
        }

        public List<int> ChangeSpecialization(int characterId, SpecializationType specializationType)
        {
            return new List<int>();
        }

        public int CreateCharacter(CharacterCreateParams createParams)
        {
            return -1;
        }

        public int CreateMob(Mobs.MobId mobId, int level)
        {
            return -1;
        }

        public List<int> EquipCharacter(int characterId, EquippableId equippableId, Equipment.Slot inSlot)
        {
            return new List<int>();
        }

        public Character GetCharacter(int characterId)
        {
            Character character = null;
            if (mMockCharacters.ContainsKey(characterId))
            {
                character = mMockCharacters[characterId];
            }
            return character;
        }

        public void DeleteCharacter(int characterId)
        {
            // Empty
        }

        public List<int> GetCharactersOfType(CharacterType characterType)
        {
            return new List<int>();
        }

        public List<int> ResetTalentPoints(int characterId)
        {
            return new List<int>();
        }

        public void RegisterForCharacterChanges(int characterId, ICharacterUpdateListener characterUpdateListener)
        {
            // Empty
        }

        public List<int> UnEquipCharacter(int characterId, Equipment.Slot inSlot)
        {
            return new List<int>();
        }

        public void UnRegisterForCharacterChanges(int characterId, ICharacterUpdateListener characterUpdateListener)
        {
            // Empty
        }

        // Debug
        public void Debug_MaxOutTalents(int characterId)
        {
            // Empty
        }
    }
}
