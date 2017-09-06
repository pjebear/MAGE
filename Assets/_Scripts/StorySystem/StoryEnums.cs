using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StorySystem.Common
{
    enum StoryArcId
    {
        INVALID = -1,

        Main,

        NUM
    }

    enum StoryEventType
    {
        Encounter,
        Dialogue,
        Travel,
        Cinematic
    }
}
