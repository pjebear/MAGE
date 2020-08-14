using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameServices.Character
{
    class AuraInfo
    {
        public int Range;
        public bool IsBeneficial;
        public AuraType Type;
        public StatusEffectType AuraEffectType;

        public AuraInfo(int range, bool isBeneficial, AuraType type, StatusEffectType auraEffectType)
        {
            Range = range;
            IsBeneficial = isBeneficial;
            Type = type;
            AuraEffectType = auraEffectType;
        }
    }
}



