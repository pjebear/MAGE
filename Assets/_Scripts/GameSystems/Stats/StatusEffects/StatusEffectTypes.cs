using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Stats
{
    static class StatusEffectConstants
    {
        public static int PERMANENT_DURATION = -1;
        public static int SHORT_DURATION = 8;
        public static int MEDIUM_DURATION = 16;
        public static int LONG_DURATION = 24;
    }

    enum StatusEffectId
    {
        INVALID = -1,

        Avenger,
        Poison,
        Protection,
        Aura_Protection,
        Regen,
        Aura_Regen,
        Shackle,
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
