using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems
{
    enum AnimationSide
    {
        None,
        Left,
        Right,

        NUM
    }

    public enum AnimationId
    {
        INVALID = -1,

        BowDraw,
        SwordSwing,
        Cleave,
        Block,
        Dodge,
        Parry,
        Hurt,
        Cast,
        ChargeUp,
        Faint,
        Revive,
        DaggerStrike,

        NUM
    }
}
