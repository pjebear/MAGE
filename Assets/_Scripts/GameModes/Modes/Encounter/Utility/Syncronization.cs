using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

interface ISynchronizable
{
    int NumFrames { get; }
    int SyncedFrame { get; }
}

class SyncPoint
{
    public ISynchronizable Referenced { get; }   // The root Synchronizable
    public ISynchronizable Synced { get; }        // The synchronizable that 
    private int StartPointOffset;

    protected SyncPoint()
        : this(null, null, 0)
    {
    }

    protected SyncPoint(ISynchronizable reference, ISynchronizable synced, int startPointOffset)
    {
        Referenced = reference;
        Synced = synced;
        StartPointOffset = startPointOffset;
    }

    public int GetRelativeOffset(AllignmentPosition referenceAllignment, AllignmentPosition syncAllignment)
    {
        int offset = StartPointOffset;

        switch (referenceAllignment)
        {
            case AllignmentPosition.Interaction:
                offset -= Referenced.SyncedFrame;
                break;
            case AllignmentPosition.End:
                offset -= Referenced.NumFrames;
                break;
        }

        switch (syncAllignment)
        {
            case AllignmentPosition.Interaction:
                offset += Synced.SyncedFrame;
                break;
            case AllignmentPosition.End:
                offset += Synced.NumFrames;
                break;
        }

        return offset;
    }

    public static int GetStartPointOffset(ISynchronizable syncro, AllignmentPosition alignPos, int beingSyncedOffset)
    {
        int startPointOffset = beingSyncedOffset;

        if (alignPos == AllignmentPosition.Start)
        {
            // nothing
        }
        else if (alignPos == AllignmentPosition.Interaction)
        {
            startPointOffset += syncro.SyncedFrame;

        }
        else if (alignPos == AllignmentPosition.End)
        {
            startPointOffset += syncro.NumFrames;
        }

        return startPointOffset;
    }

    public static int GetStartPointOffset(ISynchronizable syncroA, AllignmentPosition alignPosA, ISynchronizable syncroB, AllignmentPosition allignPosB, int beingSyncedOffset)
    {
        int startPointOffset = beingSyncedOffset;

        startPointOffset += GetStartPointOffset(syncroA, alignPosA, 0);
        startPointOffset -= GetStartPointOffset(syncroB, allignPosB, 0);

        return startPointOffset;
    }
}

class AllignmentPoint<T, V>
{
    public T toAllign;
    public AllignmentPosition allignmentPosition;
    public V offset;
    public AllignmentPoint(T toAllign, AllignmentPosition allignmentPosition, V offset)
    {
        this.toAllign = toAllign;
        this.allignmentPosition = allignmentPosition;
        this.offset = offset;
    }
}

class AllignmentLink<T, V>
{
    public AllignmentPoint<T, V> from;
    public AllignmentPoint<T, V> to;

    public AllignmentLink(AllignmentPoint<T, V> from, AllignmentPoint<T, V> to)
    {
        this.from = from;
        this.to = to;
    }
}

class InteractionFrameLink : AllignmentLink<int, int>
{
    public InteractionFrameLink(int idA, AllignmentPosition allignPosA, int offsetA, int idB, AllignmentPosition allignPosB, int offsetB)
        : base(new AllignmentPoint<int, int>(idA, allignPosA, offsetA), new AllignmentPoint<int, int>(idB, allignPosB, offsetB))
    {
    }
}

enum AllignmentPosition
{
    Start,
    Interaction,
    End,
    NUM
}