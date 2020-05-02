using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

enum AreaType
{
    Circle,
    Ring,
    Cone,
    Line,
    Cross,

    NUM
}

struct RangeInfo
{
    public static RangeInfo Unit { get { return new RangeInfo(0, 0, 0, AreaType.Circle); } }
    public int MinRange;
    public int MaxRange;
    public int MaxElevationChange;
    public AreaType AreaType;

    public RangeInfo(int minRange, int maxRange, int maxElevationChange, AreaType areaType)
    {
        MinRange = minRange;
        MaxRange = maxRange;
        MaxElevationChange = maxElevationChange;
        AreaType = areaType;
    }
}

struct ActionTargetDetails
{
    public RangeInfo CastDetails;
    public RangeInfo SelectionDetails;
    public bool IsSelfCast;

    public ActionTargetDetails(RangeInfo castDetails, RangeInfo selectionDetails, bool isSelfCast)
    {
        CastDetails = castDetails;
        SelectionDetails = selectionDetails;
        IsSelfCast = isSelfCast;
    }
}

struct TargetSelection
{
    public Target FocalTarget;
    public RangeInfo SelectionRange;
    public TargetSelection(Target focalTarget, RangeInfo range)
    {
        FocalTarget = focalTarget;
        SelectionRange = range;
    }

    public TargetSelection(Target target)
        : this(target, RangeInfo.Unit)
    {
    }
}


enum TargetSelectionType
{
    Tile,
    Actor,

    NUM
}

struct Target
{
    public TileIdx TileTarget;
    public EncounterCharacter ActorTarget;
    public TargetSelectionType TargetType;

    public Target(TileIdx tileTarget)
    {
        TargetType = TargetSelectionType.Tile;
        TileTarget = tileTarget;
        ActorTarget = null;
    }

    public Target(EncounterCharacter actorTarget)
    {
        TargetType = TargetSelectionType.Actor;
        ActorTarget = actorTarget;
        TileTarget = new TileIdx();
    }

    public Transform GetTargetTransform()
    {
        if (TargetType == TargetSelectionType.Actor)
        {
            return EncounterModule.CharacterDirector.CharacterActorLookup[ActorTarget].transform;
        }
        else
        {
            return EncounterModule.Map[TileTarget].transform;
        }
    }
}