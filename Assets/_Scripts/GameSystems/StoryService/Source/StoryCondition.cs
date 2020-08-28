using MAGE.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Story.Internal
{
    class StoryCondition
    {
        public StoryEventType StoryEventType;
        public int EventParam;

        public StoryCondition(StoryEventType storyEventType, int eventParam)
        {
            StoryEventType = storyEventType;
            EventParam = eventParam;
        }

        public bool IsMet(StoryEventBase storyEventBase)
        {
            return storyEventBase.EventType == StoryEventType && EventParam == storyEventBase.Param;
        }
    }
}
