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
        NewGame,
        PropInteractedWith
    }

    class StoryEventBase
    {
        public StoryEventType EventType;
        public int Param;

        public StoryEventBase(EncounterScenarioId encounterCompleteId) : this (StoryEventType.EncounterComplete, (int)encounterCompleteId) { }
        public StoryEventBase(ConversationId conversationId) : this(StoryEventType.ConversationComplete, (int)conversationId) { }
        public StoryEventBase(ItemId itemAddedId) : this(StoryEventType.ItemAddedToInventory, (int)itemAddedId) { }
        public StoryEventBase(PropTag propInteractedWith) : this(StoryEventType.PropInteractedWith, propInteractedWith.Id) { }
        public StoryEventBase(StoryEventType eventType, int param = -1)
        {
            EventType = eventType;
            Param = param;
        }
    }
}
