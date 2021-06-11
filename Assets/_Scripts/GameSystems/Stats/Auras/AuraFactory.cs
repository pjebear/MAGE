using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameSystems.Stats
{
    static class AuraFactory
    {
        public static AuraInfo CheckoutAuraInfo(AuraType type)
        {
            int range = 2;
            bool isBeneficial = true;
            StatusEffectId auraEffectType = StatusEffectId.NUM;
            switch (type)
            {
                case AuraType.Protection:
                    auraEffectType = StatusEffectId.Aura_Protection;
                    break;
                case AuraType.Regen:
                    auraEffectType = StatusEffectId.Aura_Regen;
                    break;
                case AuraType.RighteousGlory:
                    auraEffectType = StatusEffectId.Aura_RighteousGlory;
                    break;
                case AuraType.ScorchedEarth:
                    auraEffectType = StatusEffectId.ScorchedEarth;
                    isBeneficial = false;
                    range = 3;
                    break;

                default:
                    Debug.Assert(false);
                    break;
            }

            return new AuraInfo(range, isBeneficial, type, auraEffectType);
        }
    }
}



