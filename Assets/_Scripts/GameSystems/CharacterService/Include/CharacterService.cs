using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameServices
{
    interface ICharacterService : Services.IService
    {
        void AssignExperience(int characterId, int experience);
        void AssignTalentPoint(int characterId, Character.TalentId talentId);
        List<int> ChangeSpecialization(int characterId, Character.SpecializationType specializationType);
        int CreateCharacter(Character.CharacterCreateParams createParams);
        List<int> EquipCharacter(int characterId, EquippableId equippableId, Character.Equipment.Slot inSlot);
        int GetCharacterAppearanceId(int characterId);
        Character.CharacterInfo GetCharacterInfo(int characterId);
        List<int> GetCharactersOfType(Character.CharacterType characterType);
        List<int> ResetTalentPoints(int characterId);
        void RegisterForCharacterChanges(int characterId, Character.ICharacterUpdateListener characterUpdateListener);
        List<int> UnEquipCharacter(int characterId, Character.Equipment.Slot inSlot);
        void UnRegisterForCharacterChanges(int characterId, Character.ICharacterUpdateListener characterUpdateListener);
        // Debug 
        void Debug_MaxOutTalents(int characterId);
    }

    abstract class CharacterService : Services.ServiceBase<ICharacterService> { };
}
