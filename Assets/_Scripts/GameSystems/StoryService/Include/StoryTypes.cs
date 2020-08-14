using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;


namespace MAGE.GameServices.Story
{
    enum StoryArcId
    {
        INVALID = -1,

        Main,
        Test,

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

    interface IStoryArcUpdatedListener
    {
        void StoryArcUpdated(StoryArcId storyArcId, StoryArcUpdateType updateType);
    }


    class StoryArcInfo
    {
        public string StoryArcName;
        public string CurrentObjective;
        public string CurrentDescription;
    }
}
