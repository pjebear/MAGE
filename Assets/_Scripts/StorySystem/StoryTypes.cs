using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common.EncounterEnums;
using Common.EncounterTypes;
using WorldSystem.Common;

namespace StorySystem.Common
{
    class StoryEventPayload
    {
        public StoryEventType EventType { get; private set; }
        public int EventId { get; private set; }
        public StoryEventPayload(StoryEventType eventType, int eventId)
        {
            EventType = eventType;
            EventId = eventId;
        }
    }

    //class EncounterEventPayload : StoryEventPayload
    //{

    //    public EncounterEventPayload(EncounterScenarioId id)
    //        : base(StoryEventType.Encounter, (int)id)
    //    {
    //        EncounterId = id;
    //    }
    //}

    //class TravelEventPayload : StoryEventPayload
    //{
    //    public MapLocationId MapLocation;
    //    public TravelEventPayload(MapLocationId locationId)
    //        :base (StoryEventType.Travel)
    //    {
    //        MapLocation = locationId;
    //    }
    //}

    public delegate void StoryArcManipulation();

    class StoryArc
    {
        public bool IsComplete { get; private set; }
        private StoryArcId mId;
        public StoryArcManipulation _BeginArc = null;
        private List<StoryArcNode> mStoryArc;
        private int mCurrentNodeIdx;

        public StoryArc(StoryArcId id, StoryArcManipulation beginArc, List<StoryArcNode> nodes)
        {
            mId = id;
            IsComplete = false;
            mCurrentNodeIdx = 0;
            _BeginArc = beginArc;
            mStoryArc = nodes;
        }

        public bool ClaimEvent(StoryEventPayload payload)
        {
            int claimedIndex = -1;
            for (int i = mCurrentNodeIdx; i < mStoryArc.Count; ++i)
            {
                if (mStoryArc[i].ClaimStoryEvent(payload))
                {
                    claimedIndex = i;
                    break;
                }
            }

            if (claimedIndex != -1) // claimed a node
            {
                if (claimedIndex > mCurrentNodeIdx)
                {
                    UnityEngine.Debug.Log("StoryArc " + mId.ToString() + " claimed story event that bypassed " + (claimedIndex - mCurrentNodeIdx) + " nodes. Progressing to claimed node");
                    while (mCurrentNodeIdx < claimedIndex)
                    {
                        ProgressArc();
                    }
                }
                return true;
            }
            return false;
        }

        public void ProgressArc()
        {
            UnityEngine.Debug.Assert(mCurrentNodeIdx < mStoryArc.Count);
            mStoryArc[mCurrentNodeIdx]._ProgressStory();
            ++mCurrentNodeIdx;
            if (mCurrentNodeIdx == mStoryArc.Count)
            {
                IsComplete = true;
            }
        }
    }

    class StoryArcNode
    {
        private StoryEventType EventType;
        private int EventId;
        public StoryArcManipulation _ProgressStory { get; private set; }

        public StoryArcNode(StoryEventType eventType, int eventId, StoryArcManipulation manipulationFunction)
        {
            EventType = eventType;
            EventId = eventId;
            _ProgressStory = manipulationFunction;
        }

        public bool ClaimStoryEvent(StoryEventPayload storyEvent)
        {
            return EventType == storyEvent.EventType && EventId == storyEvent.EventId;
        }
    }
}
