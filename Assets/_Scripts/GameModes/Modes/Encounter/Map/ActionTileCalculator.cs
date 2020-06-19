using System.Collections.Generic;
using UnityEngine;

class ActionTileCalculator
{
    private Map mMap = null;

    public ActionTileCalculator(Map map)
    {
        mMap = map;
    }

    public List<Tile> CalculateTilesInRange(TileIdx centerTile, RangeInfo rangeInfo)
    {
        List<Tile> tilesInRange = new List<Tile>();
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
            default:
                Debug.Assert(false);
                break;
        }
        return tilesInRange;
    }

    private List<Tile> CalculateCircleTiles(TileIdx center, int minRange, int maxRange, int maxElevation)
    {
        List<Tile> tiles = new List<Tile>();

        Tile centerTile = mMap[center];

        for (int z = 0; z < mMap.Length; z++)
        {
            for (int x = 0; x < mMap.Width; x++)
            {
                TileIdx tileIdx = new TileIdx(x, z);

                Tile tile = mMap.Relative(tileIdx);

                float distanceToTile = centerTile.ManhattanDistance(tile);
                float elevationDifference = centerTile.ElevationDifference(tile);
                if (distanceToTile >= minRange
                    && distanceToTile <= maxRange
                    && Mathf.Abs(elevationDifference) <= maxElevation)
                {
                    tiles.Add(tile);
                }
            }
            
        }
        return tiles;
    }

    private List<Tile> CalculateExpandingTiles(TileIdx center, int minRange, int maxRange, int maxElevation)
    {
        List<Tile> tiles = new List<Tile>();

        Tile centerTile = mMap[center];

        for (int z = 0; z < mMap.Length; z++)
        {
            for (int x = 0; x < mMap.Width; x++)
            {
                TileIdx tileIdx = new TileIdx(x, z);

                Tile tile = mMap.Relative(tileIdx);

                float distanceToTile = centerTile.ManhattanDistance(tile);
                float elevationDifference = centerTile.ElevationDifference(tile);

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
                    tiles.Add(tile);
                }
            }
            
        }
        return tiles;
    }

    //private void CalculateCrossTiles(MapTile origin, ref Dictionary<int, MapTile> tileMap, int minRange, int maxRange, int maxElevation, int elevationModifier = 1, bool localized = true /* actionarea*/)
    //{
    //    tileMap.Clear();

    //    Vector3 centerTile = origin.transform.localPosition;
    //    List<Vector3> directions = new List<Vector3>() { Vector3.forward, Vector3.back, Vector3.right, Vector3.left };
    //    Debug.Log("center elevation " + centerElevation);


    //    foreach (Vector3 direction in directions)
    //    {
    //        int tileCount = 1;
    //        MapTile nextInLine = null;
    //        while ((nextInLine = MapManager.Instance.GetTileAt(centerTile + direction * (tileCount))) != null)
    //        {
    //            if (tileCount > maxRange)
    //            {
    //                break;
    //            }
    //            else if (tileCount >= minRange)
    //            {
    //                if (Mathf.Abs(nextInLine.transform.position.y - centerTile.y) < maxElevation)
    //                {
    //                    tileMap.Add(nextInLine.Id, nextInLine);
    //                }
    //            }
    //            tileCount++;
    //        }
    //    }
    //    if (minRange == 0)
    //    {
    //        tileMap.Add(origin.Id, origin);
    //    }
    //}

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
    //    Vector3 center = origin.transform.localPosition;

    //    float centerElevation = origin.GetTileCenter().y;
    //    foreach (var row in mEncounterMap.MapTiles)
    //    {
    //        foreach (var tile in row)
    //        {
    //            Vector3 displacement = center - tile.transform.localPosition;
    //            float distance = Mathf.Sqrt(displacement.x * displacement.x + displacement.z * displacement.z);
    //            if (distance >= minRange)
    //            {
    //                float elevationDifference = center.y - displacement.y;

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


