using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MAGE.GameSystems
{
    interface IStoryService : Services.IService
    {
        List<Story.StoryArcInfo> GetActiveStoryArcs();
        Story.StoryProgress GetStoryProgress();
        void NotifyStoryEvent(Story.StoryEventBase storyEvent);
    }

    abstract class StoryService : Services.ServiceBase<IStoryService> { }
}
