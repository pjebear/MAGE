using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Stats
{
    class AuraInfo
    {
        public int Range;
        public bool IsBeneficial;
        public AuraType Type;
        public StatusEffectId AuraEffectType;

        public AuraInfo(int range, bool isBeneficial, AuraType type, StatusEffectId auraEffectType)
        {
            Range = range;
            IsBeneficial = isBeneficial;
            Type = type;
            AuraEffectType = auraEffectType;
        }
    }
}



