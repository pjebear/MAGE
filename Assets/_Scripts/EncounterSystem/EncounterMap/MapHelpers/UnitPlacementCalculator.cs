using EncounterSystem.MapEnums;
using EncounterSystem.MapTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EncounterSystem.Map
{

    class UnitPlacementCalculator
    {
        private Dictionary<int,MapTile> mDisplayedZoneTiles;
        private EncounterMap mEncounterMap;

        public UnitPlacementCalculator(EncounterMap encounterMap)
        {
            mEncounterMap = encounterMap;
            mDisplayedZoneTiles = new Dictionary<int, MapTile>();
        }

        public void CalculateStartingZoneTiles(int zone)
        {
            mDisplayedZoneTiles.Clear();
            foreach (MapTile tile in mEncounterMap.GetTilesForStartingZone(zone))
            {
                mDisplayedZoneTiles.Add(tile.Id, tile);
            }
        }

        public void DisplayStartingZoneTiles(bool visible)
        {
            foreach(MapTile tile in mDisplayedZoneTiles.Values)
            {
                tile.SetTileColor(visible && tile.GetCharacterOnTile() == null ? 
                    MapConstants.UnitPlacementTileColor : UnityEngine.Color.clear);
            }
        }

        public bool IsValidStartingTile(MapTile tile)
        {
            return mDisplayedZoneTiles.ContainsKey(tile.Id);
        }

        public MapTile GetRandomStartingTile(int zone)
        {
            List<MapTile> potentialTiles = new List<MapTile>();
            foreach (int tileId in (zone == 0 ? mEncounterMap.StartingZoneTilesA : mEncounterMap.StartingZoneTilesB))
            {
                if (mEncounterMap.MapTileLookup[tileId].GetCharacterOnTile() == null)
                {
                    potentialTiles.Add(mEncounterMap.MapTileLookup[tileId]);
                }
            }
            UnityEngine.Debug.Assert(potentialTiles.Count > 0);
            int randomIndex = UnityEngine.Random.Range(0, potentialTiles.Count);
            return potentialTiles[randomIndex];
        }
    }
}
