using System.Collections.Generic;
using UnityEngine;
namespace EncounterSystem.Map
{
    using MapTypes;
    using MapEnums;

   
    class ActionTileCalculator
    {
        private EncounterMap mEncounterMap = null;
        private MapTile mAOEOrigin = null;
        private Dictionary<int, MapTile> mActionAOETiles;
        private Dictionary<int, MapTile> mActionTiles;
        // both ranges are inclusive

        public ActionTileCalculator(EncounterMap encounterMap)
        {
            mEncounterMap = encounterMap;
            mActionAOETiles = new Dictionary<int, MapTile>();
            mActionTiles = new Dictionary<int, MapTile>();
        }
        public void CalculateActionTiles(MapTile actorOrigin, MapInteractionInfo interactionInfo)
        {
            mActionTiles.Clear();
            switch (interactionInfo.ActionAreaType)
            {
                case (TileAreaType.Circle):
                    CalculateCircleTiles(actorOrigin, ref mActionTiles, interactionInfo.MinRange, interactionInfo.MaxRange, interactionInfo.MaxRangeElevation);
                    break;
                case (TileAreaType.Cross):
                    CalculateCrossTiles(actorOrigin, ref mActionTiles, interactionInfo.MinRange, interactionInfo.MaxRange, interactionInfo.MaxRangeElevation);
                    break;
                case (TileAreaType.Ring):
                    CalculateRingTiles(actorOrigin, ref mActionTiles, interactionInfo.MinRange, interactionInfo.MaxRange, interactionInfo.MaxRangeElevation);
                    break;
                default:
                    Debug.LogError("Invalid action tile selection provided. Got " + interactionInfo.ActionAreaType.ToString());
                    break;
            }
        }

        public void CalculateActionAOETiles(MapTile aoeOrigin, MapTile actorOrigin, MapInteractionInfo interactionInfo)
        {
            mActionAOETiles.Clear();
            mAOEOrigin = aoeOrigin;
            switch (interactionInfo.AOEAreaType)
            {
                case (TileAreaType.Circle):
                    CalculateCircleTiles(aoeOrigin, ref mActionAOETiles, interactionInfo.MinAoe, interactionInfo.MaxAoe, interactionInfo.MaxAoeElevation);
                    break;
                case (TileAreaType.Cone):
                    CalculateConeTiles(aoeOrigin, actorOrigin, ref mActionAOETiles, interactionInfo.MinAoe, interactionInfo.MaxAoe, interactionInfo.MaxAoeElevation);
                    break;
                case (TileAreaType.Line):
                    CalculateLineTiles(actorOrigin, aoeOrigin, ref mActionAOETiles, interactionInfo.MinAoe, interactionInfo.MaxAoe, interactionInfo.MaxAoeElevation);
                    break;
                default:
                    Debug.LogError("Invalid AOE tile selection provided. Got " + interactionInfo.AOEAreaType.ToString());
                    break;
            }
                
        }

        public List<MapTile> GetActionTiles()
        {
            return GetTiles(mActionTiles);
        }

        public MapTile GetAOEOrigin()
        {
            return mAOEOrigin;
        }

        public List<MapTile> GetActionAOETiles()
        {
            return GetTiles(mActionAOETiles);
        }

        private List<MapTile> GetTiles(Dictionary<int,MapTile> map)
        {
            List<MapTile> tiles = new List<MapTile>();
            foreach (var tile in map.Values)
            {
                tiles.Add(tile);
            }
            return tiles;
        }

        public bool IsValidActionTile(MapTile selection)
        {
            return mActionTiles.ContainsKey(selection.Id);
        }

        private void CalculateCircleTiles(MapTile origin, ref Dictionary<int, MapTile> tileMap, int minRange, int maxRange, int maxElevation, int elevationModifier = 1, bool localized = true /* actionarea*/)
        {
            tileMap.Clear();
            TileIndex center = origin.GetLocalMapIndex();
            TileIndex index;

            bool isAoe = tileMap == mActionAOETiles;
                
            float centerElevation = origin.GetTileCenter().y;
            //Debug.Log ("center elevation " + centerElevation);
            for (int x = -maxRange; x <= maxRange; x++)
            {
                int rowHeight = maxRange - Mathf.Abs(x);
                for (int z = -rowHeight; z <= rowHeight; z++)
                {
                    int distance = Mathf.Abs(x) + Mathf.Abs(z);
                    if (distance >= minRange)
                    {
                        index.x = center.x + x;
                        index.y = center.y + z;
                        MapTile tile = mEncounterMap.GetTileAt(index);
                        if (tile != null)
                        {
                            if (Mathf.Abs(tile.GetTileCenter().y - centerElevation) <= maxElevation)
                            {
                                tileMap.Add(tile.Id, tile);
                            }
                        }
                    }
                }
            }
        }

        private void CalculateCrossTiles(MapTile origin, ref Dictionary<int, MapTile> tileMap, int minRange, int maxRange, int maxElevation, int elevationModifier = 1, bool localized = true /* actionarea*/)
        {
            tileMap.Clear();

            Vector3 centerTile = origin.transform.localPosition;
            List<Vector3> directions = new List<Vector3>() { Vector3.forward, Vector3.back, Vector3.right, Vector3.left };
            //Debug.Log ("center elevation " + centerElevation);


            foreach( Vector3 direction in directions)
            {
                int tileCount = 1;
                MapTile nextInLine = null;
                while ((nextInLine = MapManager.Instance.GetTileAt(centerTile + direction * (tileCount))) != null)
                {
                    if (tileCount > maxRange)
                    {
                        break;
                    }
                    else if (tileCount >= minRange)
                    {
                        if (Mathf.Abs(nextInLine.transform.position.y - centerTile.y ) < maxElevation)
                        {
                            tileMap.Add(nextInLine.Id, nextInLine);
                        }
                    }
                    tileCount++;
                }
            }
            if (minRange == 0)
            {
                tileMap.Add(origin.Id, origin);
            }
        }

        private void CalculateLineTiles(MapTile lineOrigin, MapTile lineTarget, ref Dictionary<int, MapTile> tileMap, int minRange, int maxRange, int maxElevation, int elevationModifier = 1, bool localized = true /* actionarea*/)
        {
            tileMap.Clear();

            Vector3 centerTile = lineOrigin.transform.localPosition;
            Vector3 direction = lineTarget.transform.localPosition - centerTile;
            Debug.Assert(direction != Vector3.zero, "Recieved Zero length direction vector for line tile calculation");
            direction.y = 0f;
            direction.Normalize();

            int tileCount = 0;
            MapTile nextInLine = null;
            while ((nextInLine = MapManager.Instance.GetTileAt(centerTile + direction * (tileCount))) != null)
            {
                if (tileCount > maxRange)
                {
                    break;
                }
                else if (tileCount >= minRange)
                {
                    if (Mathf.Abs(nextInLine.transform.position.y - centerTile.y) < maxElevation)
                    {
                        tileMap.Add(nextInLine.Id, nextInLine);
                    }
                }
                tileCount++;
            }
        }

        // Cone Origin indicates the tile the user selected for the cone AOE to be spawned. the tip of the cone may start behind the cone origin but only tiles in the direction of the cone will be added. 
        private void CalculateConeTiles(MapTile coneOrigin, MapTile actorOrigin, ref Dictionary<int, MapTile> tileMap, int coneStart, int coneEnd, int maxElevation, int elevationModifier = 1)
        {
            tileMap.Clear();
            Vector3 coneDirection = coneOrigin.transform.localPosition - actorOrigin.transform.localPosition;
            coneDirection.y = 0;
            coneDirection.Normalize();
            Vector3 widthDirection;

            if (coneDirection.z != 0) // up or down
            {
                widthDirection = Vector3.right;
            }
            else // left or right
            {
                widthDirection = Vector3.back;
            }
                
            int rowNumber = 0;
            for (int coneLength = coneStart; coneLength <= coneEnd; coneLength++)
            {
                if (coneLength >= 0) // don't add tiles behind cone origin
                {
                    Vector3 coneSpine = coneOrigin.transform.localPosition + coneDirection * coneLength;
                    // add @coneWidth tiles along the cones row at [@coneLength] to the list
                    for (int offsetFromSpine = -rowNumber; offsetFromSpine <= rowNumber; offsetFromSpine++)
                    {
                        MapTile potentialTile = MapManager.Instance.GetTileAt(coneSpine + widthDirection * offsetFromSpine);
                        if (potentialTile != null)
                        {
                            tileMap.Add(potentialTile.Id, potentialTile);
                        }
                    }
                }
                rowNumber++;
            }

        }

        private void CalculateRingTiles(MapTile origin, ref Dictionary<int, MapTile> tileMap, int minRange, int maxRange, int maxElevation, int elevationModifier = 1, bool localized = true /* actionarea*/)
        {
            Vector3 center = origin.transform.localPosition;

            float centerElevation = origin.GetTileCenter().y;
            foreach (var row in mEncounterMap.MapTiles)
            {
                foreach (var tile in row)
                {
                    Vector3 displacement = center - tile.transform.localPosition;
                    float distance = Mathf.Sqrt(displacement.x * displacement.x + displacement.z * displacement.z);
                    if (distance >= minRange)
                    {
                        float elevationDifference = center.y - displacement.y;

                        float lobbingDistance = Mathf.Floor(distance + elevationDifference / 2f);

                        if (lobbingDistance <= maxRange)
                        {
                            tileMap.Add(tile.Id, tile);
                        }
                    }
                }
            }
        }

        public void ResetActionTiles()
        {
            mActionTiles.Clear();
        }

        public void ResetActionAOETiles()
        {
            mActionAOETiles.Clear();
            mAOEOrigin = null;
        }
    }
}
    
