using MAGE.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Story.Internal
{
    class StoryObjective
    {
        public StoryEventType ListeningForEvent;
        public int ListeningForParam;
        public int Progress;
        public int Goal;

        public StoryObjective(StoryEventType storyEvent, int param, int progress, int goal)
        {
            ListeningForEvent = storyEvent;
            ListeningForParam = param;
            Progress = 0;
            Goal = goal;
        }

        public bool HandleStoryEvent(StoryEventBase storyEvent)
        {
            bool handled = false;

            if (storyEvent.EventType == ListeningForEvent
                && storyEvent.Param == ListeningForParam)
            {
                handled = true;

                Progress += storyEvent.Count;
            }

            return handled;
        }

        public bool IsMet()
        {
            return Progress >= Goal;
        }
    }
}
