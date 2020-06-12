using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class StatusEffectInfo
{
    

    public StatusEffectType Type;
    public int MaxStackCount = 5;
    public int Duration = 3;
    public bool Beneficial = true;
    public StatusIconSpriteId SpriteId;
}

