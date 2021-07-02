using MAGE.GameSystems.Appearances;
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
                foreach (SpecializationType specializationType in SpecializationConstants.MULTI_SPECIALIZATIONS)
                {
                    specializationTypes.Add(specializationType);
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
                case SpecializationType.Adept:      portraitSpriteId = PortraitSpriteId.Adept; break;
                case SpecializationType.Archer:     portraitSpriteId = PortraitSpriteId.Archer; break;
                case SpecializationType.Assassin:   portraitSpriteId = PortraitSpriteId.Assassin; break;
                case SpecializationType.Bear:       portraitSpriteId = PortraitSpriteId.Bear; break;
                case SpecializationType.Footman:    portraitSpriteId = PortraitSpriteId.Footman; break;
                case SpecializationType.Monk:       portraitSpriteId = PortraitSpriteId.Monk; break;
                default:
                    break;
            }

            return portraitSpriteId;
        }

        public static SpecializationRole GetRoleForSpecialization(SpecializationType specializationType)
        {
            SpecializationRole role = SpecializationRole.Tank;

            switch (specializationType)
            {
                case SpecializationType.Adept:              role = SpecializationRole.Range; break;
                case SpecializationType.Archer:             role = SpecializationRole.Range; break;
                case SpecializationType.Bear:               role = SpecializationRole.Tank; break;
                case SpecializationType.Footman:            role = SpecializationRole.Tank; break;
                case SpecializationType.Monk:               role = SpecializationRole.Support; break;
                default:
                    break;
            }

            return role;
        }
    }
}
