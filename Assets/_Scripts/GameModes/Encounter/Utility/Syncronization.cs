using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

interface ISynchronizable
{
    SyncPoint Parent { get; set; }
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

    public int GetAbsoluteOffset(AllignmentPosition fromSyncPos)
    {
        int cummOffset = GetRelativeOffset(AllignmentPosition.Start, fromSyncPos);
        if (Referenced.Parent != null)
        {
            cummOffset += Referenced.Parent.GetAbsoluteOffset(AllignmentPosition.Start);
        }
        return cummOffset;
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

    public static int GetStartPointOffset(ISynchronizable syncroA, AllignmentPosition alignPosA, ISynchronizable syncroB, AllignmentPosition allignPosB, int beingSyncedOffset)
    {
        int startPointOffset = beingSyncedOffset;

        if (allignPosB == AllignmentPosition.Start)
        {
            // nothing
        }
        else if (allignPosB == AllignmentPosition.Interaction)
        {
            startPointOffset -= syncroB.SyncedFrame;
           
        }
        else if (allignPosB == AllignmentPosition.End)
        {
            startPointOffset -= syncroA.NumFrames;
        }

        if (alignPosA == AllignmentPosition.Start)
        {
            // nothing
        }
        else if (alignPosA == AllignmentPosition.Interaction)
        {
            startPointOffset += syncroA.SyncedFrame;
        }
        else if (alignPosA == AllignmentPosition.End)
        {
            startPointOffset += syncroA.NumFrames;
        }

        return startPointOffset;
    }

    public static SyncPoint Syncronize(ISynchronizable referenced, AllignmentPosition referencedPos, ISynchronizable beingSynced, AllignmentPosition beingSyncedPos, int beingSyncedOffset)
    {
        int startPointOffset = GetStartPointOffset(referenced, referencedPos, beingSynced, beingSyncedPos, beingSyncedOffset);

        SyncPoint synchronizedPoint = new SyncPoint(referenced, beingSynced, startPointOffset);
        beingSynced.Parent = synchronizedPoint;

        return synchronizedPoint;
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