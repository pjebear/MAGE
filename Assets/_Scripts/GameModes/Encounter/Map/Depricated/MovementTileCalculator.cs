using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;



//class MovementTileCalculator
//{
//    private const float ElevationDifferenceBuffer = 0.1f;
//    private Dictionary<MapTile, TilePath> mPotentialMapPaths; // key = destination, value = path to get there
//    private Dictionary<int, MapTile> mPotentialMovementTiles; // key = tile id, value = tile
//    private EncounterMap mEncounterMap;

//    public MovementTileCalculator(EncounterMap encounterMap)
//    {
//        mPotentialMapPaths = new Dictionary<MapTile, TilePath>();
//        mPotentialMovementTiles = new Dictionary<int, MapTile>();
//        mEncounterMap = encounterMap;
//    }

//    public bool IsValidMoveTile(MapTile tile)
//    {
//        return mPotentialMovementTiles.ContainsKey(tile.Id);
//    }

//    public Stack<MapTile> GetPathTo(MapTile destination)
//    {
//        if (IsValidMoveTile(destination))
//        {
//            //create a path for unit to follow
//            Stack<MapTile> movementPath = new Stack<MapTile>();
//            do
//            {
//                movementPath.Push(destination);
//                if (mPotentialMapPaths.ContainsKey(destination))
//                {
//                    destination = mPotentialMapPaths[destination].previous;
//                }
//            } while (mPotentialMapPaths[destination].previous != null);

//            return movementPath;
//        }
//        else
//        {
//            return null;
//        }
//    }

//    public List<MapTile> GetValidMovementTiles()
//    {
//        List<MapTile> validMovementTiles = new List<MapTile>();
//        validMovementTiles.AddRange(mPotentialMovementTiles.Values);
//        return validMovementTiles;
//    }

//    public Dictionary<MapTile, TilePath> GetMapPaths()
//    {
//        return mPotentialMapPaths;
//    }

//    public void CalculatePathTiles(MapTile startTile, int distance, float jump, bool playerControlled)
//    {
//        mPotentialMapPaths.Clear();
//        mPotentialMovementTiles.Clear();

//        int maxDistance = distance;

//        List<KeyValuePair<MapTile, int>> sortedList = new List<KeyValuePair<MapTile, int>>();
                
//        mPotentialMapPaths.Add(startTile, new TilePath(0, null, false));
//        sortedList.Add(new KeyValuePair<MapTile, int>(startTile, 0));

//        while (sortedList.Count() > 0)
//        {
//            sortedList.Sort((x, y) => y.Value.CompareTo(x.Value));

//            MapTile currentTile = sortedList[sortedList.Count- 1].Key;
//            sortedList.RemoveAt(sortedList.Count - 1);
//            TileIndex currentIndex = currentTile.GetLocalMapIndex();

//            //add all tiles around
//            TileIndex nextIndex = currentIndex + TileIndex.up;
//            AddIfValid(ref sortedList, mEncounterMap.GetTileAt(nextIndex), currentTile, maxDistance, jump, playerControlled);
//            nextIndex = currentIndex + TileIndex.
//                ;
//            AddIfValid(ref sortedList, mEncounterMap.GetTileAt(nextIndex), currentTile, maxDistance, jump, playerControlled);
//            nextIndex = currentIndex + TileIndex.down;
//            AddIfValid(ref sortedList, mEncounterMap.GetTileAt(nextIndex), currentTile, maxDistance, jump, playerControlled);
//            nextIndex = currentIndex + TileIndex.left;
//            AddIfValid(ref sortedList, mEncounterMap.GetTileAt(nextIndex), currentTile, maxDistance, jump, playerControlled);
//        }
//    }

//    private void AddIfValid(ref List<KeyValuePair<MapTile,int>> stack, MapTile tileToCheck, MapTile previousTile, int maxDistance, float maxJump, bool playerControlled)
//    {
//        // Tile is outside map boundaries
//        if (tileToCheck == null)
//        {
//            return;
//        }
//        // Tile is above/below max jump distance
//        float elevationDifference = Mathf.Abs(tileToCheck.transform.localPosition.y - previousTile.transform.localPosition.y);
//        if (elevationDifference > maxJump + ElevationDifferenceBuffer)
//        {
//            return;
//        }

//        int costToMoveToTile = (int)Mathf.Abs(1 + elevationDifference);

//        if (mPotentialMapPaths.ContainsKey(tileToCheck))//has tile already been reached?
//        {
//            int newDistance = mPotentialMapPaths[previousTile].pathLength + costToMoveToTile;
//            int oldDistance = mPotentialMapPaths[tileToCheck].pathLength;
//            if (oldDistance > newDistance)// shorter path found
//            {
//                TilePath path = mPotentialMapPaths[tileToCheck];
//                path.previous = previousTile;
//                path.pathLength = newDistance;
//                mPotentialMapPaths[tileToCheck] = path;

//                // if found a shorter path to tile, check if needs to be added to potential movement tiles
//                if (oldDistance > maxDistance && newDistance <= maxDistance)
//                {
//                    mPotentialMovementTiles.Add(tileToCheck.Id, tileToCheck);
//                }
//            }
//            //else potential route is a longer path. Discard

//            return;
                    
//        }
//        else // new path found
//        {
//            // is there anyone currently on the tile?
//            CharacterManager onTile = tileToCheck.GetCharacterOnTile();
//            if (onTile != null)
//            {
//                //is the unit friendly?
//                if (!(onTile.IsPlayerControlled ^ playerControlled) || !onTile.IsAlive)// !(different teams)
//                {
//                    //not a valid tile, but valid path
//                    mPotentialMapPaths.Add(tileToCheck, new TilePath(mPotentialMapPaths[previousTile].pathLength + 1, previousTile, false));
//                }
//                else //cant move through this tile. Discard
//                {
//                    return;
//                }
//            }
//            else
//            {
//                mPotentialMapPaths.Add(tileToCheck, new TilePath(mPotentialMapPaths[previousTile].pathLength + 1, previousTile, true));
//            }
//        }

//        if (mPotentialMapPaths[tileToCheck].pathLength <= maxDistance && mPotentialMapPaths[tileToCheck].isValidEndTile)
//        {
//            mPotentialMovementTiles.Add(tileToCheck.Id, tileToCheck);
//        }

//        stack.Add(new KeyValuePair<MapTile,int>(tileToCheck, mPotentialMapPaths[tileToCheck].pathLength));
//    }

//    public void ResetMovementTiles()
//    {
//        mPotentialMapPaths.Clear();
//        mPotentialMovementTiles.Clear();
//    }
//}
