using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameSystems.Characters
{
    static class SpecializationUtil
    {
        public static List<SpecializationType> GetSpecializationsForCharacter(CharacterClass characterClass, SpecializationType currentSpecialization)
        {
            List<SpecializationType> specializationTypes = new List<SpecializationType>();
            if (characterClass == CharacterClass.MonoSpecialization)
            {
                specializationTypes.Add(currentSpecialization);
            }
            else if (characterClass == CharacterClass.MultiSpecialization)
            {
                for (int i = 0; i < (int)SpecializationType.MULTI_SEPCIALIZATION_NUM; ++i)
                {
                    specializationTypes.Add((SpecializationType)((int)SpecializationType.MULTI_SPECIALIZATION_FIRST + i));
                }
            }
            else
            {
                Debug.Assert(false);
            }

            return specializationTypes;
        }

        public static PortraitSpriteId GetPortraitSpriteIdForSpecialization(SpecializationType specializationType)
        {
            PortraitSpriteId portraitSpriteId = PortraitSpriteId.INVALID;

            switch (specializationType)
            {
                case SpecializationType.Archer:     portraitSpriteId = PortraitSpriteId.Archer; break;
                case SpecializationType.Bear:       portraitSpriteId = PortraitSpriteId.Bear; break;
                case SpecializationType.Footman:    portraitSpriteId = PortraitSpriteId.Footman; break;
                case SpecializationType.Monk:       portraitSpriteId = PortraitSpriteId.Monk; break;
                default:
                    break;
            }

            return portraitSpriteId;
        }
    }
}
