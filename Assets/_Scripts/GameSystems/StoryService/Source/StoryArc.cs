using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Story.Internal
{
    class StoryArc
    {
        private string TAG = "StoryArc";
        private const int INACTIVE_IDX = -1;

        private StoryArcId mId;
        private int mCurrentNodeIdx = INACTIVE_IDX;
        private List<StoryNode> mStoryArc = new List<StoryNode>();
        private StoryObjective mActivationObjective;

        public StoryArc(StoryArcId id, List<StoryNode> nodes, StoryObjective activationCondition)
        {
            mId = id;
            mStoryArc = nodes;
            mActivationObjective = activationCondition;
            TAG += "-" + id.ToString();
        }

        public StoryArcUpdateType NotifyEvent(StoryEventBase storyEvent)
        {
            StoryArcUpdateType updateType = StoryArcUpdateType.None;

            bool arcActivated = 
                !HasArcStarted() 
                && mActivationObjective.HandleStoryEvent(storyEvent) 
                && mActivationObjective.IsMet();

            bool arcProgressed = 
                IsArcActive() 
                && mStoryArc[mCurrentNodeIdx].HandleEvent(storyEvent) 
                && mStoryArc[mCurrentNodeIdx].IsComplete();

            if (arcActivated)
            {
                updateType = StoryArcUpdateType.Started;

                Logger.Log(LogTag.Story, TAG, "StoryArc Started!");
                ProgressStoryArc();
            }
            else if (arcProgressed)
            {
                updateType = StoryArcUpdateType.Progressed;

                Logger.Log(LogTag.Story, TAG, "StoryArc Progressed!");
                ProgressStoryArc();

                if (IsArcComplete())
                {
                    updateType = StoryArcUpdateType.Completed;
                }
            }

            return updateType;
        }

        public StoryArcInfo GetArcInfo()
        {
            StoryArcInfo storyArcInfo = new StoryArcInfo();

            storyArcInfo.StoryArcId = mId;
            storyArcInfo.Stage = mCurrentNodeIdx;

            storyArcInfo.StoryArcName = mId.ToString();

            if (IsArcActive())
            {
                storyArcInfo.Status = StoryArcStatus.Active;
                storyArcInfo.CurrentObjective = mStoryArc[mCurrentNodeIdx].Name;
                storyArcInfo.CurrentDescription = mStoryArc[mCurrentNodeIdx].Description;
            }
            else if (IsArcComplete())
            {
                storyArcInfo.Status = StoryArcStatus.Complete;
            }
            else
            {
                storyArcInfo.Status = StoryArcStatus.Inactive;
            }

            return storyArcInfo;
        }

        public bool HasArcStarted()
        {
            return mCurrentNodeIdx != INACTIVE_IDX;
        }

        public bool IsArcActive()
        {
            return HasArcStarted() && !IsArcComplete();
        }

        public bool IsArcComplete()
        {
            return mCurrentNodeIdx == mStoryArc.Count;
        }

        private void ProgressStoryArc()
        {
            if (mCurrentNodeIdx != INACTIVE_IDX)
            {
                foreach (StoryMutatorParams cleanupAction in mStoryArc[mCurrentNodeIdx].ChangesOnCompletion)
                {
                    StoryMutator.MutateGame(cleanupAction);
                }
            }

            mCurrentNodeIdx++;

            if (mCurrentNodeIdx < mStoryArc.Count)
            {
                foreach (StoryMutatorParams activationAction in mStoryArc[mCurrentNodeIdx].ChangesOnActivation)
                {
                    StoryMutator.MutateGame(activationAction);
                }
            }
        }
    }
}
