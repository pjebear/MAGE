using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameServices.Character
{
    static class SpecializationUtil
    {
        public static PortraitSpriteId GetPortraitSpriteIdForSpecialization(SpecializationType specializationType)
        {
            PortraitSpriteId portraitSpriteId = PortraitSpriteId.INVALID;

            switch (specializationType)
            {
                case SpecializationType.Archer:     portraitSpriteId = PortraitSpriteId.Archer; break;
                case SpecializationType.Footman:    portraitSpriteId = PortraitSpriteId.Footman; break;
                case SpecializationType.Monk:       portraitSpriteId = PortraitSpriteId.Monk; break;
                default:
                    break;
            }

            return portraitSpriteId;
        }
    }
}
