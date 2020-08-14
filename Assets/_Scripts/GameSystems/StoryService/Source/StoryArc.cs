using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameServices.Story.Internal
{
    class StoryArc
    {
        private string TAG = "StoryArc";
        private const int INACTIVE_IDX = -1;

        private StoryArcId mId;
        private int mCurrentNodeIdx = INACTIVE_IDX;
        private List<StoryNode> mStoryArc = new List<StoryNode>();
        private StoryCondition mActivationCondition;

        public StoryArc(StoryArcId id, List<StoryNode> nodes, StoryCondition activationCondition)
        {
            mId = id;
            mStoryArc = nodes;
            mActivationCondition = activationCondition;
            TAG += "-" + id.ToString();
        }

        public StoryArcUpdateType NotifyEvent(StoryEventBase storyEvent)
        {
            StoryArcUpdateType updateType = StoryArcUpdateType.None;

            bool arcActivated = !HasArcStarted() && mActivationCondition.IsMet(storyEvent);
            bool arcProgressed = IsArcActive() && mStoryArc[mCurrentNodeIdx].Requirement.IsMet(storyEvent);
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

            storyArcInfo.StoryArcName = mId.ToString();

            if (IsArcActive())
            {    
                storyArcInfo.CurrentObjective = mStoryArc[mCurrentNodeIdx].Description;
                storyArcInfo.CurrentDescription = mStoryArc[mCurrentNodeIdx].Name;
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
