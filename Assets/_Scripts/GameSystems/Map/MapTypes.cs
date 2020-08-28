using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameSystems
{
    enum RelativeOrientation
    {
        Front,
        Left,
        Right,
        Behind
    }

    enum Orientation
    {
        Forward,
        Right,
        Left,
        Back
    }

    struct TileIdx
    {
        public int x;
        public int y;

        public TileIdx(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static int ManhattanDistance(TileIdx lhs, TileIdx rhs)
        {
            int xDiff = Mathf.Abs(lhs.x - rhs.x);
            int yDiff = Mathf.Abs(lhs.y - rhs.y);

            return xDiff + yDiff;
        }

        public static Vector2 Displacement(TileIdx lhs, TileIdx rhs)
        {
            return new Vector2(rhs.x - lhs.x, rhs.y - lhs.y);
        }

        public static float DistanceBetween(TileIdx lhs, TileIdx rhs)
        {
            return Displacement(lhs, rhs).magnitude;
        }

        public static bool operator ==(TileIdx lhs, TileIdx rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y;
        }

        public static bool operator !=(TileIdx lhs, TileIdx rhs)
        {
            return !(lhs == rhs);
        }
    }

    struct CharacterPosition
    {
        public TileIdx Location;
        public Orientation Orientation;

        public CharacterPosition(TileIdx location, Orientation orientation)
        {
            Location = location;
            Orientation = orientation;
        }
    }

    enum TileState
    {
        Empty,
        Occupied,
        Obstructed,

        NUM
    }


    class Tile
    {
        public TileIdx TileIdx;
        public float Elevation;
        public bool IsObstructed;
        public Character OnTile;

        public Tile(TileIdx tileIdx, float elevation, bool isObstructed)
        {
            TileIdx = tileIdx;
            Elevation = elevation;
            IsObstructed = isObstructed;
        }
    }
}
