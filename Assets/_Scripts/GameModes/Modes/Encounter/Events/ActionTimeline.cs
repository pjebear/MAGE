using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

interface ITimelineEvent
{
    int StartPointOffset { get; set; }
    int DurationFrames { get; set; }
    void Trigger();
}

class Timeline<T> where T : ITimelineEvent
{
    private float mElapsedTime = 0f;
    private int mTimelineIdx = 0;
    private List<T> mActiveElements;
    private List<T> mPendingElements;

    protected Timeline()
    {
        mPendingElements = new List<T>();
        mActiveElements = new List<T>();
    }

    public Timeline(List<T> timeline)
        : this()
    {
        mPendingElements.AddRange(timeline);
        mPendingElements.Sort((x, y) => { return x.StartPointOffset.CompareTo(y.StartPointOffset); });
    }

    public bool ProgressTimeline(float dt)
    {
        mElapsedTime += dt;
        int currentFrame = AnimationConstants.FRAMES_IN_DURATION(mElapsedTime);

        List<T> startedThisFrame = new List<T>();

        while (mTimelineIdx < mPendingElements.Count
            && mPendingElements[mTimelineIdx].StartPointOffset <= currentFrame)
        {
            mPendingElements[mTimelineIdx].Trigger();
            startedThisFrame.Add(mPendingElements[mTimelineIdx]);
            mTimelineIdx++;
        }

        mActiveElements.AddRange(startedThisFrame);
        mActiveElements.RemoveAll(x => x.StartPointOffset + x.DurationFrames < currentFrame);

        return mActiveElements.Count == 0 // no elements in progress
            && mTimelineIdx == mPendingElements.Count; // no elements pending
    }
}

