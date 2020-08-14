using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameServices
{
    enum LevelId
    {
        INVALID = -1,

        Forest,

        NUM
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

        public static bool operator ==(TileIdx lhs, TileIdx rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y;
        }

        public static bool operator !=(TileIdx lhs, TileIdx rhs)
        {
            return !(lhs == rhs);
        }
    }
}