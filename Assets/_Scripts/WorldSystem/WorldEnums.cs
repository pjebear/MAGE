using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldSystem.Common
{
    enum MapLocationId
    {
        INVALID = -1,

        NUM
    }

    enum MapLocationState
    {
        INVALID = -1,

        Default,
        StoryEvent,
        ImmediateEncounter,

        NUM
    }

    enum WorldTransitionType
    {
        Encounter,
        Map,
        Area,
        Cinematic,
        TitleScreen
    }
}
