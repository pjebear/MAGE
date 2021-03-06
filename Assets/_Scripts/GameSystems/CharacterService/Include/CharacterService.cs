﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;

namespace MAGE.GameSystems
{
    interface ICharacterService : Services.IService
    {
        Characters.CharacterGrowthInfo AssignExperience(int characterId, int experience);
        void AssignTalentPoint(int characterId, Characters.TalentId talentId);
        List<int> ChangeSpecialization(int characterId, Characters.SpecializationType specializationType);
        int CreateCharacter(Characters.CharacterCreateParams createParams);
        int CreateMob(Mobs.MobId mobId, int level);
        void DeleteCharacter(int characterId);
        List<int> EquipCharacter(int characterId, EquippableId equippableId, Items.Equipment.Slot inSlot);
        Characters.Character GetCharacter(int characterId);
        List<int> GetCharactersOfType(Characters.CharacterType characterType);
        List<int> ResetTalentPoints(int characterId);
        List<int> UnEquipCharacter(int characterId, Items.Equipment.Slot inSlot);
        // Debug 
        void Debug_MaxOutTalents(int characterId);
    }

    abstract class CharacterService : Services.ServiceBase<ICharacterService> { };
}
