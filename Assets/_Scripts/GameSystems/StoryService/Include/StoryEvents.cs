using MAGE.GameModes.SceneElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Story
{
    enum StoryEventType
    {
        EncounterComplete,
        ConversationComplete,
        ItemAddedToInventory,
        MobDefeated,
        NewGame,
        PropInteractedWith,
        CinematicComplete,
    }

    class StoryEventBase
    {
        public StoryEventType EventType;
        public int Param;
        public int Count;

        public StoryEventBase(EncounterScenarioId encounterCompleteId) : this (StoryEventType.EncounterComplete, (int)encounterCompleteId) { }
        public StoryEventBase(CinematicId cinematicId) : this(StoryEventType.CinematicComplete, (int)cinematicId) { }
        public StoryEventBase(ConversationId conversationId) : this(StoryEventType.ConversationComplete, (int)conversationId) { }
        public StoryEventBase(ItemId itemAddedId, int numAdded) : this(StoryEventType.ItemAddedToInventory, (int)itemAddedId, numAdded) { }
        public StoryEventBase(Mobs.MobId defeatedMobId, int numDefeated) : this(StoryEventType.MobDefeated, (int)defeatedMobId, numDefeated) { }
        public StoryEventBase(PropTag propInteractedWith) : this(StoryEventType.PropInteractedWith, propInteractedWith.Id) { }
        public StoryEventBase(StoryEventType eventType, int param = -1, int count = 1)
        {
            EventType = eventType;
            Param = param;
            Count = count;
        }
    }
}
