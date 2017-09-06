
using UnityEngine;

namespace EncounterSystem.MapTypes
{

    using MapEnums;
    struct TilePath
    {
        public MapTile previous;
        public int pathLength;
        public bool isValidEndTile;
        public TilePath(int length, MapTile prev, bool canFinishMoveOnTile)
        {
            previous = prev;
            pathLength = length;
            isValidEndTile = canFinishMoveOnTile;
        }
    }

    public struct TileIndex
    {
        public int x;
        public int y;
        public TileIndex(Vector2 index)
        {
            x = (int)index.x;
            y = (int)index.y;
        }
        public TileIndex(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static TileIndex up { get { return new TileIndex(0, 1); } }
        public static TileIndex down { get { return new TileIndex(0, -1); } }
        public static TileIndex left { get { return new TileIndex(-1, 0); } }
        public static TileIndex right { get { return new TileIndex(1, 0); } }
        public static TileIndex Invalid { get { return new TileIndex(-1, -1); } }


        public static TileIndex operator +(TileIndex lhs, TileIndex rhs)
        {
            return new TileIndex(lhs.x + rhs.x, lhs.y + rhs.y);
        }
        public static TileIndex operator -(TileIndex lhs, TileIndex rhs)
        {
            return new TileIndex(lhs.x - rhs.x, lhs.y - rhs.y);
        }

        public static bool operator ==(TileIndex lhs, TileIndex rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y;
        }

        public static bool operator !=(TileIndex lhs, TileIndex rhs)
        {
            return !(lhs == rhs);
        }
    };

    public struct MapInteractionInfo
    {
        public TargetSelectionType TargetSelectionType;
        public TileAreaType AOEAreaType;
        public TileAreaType ActionAreaType;
        public int MaxRange;
        public int MinRange;
        public int MaxRangeElevation;
        public int MaxAoe;
        public int MinAoe;
        public int MaxAoeElevation;

        public MapInteractionInfo(TargetSelectionType selectionType, TileAreaType actionAreaType, TileAreaType aoeAreaType,
            int maxRange, int minRange, int maxRangeElevation, int maxAoe, int minAoe, int maxAoeElevation)
        {
            TargetSelectionType = selectionType;
            AOEAreaType = aoeAreaType;
            ActionAreaType = actionAreaType;
            MaxRange = maxRange;
            MinRange = minRange;
            MaxRangeElevation = maxRangeElevation;
            MaxAoe = maxAoe;
            MinAoe = minAoe;
            MaxAoeElevation = maxAoeElevation;
        }
    }
}

