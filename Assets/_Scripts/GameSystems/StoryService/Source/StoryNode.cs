using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Story.Internal
{
    class StoryNode
    {
        public string Name = "NAME";
        public string Description = "DESCRIPTION";
        public List<StoryObjective> Objectives = new List<StoryObjective>();
        public List<StoryMutatorParams> ChangesOnActivation = new List<StoryMutatorParams>();
        public List<StoryMutatorParams> ChangesOnCompletion = new List<StoryMutatorParams>();

        public bool HandleEvent(StoryEventBase storyEvent)
        {
            bool handled = false;
            foreach (StoryObjective objective in Objectives)
            {
                handled |= objective.HandleStoryEvent(storyEvent);
            }
            return handled;
        }

        public bool IsComplete()
        {
            bool completed = false;
            foreach (StoryObjective objective in Objectives)
            {
                completed |= objective.IsMet();
            }
            return completed;
        }
    }
}

