using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
                DB.DBStoryArcInfo dbStoryArc = DBService.Get().LoadStoryArcInfo(i);
                if (dbStoryArc.Id != -1)
                {
                    mStoryArcs.Add((StoryArcId)i, StoryDBUtil.FromDB(dbStoryArc));
                }
            }
        }

        public void Takedown()
        {
           
        }

        public StoryProgress GetStoryProgress()
        {
            StoryProgress storyProgress = new StoryProgress();

            foreach (var storyArcPair in mStoryArcs)
            {
                StoryArcInfo info = storyArcPair.Value.GetArcInfo();

                storyProgress.StoryArcs.Add(info.StoryArcId, info);

                switch (info.Status)
                {
                    default:
                    {
                        Debug.Assert(false);
                    }
                    break;
                    case StoryArcStatus.Inactive: storyProgress.NotStarted.Add(info.StoryArcId); break;
                    case StoryArcStatus.Active: storyProgress.InProgress.Add(info.StoryArcId); break;
                    case StoryArcStatus.Complete: storyProgress.Completed.Add(info.StoryArcId); break;
                }
            }

            return storyProgress;
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
                    Messaging.MessageRouter.Instance.NotifyMessage(new StoryMessage(MessageType.StoryArcUpdated, storyArcPair.Value));
                }
            }
        }
    }
}
