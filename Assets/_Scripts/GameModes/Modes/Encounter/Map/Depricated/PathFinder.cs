
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameSystems.EncounterSystem
{
    namespace Map
    {
        //class PriorityQueue<Key, Value>
        //{
        //    List<List<Value>> mPriorityQueue;
        //    public PriorityQueue()
        //    {
        //        mPriorityQueue = new List<List<Value>>();
        //    }

        //    public void Add(Key key, Value value)
        //    {
        //        List<Value> queue;
        //        if (mPriorityQueue.TryGetValue(key, out queue))
        //        {
        //            queue.Add(value);
        //        }
        //        else
        //        {
        //            List<Value> newQueue = new List<Value>() { value };
        //            mPriorityQueue.Add(key, newQueue);
        //        }
        //    }

        //    public bool GetNext(out Value value)
        //    {
        //        if (mPriorityQueue.Count > 0)
        //        {
        //            mPriorityQueue.
        //        }
        //        else
        //        {
        //            value = null;
        //            return false;
        //        }
        //    }
        //}

        //class PathFinder
        //{
        //    private EncounterMap mEncounterMap;
        //    const int UNTESTED = 0, OPEN = 1, CLOSED = 2;
        //    float[] gScore;
        //    float[] fScore;
        //    MapTile[] parents;
        //    List<int> openSet;
        //    Dictionary<int,MapTile> closedSet;
        //    bool mPlayerControlled;
        //    MapTile mGoal;

        //    public PathFinder(EncounterMap map)
        //    {
        //        mEncounterMap = map;
        //        int arraySize = map.MapWidth * map.MapLength;
        //        gScore = new float[arraySize];
        //        fScore = new float[arraySize];
        //        parents = new MapTile[arraySize];
        //        openSet = new List<int >();
        //        closedSet = new Dictionary<int, MapTile>();
        //        Initialize();
        //    }

        //    public bool FindPath(MapTile start, MapTile goal, float maxElevationChange, ref List<MapTile> pathTo)
        //    {
        //        Initialize();
        //        mGoal = goal;
        //        mPlayerControlled = start.GetCharacterOnTile() != null && start.GetCharacterOnTile().IsPlayerControlled;
        //        gScore[start.Id] = 0;
        //        fScore[start.Id] = heuristic_calculation(start, goal);
        //        openSet.Add(start.Id);
        //        openSet.Sort(delegate (int i, int j)
        //        {
        //            float fi = fScore[i], fj = fScore[j];
        //            if (fi < fj) return -1;
        //            else if (fi == fj) return 0;
        //            else return 1;
        //        });

        //        while (openSet.Count() > 0)
        //        {
        //            int currentIndex = openSet[0];
        //            MapTile current = mEncounterMap.MapTileLookup[currentIndex];
        //            if (current == goal)
        //            {
        //                reconstruct_path(start, current, ref pathTo);
        //                return true;
        //            }
        //            openSet.Remove(currentIndex);
        //            closedSet.Add(current.Id, current);
        //            List<MapTile> neighbours = GetAdjacentTiles(current, maxElevationChange);
        //            foreach (var neighbour in neighbours)
        //            {
        //                if (closedSet.ContainsKey(neighbour.Id))
        //                    continue;
        //                float gTemp = gScore[current.Id] + traversal_calculation(current, neighbour);

        //                if (!openSet.Contains(neighbour.Id))
        //                    openSet.Add(neighbour.Id);
        //                else if (gTemp >= gScore[neighbour.Id])
        //                    continue;
        //                parents[neighbour.Id] = current;
        //                gScore[neighbour.Id] = gTemp;
        //                fScore[neighbour.Id] = gTemp + heuristic_calculation(neighbour, goal);
        //            }

        //            openSet.Sort(delegate (int i, int j)
        //            {
        //                return fScore[i].CompareTo(fScore[j]);
        //            });
        //        }
        //        return false;
        //    }

        //    private List<MapTile> GetAdjacentTiles(MapTile centerTile, float maxElevationChange)
        //    {
        //        TileIndex centerPosition = centerTile.GetLocalMapIndex();
        //        TileIndex[] adjacentPositions = { centerPosition + TileIndex.up, centerPosition + TileIndex.down, centerPosition + TileIndex.left, centerPosition + TileIndex.right };
        //        List<MapTile> adjacentTiles = new List<MapTile>();
        //        foreach (var position in adjacentPositions)
        //        {
        //            if (mEncounterMap.InRange(position))
        //            {
        //                MapTile tile = mEncounterMap.MapTiles[position.y][position.x];
        //                bool isValidTile = true;

        //                if (mGoal != tile 
        //                    && tile.GetCharacterOnTile() != null
        //                    && (tile.GetCharacterOnTile().IsPlayerControlled ^ mPlayerControlled))
        //                {
        //                    isValidTile &= false;
        //                }

        //                if ((tile.transform.position.y - centerTile.transform.position.y) > maxElevationChange)
        //                {
        //                    isValidTile &= false;
        //                }
        //                if (isValidTile)
        //                {
        //                    adjacentTiles.Add(tile);
        //                }
        //            }
        //        }
        //        return adjacentTiles;
        //    }
        //    private void reconstruct_path(MapTile start, MapTile goal, ref List<MapTile> pathTo)
        //    {
        //        MapTile parent = parents[goal.Id];
        //        while (parent != null && parent != start)
        //        {
        //            pathTo.Add(parent);
        //            parent = parents[parent.Id];
        //        }
        //    }

        //    void Initialize()
        //    {
        //        gScore = gScore.Select(i => float.PositiveInfinity).ToArray();
        //        fScore = fScore.Select(i => float.PositiveInfinity).ToArray();
        //        parents = parents.Select<MapTile, MapTile>(i => null).ToArray();
        //        openSet.Clear();
        //        closedSet.Clear();
        //    }
        //    float heuristic_calculation(MapTile from, MapTile to)
        //    {
        //        return (to.transform.position - from.transform.position).sqrMagnitude;
        //    }
        //    float traversal_calculation(MapTile from, MapTile to)
        //    {
        //        return Vector3.Distance(from.transform.position, to.transform.position);
        //    }

        }
    }
    

