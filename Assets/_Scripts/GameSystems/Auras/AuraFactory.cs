using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

static class AuraFactory
{
    public static AuraInfo CheckoutAuraInfo(AuraType type)
    {
        int range = 2;
        bool isBeneficial = true;
        StatusEffectType auraEffectType = StatusEffectType.NUM;
        switch (type)
        {
            case AuraType.Protection:
                auraEffectType = StatusEffectType.Aura_Protection;
                break;
            case AuraType.Regen:
                auraEffectType = StatusEffectType.Aura_Regen;
                break;

            default:
                Debug.Assert(false);
                break;
        }

        return new AuraInfo(range, isBeneficial, type, auraEffectType);
    }
}

