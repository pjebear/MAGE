using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MAGE.GameServices
{
    interface IStoryService : Services.IService
    {
        List<Story.StoryArcInfo> GetActiveStoryArcs();
        void NotifyStoryEvent(Story.StoryEventBase storyEvent);
    }

    abstract class StoryService : Services.ServiceBase<IStoryService> { }
}
