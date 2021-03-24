using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using System.Collections.Generic;
using UnityEngine;

namespace MAGE.GameSystems
{
    class ActionTileCalculator
    {
        private Map mMap = null;

        public ActionTileCalculator(Map map)
        {
            mMap = map;
        }

        public List<TileIdx> CalculateTilesInRange(TileIdx casterTile, TileIdx centerTile, RangeInfo rangeInfo, TeamSide teamSide)
        {
            List<TileIdx> tilesInRange = new List<TileIdx>();
            switch (rangeInfo.AreaType)
            {
                case AreaType.Circle:
                {
                    tilesInRange = CalculateCircleTiles(centerTile, rangeInfo.MinRange, rangeInfo.MaxRange, rangeInfo.MaxElevationChange);
                }
                break;
                case AreaType.Expanding:
                {
                    tilesInRange = CalculateExpandingTiles(centerTile, rangeInfo.MinRange, rangeInfo.MaxRange, rangeInfo.MaxElevationChange);
                }
                break;
                case AreaType.Cone:
                {
                    tilesInRange = CalculateConeTiles(casterTile, centerTile, rangeInfo.MaxRange, rangeInfo.MaxElevationChange);
                }
                break;
                case AreaType.Cross:
                {
                    tilesInRange = CalculateCrossTiles(centerTile, rangeInfo.MinRange, rangeInfo.MaxRange, rangeInfo.MaxElevationChange);
                }
                break;
                default:
                    Debug.Assert(false);
                    break;
            }

            tilesInRange.RemoveAll(x => !DoesIdxMatchFilter(x, rangeInfo.TargetingType, mMap, teamSide));

            return tilesInRange;
        }

        private List<TileIdx> CalculateCircleTiles(TileIdx centerIdx, int minRange, int maxRange, int maxElevation)
        {
            List<TileIdx> tiles = new List<TileIdx>();

            for (int z = 0; z < mMap.Length; z++)
            {
                for (int x = 0; x < mMap.Width; x++)
                {
                    TileIdx tileIdx = new TileIdx(x, z);

                    float distanceToTile = mMap.DistanceBetween(centerIdx, tileIdx);
                    float elevationDifference = mMap.ElevationDifference(centerIdx, tileIdx);
                    if (distanceToTile >= minRange
                        && distanceToTile <= maxRange
                        && Mathf.Abs(elevationDifference) <= maxElevation)
                    {
                        tiles.Add(tileIdx);
                    }
                }

            }
            return tiles;
        }

        private List<TileIdx> CalculateExpandingTiles(TileIdx centerIdx, int minRange, int maxRange, int maxElevation)
        {
            List<TileIdx> tiles = new List<TileIdx>();

            for (int z = 0; z < mMap.Length; z++)
            {
                for (int x = 0; x < mMap.Width; x++)
                {
                    TileIdx tileIdx = new TileIdx(x, z);

                    float distanceToTile = mMap.DistanceBetween(centerIdx, tileIdx);
                    float elevationDifference = mMap.ElevationDifference(centerIdx, tileIdx);

                    float relativeMinRange = minRange;
                    float relativeMaxRange = maxRange;
                    float relativeMaxElevation = maxElevation;

                    if (elevationDifference < 0) // Target is below caster
                    {
                        relativeMinRange = minRange + (0.5f * elevationDifference);
                        relativeMaxRange = maxRange + (1.5f * elevationDifference);
                        relativeMaxElevation = maxElevation + (-elevationDifference);
                    }

                    if (distanceToTile >= relativeMinRange
                        && distanceToTile <= relativeMaxRange
                        && Mathf.Abs(elevationDifference) <= relativeMaxElevation)
                    {
                        tiles.Add(tileIdx);
                    }
                }

            }
            return tiles;
        }

        //// Cone Origin indicates the tile the user selected for the cone AOE to be spawned. the tip of the cone may start behind the cone origin but only tiles in the direction of the cone will be added. 
        private List<TileIdx> CalculateConeTiles(TileIdx casterTile, TileIdx coneOrigin, int maxRange, int maxElevation)
        {
            List<TileIdx> tiles = new List<TileIdx>();

            Vector2 coneDirection = TileIdx.Displacement(casterTile, coneOrigin);

            coneDirection.Normalize();
            Vector2 widthDirection;

            if (coneDirection.y != 0) // up or down
            {
                widthDirection = Vector2.right;
            }
            else // left or right
            {
                widthDirection = Vector2.up;
            }

            int rowNumber = 0;
            for (int coneLength = 0; coneLength <= maxRange; coneLength++)
            {
                if (coneLength >= 0) // don't add tiles behind cone origin
                {
                    Vector2 spine = coneDirection * coneLength;

                    // add @coneWidth tiles along the cones row at [@coneLength] to the list
                    for (int offsetFromSpine = -rowNumber; offsetFromSpine <= rowNumber; offsetFromSpine++)
                    {
                        Vector2 rib = widthDirection * offsetFromSpine;
                        Vector2 tileOffset = spine + rib;
                        TileIdx potentialTile = new TileIdx(Mathf.RoundToInt(coneOrigin.x + tileOffset.x), Mathf.RoundToInt(coneOrigin.y + tileOffset.y));
                        if (mMap.IsValidIdx(potentialTile))
                        {   
                            if (Mathf.Abs(mMap.ElevationDifference(coneOrigin, potentialTile)) < maxElevation)
                            {
                                tiles.Add(potentialTile);
                            }
                        }
                    }
                }
                rowNumber++;
            }
            return tiles;
        }

        private List<TileIdx> CalculateCrossTiles(TileIdx centerIdx, int minRange, int maxRange, int maxElevation)
        {
            List<TileIdx> tiles = new List<TileIdx>();

            for (int z = 0; z < mMap.Length; z++)
            {
                int width = z == centerIdx.y ? 1 : mMap.Width;
                for (int x = 0; x < width; x++)
                {
                    TileIdx tileIdx = new TileIdx(x, z);

                    float distanceToTile = mMap.DistanceBetween(centerIdx, tileIdx);
                    float elevationDifference = mMap.ElevationDifference(centerIdx, tileIdx);
                    if (distanceToTile >= minRange
                        && distanceToTile <= maxRange
                        && Mathf.Abs(elevationDifference) <= maxElevation)
                    {
                        tiles.Add(tileIdx);
                    }
                }
            }
            return tiles;
        }

        private bool DoesIdxMatchFilter(TileIdx idx, TargetingType filter, Map map, TeamSide teamSide)
        {
            bool matchesFilter = true;

            Character onTile = map.TileAt(idx).OnTile;

            switch (filter)
            {
                case TargetingType.Any:         matchesFilter = true; break;
                case TargetingType.Allies:      matchesFilter = onTile == null || onTile.TeamSide == teamSide; break;
                case TargetingType.Enemies:     matchesFilter = onTile == null || onTile.TeamSide != teamSide; break;
                case TargetingType.Empty:       matchesFilter = onTile == null;     break;
                default: Debug.Assert(false);   matchesFilter = false; break;
            }

            return matchesFilter;
        }

        //private void CalculateLineTiles(MapTile lineOrigin, MapTile lineTarget, ref Dictionary<int, MapTile> tileMap, int minRange, int maxRange, int maxElevation, int elevationModifier = 1, bool localized = true /* actionarea*/)
        //{
        //    tileMap.Clear();

        //    Vector3 centerTile = lineOrigin.transform.localPosition;
        //    Vector3 direction = lineTarget.transform.localPosition - centerTile;
        //    Debug.Assert(direction != Vector3.zero, "Recieved Zero length direction vector for line tile calculation");
        //    direction.y = 0f;
        //    direction.Normalize();

        //    int tileCount = 0;
        //    MapTile nextInLine = null;
        //    while ((nextInLine = MapManager.Instance.GetTileAt(centerTile + direction * (tileCount))) != null)
        //    {
        //        if (tileCount > maxRange)
        //        {
        //            break;
        //        }
        //        else if (tileCount >= minRange)
        //        {
        //            if (Mathf.Abs(nextInLine.transform.position.y - centerTile.y) < maxElevation)
        //            {
        //                tileMap.Add(nextInLine.Id, nextInLine);
        //            }
        //        }
        //        tileCount++;
        //    }
        //}

        //// Cone Origin indicates the tile the user selected for the cone AOE to be spawned. the tip of the cone may start behind the cone origin but only tiles in the direction of the cone will be added. 
        //private void CalculateConeTiles(MapTile coneOrigin, MapTile actorOrigin, ref Dictionary<int, MapTile> tileMap, int coneStart, int coneEnd, int maxElevation, int elevationModifier = 1)
        //{
        //    tileMap.Clear();
        //    Vector3 coneDirection = coneOrigin.transform.localPosition - actorOrigin.transform.localPosition;
        //    coneDirection.y = 0;
        //    coneDirection.Normalize();
        //    Vector3 widthDirection;

        //    if (coneDirection.z != 0) // up or down
        //    {
        //        widthDirection = Vector3.right;
        //    }
        //    else // left or right
        //    {
        //        widthDirection = Vector3.back;
        //    }

        //    int rowNumber = 0;
        //    for (int coneLength = coneStart; coneLength <= coneEnd; coneLength++)
        //    {
        //        if (coneLength >= 0) // don't add tiles behind cone origin
        //        {
        //            Vector3 coneSpine = coneOrigin.transform.localPosition + coneDirection * coneLength;
        //            // add @coneWidth tiles along the cones row at [@coneLength] to the list
        //            for (int offsetFromSpine = -rowNumber; offsetFromSpine <= rowNumber; offsetFromSpine++)
        //            {
        //                MapTile potentialTile = MapManager.Instance.GetTileAt(coneSpine + widthDirection * offsetFromSpine);
        //                if (potentialTile != null)
        //                {
        //                    tileMap.Add(potentialTile.Id, potentialTile);
        //                }
        //            }
        //        }
        //        rowNumber++;
        //    }

        //}

        //private void CalculateRingTiles(MapTile origin, ref Dictionary<int, MapTile> tileMap, int minRange, int maxRange, int maxElevation, int elevationModifier = 1, bool localized = true /* actionarea*/)
        //{
        //    Vector3 centerIdx = origin.transform.localPosition;

        //    float centerElevation = origin.GetTileCenter().y;
        //    foreach (var row in mEncounterMap.MapTiles)
        //    {
        //        foreach (var tile in row)
        //        {
        //            Vector3 displacement = centerIdx - tile.transform.localPosition;
        //            float distance = Mathf.Sqrt(displacement.x * displacement.x + displacement.z * displacement.z);
        //            if (distance >= minRange)
        //            {
        //                float elevationDifference = centerIdx.y - displacement.y;

        //                float lobbingDistance = Mathf.Floor(distance + elevationDifference / 2f);

        //                if (lobbingDistance <= maxRange)
        //                {
        //                    tileMap.Add(tile.Id, tile);
        //                }
        //            }
        //        }
        //    }
        //}
    }
}




