using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


enum AreaType
{
    Circle,
    Ring,
    Cone,
    Line,
    Cross,

    NUM
}

class RangeInfo
{
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

class ActionTargetDetails
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

class TargetSelection
{
    public Target FocalTarget;
    public List<Target> PeripheralTargets;
    public TargetSelection(Target focalTarget, List<Target> peripheralTargets)
    {
        FocalTarget = focalTarget;
        PeripheralTargets = peripheralTargets;
    }

    public TargetSelection(Target target)
        : this(target, new List<Target>())
    {
    }
}


enum TargetSelectionType
{
    Tile,
    Actor,

    NUM
}

class Target
{
    public TileIdx TileTarget;
    public EncounterCharacter ActorTarget;
    public TargetSelectionType TargetType;

    public Target(TileIdx tileTarget)
    {
        TargetType = TargetSelectionType.Tile;
        TileTarget = tileTarget;
    }

    public Target(EncounterCharacter actorTarget)
    {
        TargetType = TargetSelectionType.Actor;
        ActorTarget = actorTarget;
    }
}