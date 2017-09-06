using System;
using System.Collections.Generic;
using UnityEngine;

namespace EncounterSystem.Map
{
    using Character;
    using Common.AttributeEnums;
    using MapEnums;
    using MapTypes;

    class MapManager
    {
        public static MapManager Instance = null;
        private GameObject mMapContainerOffset;
        private EncounterMap mEncounterMap = null;
        private MapTile CursorTile = null;
        private MovementTileCalculator mMovementTileCalculator = null;
        private ActionTileCalculator mActionTileCalculator = null;
        private UnitPlacementCalculator mUnitPlacementCalculator = null;

        public int TEMP_GETMAPLENGTH { get { return mEncounterMap.MapLength; } }
        public bool FinishedInitialization { get { return mEncounterMap.finishedInitialization; } }

        public MapManager(string mapAssetId)
        {
            mMapContainerOffset = new GameObject("MapContainer");
            mMapContainerOffset.transform.SetParent(GameObject.Find("Canvas").transform);
            EncounterMap map = (UnityEngine.Object.Instantiate(Resources.Load(mapAssetId)) as GameObject).GetComponent<EncounterMap>();
            map.gameObject.transform.parent = mMapContainerOffset.transform;
            mEncounterMap = map;
            mMovementTileCalculator = new MovementTileCalculator(map);
            mActionTileCalculator = new ActionTileCalculator(map);
            mUnitPlacementCalculator = new UnitPlacementCalculator(map);
        }

        public void Initialize()
        {
            mEncounterMap.Initialize();
            mMapContainerOffset.transform.Rotate(new Vector3(0, -45, 0));
            Instance = this;
        }

        public void CalculateUnitPlacementTiles(int zone)
        {
            mUnitPlacementCalculator.CalculateStartingZoneTiles(zone);
        }

        public void DisplayUnitPlacementTiles(bool visible)
        {
            mUnitPlacementCalculator.DisplayStartingZoneTiles(visible);
        }

        public List<MapTile> GetTilesForStartingZone(int zone)
        {
            return mEncounterMap.GetTilesForStartingZone(zone);
        }

        public bool IsValidUnitPlacementTile(MapTile tile)
        {
            return mUnitPlacementCalculator.IsValidStartingTile(tile);
        }

        #region MovementTileAPI 

        public void CalculatePathTiles(CharacterManager character, bool forCinematic)
        {
            int maxDistance = forCinematic ? 100 : (int)character.Stats[(int)TertiaryStat.Movement];
            float maxJump = forCinematic ? 100 : character.Stats[(int)TertiaryStat.Jump];
            mMovementTileCalculator.CalculatePathTiles(character.GetCurrentTile(), maxDistance, maxJump, character.IsPlayerControlled);
        }

        public List<MapTile> GetValidMovementTiles()
        {
            return mMovementTileCalculator.GetValidMovementTiles();
        }

        public Dictionary<MapTile, TilePath> GetValidMapPaths()
        {
            return mMovementTileCalculator.GetMapPaths();
        }

        public void DisplayMovementTiles(bool display)
        {
            foreach (MapTile tile in mMovementTileCalculator.GetValidMovementTiles())
            {
                tile.SetTileColor(display ? MapConstants.MoveTileColor : Color.clear);
            }
        }

        public void RotateMap(float scrollDelta)
        {
            mMapContainerOffset.transform.Rotate(Vector3.up * scrollDelta * Time.deltaTime * 90f);
        }

        public void ClearMovementTiles()
        {
            mMovementTileCalculator.ResetMovementTiles();
        }

        public bool IsValidMoveTile(MapTile tile)
        {
            return mMovementTileCalculator.IsValidMoveTile(tile);
        }

        public Stack<MapTile> GetMovementPathTo(MapTile destination)
        {
            return mMovementTileCalculator.GetPathTo(destination);
        }
        #endregion

        #region ActionTileAPI

        public void CalculateActionTiles(MapTile castTile, MapInteractionInfo actionInfo)
        {
            mActionTileCalculator.CalculateActionTiles(castTile, actionInfo);
        }
        public void CalculateActionAOETiles(MapTile aoeOrigin, MapTile castTile, MapInteractionInfo actionInfo)
        {
            mActionTileCalculator.CalculateActionAOETiles(aoeOrigin, castTile, actionInfo);
        }

        public MapTile GetAOEOrigin()
        {
            return mActionTileCalculator.GetAOEOrigin();
        }

        public void DisplayActionTiles(bool display)
        {
            Color toSet = display ? MapConstants.ActionTileColor : Color.clear;
            foreach (MapTile tile in mActionTileCalculator.GetActionTiles())
            {
                tile.SetTileColor(toSet);
            }
        }

        public void DisplayActionAOETiles(bool display)
        {
            Color toSet = display ? MapConstants.ActionAOETileColor : Color.clear;
            foreach (MapTile tile in mActionTileCalculator.GetActionAOETiles())
            {
                tile.SetTileColor(toSet);
            }
        }

        public bool IsValidActionTile(MapTile tile)
        {
            return mActionTileCalculator.IsValidActionTile(tile);
        }

        public List<MapTile> GetActionTiles()
        {
            return mActionTileCalculator.GetActionTiles();
        }

        public List<MapTile> GetActionAOETiles()
        {
            return mActionTileCalculator.GetActionAOETiles();
        }

        public void ResetActionAOETiles()
        {
            mActionTileCalculator.ResetActionAOETiles();
        }

        public void ResetActionTiles()
        {
            mActionTileCalculator.ResetActionTiles();
        }
        #endregion

        //----------------------------------- Helpers ---------------------------------------------

        public MapTile GetTileAt(int index)
        {
            if (mEncounterMap.MapTileLookup.ContainsKey(index))
            {
                return mEncounterMap.MapTileLookup[index];
            }
            return null;
        }

        public MapTile GetTileAt(CharacterManager character)
        {
            return GetTileAt(character.transform.localPosition);
        }

        public MapTile GetTileAt(Vector3 location)
        {
            
            return GetTileAt(Mathf.RoundToInt(location.x), Mathf.RoundToInt(location.z));
        }

        private MapTile GetTileAt(int x, int y)
        {
            return mEncounterMap.GetTileAt(new TileIndex(x, y));
        }

        public void AddCharacter(CharacterManager character)
        {
            character.transform.SetParent(mEncounterMap.transform);
        }

        public void PlaceCharacterAtRandomSpawnTile(CharacterManager character, int zone)
        {
            character.transform.SetParent(mEncounterMap.transform);
            character.UpdateCurrentTile(mUnitPlacementCalculator.GetRandomStartingTile(zone), true);
        }

        public bool AddCharacterAt(CharacterManager character, int x, int y)
        {
            character.transform.SetParent(mEncounterMap.transform);
            TileIndex index = new TileIndex(x, y);

            if (mEncounterMap.InRange(index))
            {
                MapTile tile = mEncounterMap.GetTileAt(index);
                if (tile.GetCharacterOnTile() == null)
                {
                    character.UpdateCurrentTile(tile, true);
                    return true;
                }
                else
                {
                    Debug.LogError("Character already on Tile");
                }
            }
            else
            {
                Debug.LogError("Tile Out of range");
            }
            return false;
        }

    }
}
