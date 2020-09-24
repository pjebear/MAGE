﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Story.Internal
{
    static class MutatorConstants
    {
        public const int OPEN = 1;
        public const int CLOSE = 0;
        public const int TRUE = 1;
        public const int FALSE = 0;
        public const int ADD = 1;
        public const int REMOVE = 0;
    }


    enum StoryMutatorType
    {
        Cinematic,
        Encounter,
        Prop,
        Scenario,
        Party
    }

    enum PropMutatorType
    {
        Item_Add,
        Item_Remove,
        Conversation_Add,
        Conversation_Remove,
        Activate,
        Interactible,
        StateChange,
    }

    enum CinematicMutatorType
    {
        Activate
    }

    enum EncounterMutatorType
    {
        Activate
    }

    enum ScenarioMutatorType
    {
        Activate
    }

    enum PartyMutatorType
    {
        Item_Add,
        Item_Remove,
        Party_Add,
        Party_Remove
    }

    class StoryMutatorParams
    {
        public StoryMutatorType StoryMutatorType;
        public int Param1;
        public int Param2;
        public int Param3;

        public StoryMutatorParams(EncounterMutatorType mutatorType, int encounterId, int param) : this(StoryMutatorType.Encounter, (int)mutatorType, encounterId, param) { }
        public StoryMutatorParams(CinematicMutatorType mutatorType, int cinematicId, int param) : this(StoryMutatorType.Cinematic, (int)mutatorType, cinematicId, param) { }
        public StoryMutatorParams(PropMutatorType mutatorType, int propId, int param) : this(StoryMutatorType.Prop, (int)mutatorType, propId, param) { }
        public StoryMutatorParams(ScenarioMutatorType mutatorType, int scenarioId, int param) : this(StoryMutatorType.Scenario, (int)mutatorType, scenarioId, param) { }
        public StoryMutatorParams(PartyMutatorType mutatorType, int param) : this(StoryMutatorType.Party, (int)mutatorType, param, -1) { }
        public StoryMutatorParams(StoryMutatorType mutatorType, int param1, int param2, int param3)
        {
            StoryMutatorType = mutatorType;
            Param1 = param1;
            Param2 = param2;
            Param3 = param3;
        }
    }
}