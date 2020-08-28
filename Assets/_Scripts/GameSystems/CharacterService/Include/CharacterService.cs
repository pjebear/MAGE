using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems
{
    interface ICharacterService : Services.IService
    {
        void AssignExperience(int characterId, int experience);
        void AssignTalentPoint(int characterId, Characters.TalentId talentId);
        List<int> ChangeSpecialization(int characterId, Characters.SpecializationType specializationType);
        int CreateCharacter(Characters.CharacterCreateParams createParams);
        List<int> EquipCharacter(int characterId, EquippableId equippableId, Characters.Equipment.Slot inSlot);
        Characters.Character GetCharacter(int characterId);
        List<int> GetCharactersOfType(Characters.CharacterType characterType);
        List<int> ResetTalentPoints(int characterId);
        void RegisterForCharacterChanges(int characterId, Characters.ICharacterUpdateListener characterUpdateListener);
        List<int> UnEquipCharacter(int characterId, Characters.Equipment.Slot inSlot);
        void UnRegisterForCharacterChanges(int characterId, Characters.ICharacterUpdateListener characterUpdateListener);
        // Debug 
        void Debug_MaxOutTalents(int characterId);
    }

    abstract class CharacterService : Services.ServiceBase<ICharacterService> { };
}
