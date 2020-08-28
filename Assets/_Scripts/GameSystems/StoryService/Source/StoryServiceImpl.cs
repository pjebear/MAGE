using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Story.Internal
{
    class StoryServiceImpl : IStoryService
    {
        private readonly string TAG = "StoryServiceImpl";

        private Dictionary<StoryArcId, StoryArc> mStoryArcs = new Dictionary<StoryArcId, StoryArc>();

        // IService
        public void Init()
        {
            for (int i = 0; i < (int)StoryArcId.NUM; ++i)
            {
                mStoryArcs.Add((StoryArcId)i, StoryDBUtil.FromDB(DBService.Get().LoadStoryArcInfo(i)));
            }
        }

        public void Takedown()
        {
           
        }

        public List<StoryArcInfo> GetActiveStoryArcs()
        {
            List<StoryArcInfo> activeStoryArcs = new List<StoryArcInfo>();

            foreach (var storyArcPair in mStoryArcs)
            {
                if (storyArcPair.Value.IsArcActive())
                {
                    activeStoryArcs.Add(storyArcPair.Value.GetArcInfo());
                }
            }

            return activeStoryArcs;
        }

        public void NotifyStoryEvent(StoryEventBase storyEvent)
        {
            foreach (var storyArcPair in mStoryArcs)
            {
                StoryArcUpdateType updateType = storyArcPair.Value.NotifyEvent(storyEvent);
                if (updateType != StoryArcUpdateType.None)
                {
                    Messaging.MessageRouter.Instance.NotifyMessage(new StoryMessage(MessageType.StoryArcUpdated, storyArcPair.Key));
                }
            }
        }
    }
}
