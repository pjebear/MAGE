using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameSystems
{
    static class OrientationConstants
    {
        public static Vector3 FORWARD = Vector3.forward;
        public static Vector3 RIGHT = Vector3.right;
        public static Vector3 LEFT = Vector3.left;
        public static Vector3 BACK = Vector3.back;
    }


    enum RelativeOrientation
    {
        Front,
        Left,
        Right,
        Behind,
        OnTopOf
    }

    enum Orientation
    {
        Forward,
        Right,
        Left,
        Back,
        NUM
    }

    static class OrientationUtil
    {
        public static Vector3 ToVector(Orientation orientation)
        {
            Vector3 vector = OrientationConstants.FORWARD;
            switch (orientation)
            {
                case Orientation.Forward: vector = OrientationConstants.FORWARD; break;
                case Orientation.Right: vector = OrientationConstants.RIGHT; break;
                case Orientation.Back: vector = OrientationConstants.BACK; break;
                case Orientation.Left: vector = OrientationConstants.LEFT; break;
                default: Debug.Assert(false); break;
            }

            return vector;
        }

        public static Orientation FromVector(Vector3 vector)
        {
            Orientation closestOrientation = Orientation.Forward;
            float closestAngle = float.MaxValue;

            vector.y = 0;
            vector.Normalize();

            for (int i = 0; i < (int)Orientation.NUM; ++i)
            {
                float angle = Mathf.Abs(Vector3.Angle(vector, ToVector((Orientation)i)));
                if (angle < closestAngle)
                {
                    closestOrientation = (Orientation)i;
                    closestAngle = angle;
                }
            }

            return closestOrientation;
        }

        public static Orientation Flip(Orientation toFlip)
        {
            Orientation flipped = Orientation.NUM;
            switch (toFlip)
            {
                case Orientation.Forward: flipped = Orientation.Back; break;
                case Orientation.Back: flipped = Orientation.Forward; break;
                case Orientation.Right: flipped = Orientation.Left; break;
                case Orientation.Left: flipped = Orientation.Right; break;
            }
            return flipped;
        }
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

        public TileIdx GetTileToThe(Orientation orientation)
        {
            TileIdx toThe = default;
            switch (orientation)
            {
                case Orientation.Forward:   toThe = new TileIdx(x, y + 1); break;
                case Orientation.Right:     toThe = new TileIdx(x + 1, y); break;
                case Orientation.Back:      toThe = new TileIdx(x, y - 1); break;
                case Orientation.Left:      toThe = new TileIdx(x - 1, y); break;
            }

            return toThe;
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

    class TileConnections
    {
        public bool[] Connections = new bool[(int)Orientation.NUM];

        public bool this[Orientation orientation] { get { return Connections[(int)orientation]; } }
    }
}
