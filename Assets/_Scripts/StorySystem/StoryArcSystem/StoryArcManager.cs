using StorySystem.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StorySystem.StoryArcSystem
{
    class StoryArcManager
    {
        private Dictionary<StoryArcId, StoryArc> mActiveStoryArcLog;
        private Dictionary<StoryArcId, StoryArc> mCompletedStoryArcLog;

        public StoryArcManager()
        {
            mActiveStoryArcLog = new Dictionary<StoryArcId, StoryArc>();
            mCompletedStoryArcLog = new Dictionary<StoryArcId, StoryArc>();
        }

        public void BeginNewStoryArc(StoryArcId arcId)
        {
            UnityEngine.Debug.Assert(!mActiveStoryArcLog.ContainsKey(arcId), "Story Arc " + arcId.ToString() + " already in progress!");
            UnityEngine.Debug.Assert(!mCompletedStoryArcLog.ContainsKey(arcId), "Story Arc " + arcId.ToString() + " already completed!");

            StoryArc newArc = null;

            StoryArcFactory.CheckOutStoryArc(arcId, out newArc);

            newArc._BeginArc();
            mActiveStoryArcLog.Add(arcId, newArc);
        }

        public void ProgressStoryArcs(StoryEventPayload eventPayload)
        {
            Queue<StoryArcId> toProgress = new Queue<StoryArcId>();
            foreach (var storyArc in mActiveStoryArcLog)
            {
                if (storyArc.Value.ClaimEvent(eventPayload))
                {
                    toProgress.Enqueue(storyArc.Key);
                }
            }
            
            if (toProgress.Count > 0)
            {
                string debugTag = "Progressing story arcs: ";
                foreach (StoryArcId id in toProgress)
                {
                    debugTag += id.ToString() + " ";
                }
                UnityEngine.Debug.Log(debugTag);

                while (toProgress.Count > 0)
                {
                    StoryArcId arcId = toProgress.Dequeue();
                    StoryArc arc = mActiveStoryArcLog[arcId];
                    arc.ProgressArc();
                    if (arc.IsComplete)
                    {
                        mActiveStoryArcLog.Remove(arcId);
                        mCompletedStoryArcLog.Add(arcId, arc);
                    }
                }
            }
        }
    }
}
