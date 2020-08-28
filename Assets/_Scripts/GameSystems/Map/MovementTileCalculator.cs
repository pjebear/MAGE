using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MAGE.GameSystems
{
    struct TilePath
    {
        public Tile Previous;
        public int Length;
        public bool IsValidEndTile;

        public TilePath(Tile previous, int length, bool isValidEndTile)
        {
            Previous = previous;
            Length = length;
            IsValidEndTile = isValidEndTile;
        }
    }

    class MovementTileCalculator
    {
        private readonly string TAG = "MovementTileCalculator";

        private const float ElevationDifferenceBuffer = 0.1f;
        private Dictionary<Tile, TilePath> mPotentialMapPaths; // key = destination, value = path to get there
        private Dictionary<TileIdx, Tile> mPotentialMovementTiles; // key = tile id, value = tile
        private Map mMap;

        public MovementTileCalculator(Map map)
        {
            mPotentialMapPaths = new Dictionary<Tile, TilePath>();
            mPotentialMovementTiles = new Dictionary<TileIdx, Tile>();
            mMap = map;
        }

        public bool IsValidMoveTile(Tile tile)
        {
            return mPotentialMovementTiles.ContainsKey(tile.TileIdx);
        }

        public List<Tile> GetPathTo(Tile destination)
        {
            Logger.Assert(IsValidMoveTile(destination), LogTag.GameModes, TAG, "Invalid destination provided");

            List<Tile> movementPath = new List<Tile>();

            if (IsValidMoveTile(destination))
            {
                //create a path for unit to follow
                do
                {
                    movementPath.Add(destination);
                    if (mPotentialMapPaths.ContainsKey(destination))
                    {
                        destination = mPotentialMapPaths[destination].Previous;
                    }
                } while (mPotentialMapPaths[destination].Previous != null);

                movementPath.Reverse();
            }

            return movementPath;
        }

        public List<Tile> GetValidMovementTiles()
        {
            List<Tile> validMovementTiles = new List<Tile>();
            validMovementTiles.AddRange(mPotentialMovementTiles.Values);
            return validMovementTiles;
        }

        public Dictionary<Tile, TilePath> GetMapPaths()
        {
            return mPotentialMapPaths;
        }

        public void CalculatePathTiles(TileIdx startTile, int distance, float jump, TeamSide sideBeingMoved)
        {
            mPotentialMapPaths.Clear();
            mPotentialMovementTiles.Clear();

            int maxDistance = distance;

            List<KeyValuePair<Tile, int>> sortedList = new List<KeyValuePair<Tile, int>>();

            mPotentialMapPaths.Add(mMap.TileAt(startTile), new TilePath(null, 0, false));
            sortedList.Add(new KeyValuePair<Tile, int>(mMap.TileAt(startTile), 0));

            while (sortedList.Count() > 0)
            {
                sortedList.Sort((x, y) => y.Value.CompareTo(x.Value));

                Tile currentTile = sortedList[sortedList.Count - 1].Key;
                sortedList.RemoveAt(sortedList.Count - 1);
                TileIdx currentIndex = currentTile.TileIdx;

                //add all tiles around
                // Forward
                TileIdx nextIndex = new TileIdx(currentIndex.x, currentIndex.y + 1);
                if (mMap.IsValidIdx(nextIndex)) AddIfValid(ref sortedList, mMap.TileAt(nextIndex), currentTile, maxDistance, jump, sideBeingMoved);
                // Right
                nextIndex = new TileIdx(currentIndex.x + 1, currentIndex.y);
                if (mMap.IsValidIdx(nextIndex)) AddIfValid(ref sortedList, mMap.TileAt(nextIndex), currentTile, maxDistance, jump, sideBeingMoved);
                // Behind
                nextIndex = new TileIdx(currentIndex.x, currentIndex.y - 1);
                if (mMap.IsValidIdx(nextIndex)) AddIfValid(ref sortedList, mMap.TileAt(nextIndex), currentTile, maxDistance, jump, sideBeingMoved);
                // Left
                nextIndex = new TileIdx(currentIndex.x - 1, currentIndex.y);
                if (mMap.IsValidIdx(nextIndex)) AddIfValid(ref sortedList, mMap.TileAt(nextIndex), currentTile, maxDistance, jump, sideBeingMoved);
            }
        }

        private void AddIfValid(ref List<KeyValuePair<Tile, int>> stack, Tile tileToCheck, Tile previousTile, int maxDistance, float maxJump, TeamSide sideBeingMoved)
        {
            // Tile is above/below max jump distance
            float elevationDifference = Mathf.Abs(tileToCheck.Elevation - previousTile.Elevation);
            if (elevationDifference > maxJump + ElevationDifferenceBuffer)
            {
                return;
            }

            int costToMoveToTile = (int)Mathf.Abs(1 + elevationDifference);

            if (mPotentialMapPaths.ContainsKey(tileToCheck))//has tile already been reached?
            {
                int newDistance = mPotentialMapPaths[previousTile].Length + costToMoveToTile;
                int oldDistance = mPotentialMapPaths[tileToCheck].Length;
                if (oldDistance > newDistance)// shorter path found
                {
                    TilePath path = mPotentialMapPaths[tileToCheck];
                    path.Previous = previousTile;
                    path.Length = newDistance;
                    mPotentialMapPaths[tileToCheck] = path;

                    // if found a shorter path to tile, check if needs to be added to potential movement tiles
                    if (oldDistance > maxDistance && newDistance <= maxDistance)
                    {
                        mPotentialMovementTiles.Add(tileToCheck.TileIdx, tileToCheck);
                    }
                }
                //else potential route is a longer path. Discard

                return;

            }
            else // new path found
            {
                // is there anyone currently on the tile?
                Character onTile = tileToCheck.OnTile;
                if (onTile != null)
                {
                    bool isOnSameTeam = onTile.TeamSide == sideBeingMoved;
                    //is the unit friendly?
                    if (isOnSameTeam || !onTile.IsAlive)
                    {
                        //not a valid tile, but valid path
                        mPotentialMapPaths.Add(tileToCheck, new TilePath(previousTile, mPotentialMapPaths[previousTile].Length + 1, false));
                    }
                    else //cant move through this tile. Discard
                    {
                        return;
                    }
                }
                else
                {
                    mPotentialMapPaths.Add(tileToCheck, new TilePath(previousTile, mPotentialMapPaths[previousTile].Length + 1, true));
                }
            }

            if (mPotentialMapPaths[tileToCheck].Length <= maxDistance && mPotentialMapPaths[tileToCheck].IsValidEndTile)
            {
                mPotentialMovementTiles.Add(tileToCheck.TileIdx, tileToCheck);
            }

            stack.Add(new KeyValuePair<Tile, int>(tileToCheck, mPotentialMapPaths[tileToCheck].Length));
        }

        public void ResetMovementTiles()
        {
            mPotentialMapPaths.Clear();
            mPotentialMovementTiles.Clear();
        }
    }

}
