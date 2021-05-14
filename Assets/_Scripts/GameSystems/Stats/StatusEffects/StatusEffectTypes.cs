using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Stats
{
    static class StatusEffectConstants
    {
        public const int UNTIL_NEXT_TURN = -2;
        public const int PERMANENT_DURATION = -1;
        public const int SHORT_DURATION = 8;
        public const int MEDIUM_DURATION = 16;
        public const int LONG_DURATION = 24;
    }

    enum StatusEffectId
    {
        INVALID = -1,

        Avenger,
        Daze,
        Defend,
        Disarm,
        DoubleTime,
        Hamstring,
        Poison,
        Protection,
        Aura_Protection,
        Regen,
        Root,
        Aura_Regen,
        BloodScent,
        Aura_RighteousGlory,

        NUM
    }

    class StatusEffectInfo
    {
        public StatusEffectId Type;
        public int MaxStackCount = 5;
        public int Duration = 3;
        public bool Beneficial = true;
        public UI.StatusIconSpriteId SpriteId;
    }
}
