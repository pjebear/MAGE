using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;


namespace MAGE.GameSystems.Story
{
    enum StoryArcId
    {
        INVALID = -1,

        Main,
        Test,
        DebugLevel,
        OnBoardingBlacksmith,

        NUM
    }

    enum StoryArcUpdateType
    {
        None,
        Started,
        Progressed,
        Completed,
        NUM
    }

    enum StoryArcStatus
    {
        Inactive,
        Active,
        Complete,

        NUM
    }

    interface IStoryArcUpdatedListener
    {
        void StoryArcUpdated(StoryArcId storyArcId, StoryArcUpdateType updateType);
    }


    class StoryArcInfo
    {
        public StoryArcId StoryArcId = StoryArcId.INVALID;
        public StoryArcStatus Status = StoryArcStatus.Inactive;
        public int Stage = 0;

        public string StoryArcName;
        public string CurrentObjective;
        public string CurrentDescription;
    }

    class StoryProgress
    {
        public Dictionary<StoryArcId, StoryArcInfo> StoryArcs = new Dictionary<StoryArcId, StoryArcInfo>();

        public List<StoryArcId> NotStarted = new List<StoryArcId>();
        public List<StoryArcId> InProgress = new List<StoryArcId>();
        public List<StoryArcId> Completed = new List<StoryArcId>();
    }
}
