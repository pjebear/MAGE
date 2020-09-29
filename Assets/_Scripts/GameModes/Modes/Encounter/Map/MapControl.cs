using MAGE.GameModes.Encounter;
using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    class MapControl
    {
        public Map Map;
        public Dictionary<Tile, TileControl> TileControlLookup = new Dictionary<Tile, TileControl>();

        public TileControl this[TileIdx idx]
        {
            get
            {
                return TileControlLookup[Map.TileAt(idx)];
            }
        }

        public void Initialize(TileContainer tileContainer)
        {
            TileControlLookup.Clear();

            int width = tileContainer.Tiles.Count;
            int length = tileContainer.Tiles[0].Count;

            List<List<Tile>> tileGrid = new List<List<Tile>>();

            for (int z = 0; z < length; z++)
            {
                List<Tile> tileRow = new List<Tile>();
                for (int x = 0; x < width; ++x)
                {
                    TileControl tileControl = tileContainer.Tiles[z][x];

                    Tile tile = new Tile(new TileIdx(x,z), tileControl.gameObject.transform.position.y, tileContainer.Obstructions[z][x]);
                    tileControl.Init(tile);
                    tileControl.gameObject.SetActive(true);
                    TileControlLookup.Add(tile, tileControl);

                    tileRow.Add(tile);
                }

                tileGrid.Add(tileRow);
            }

            Map = new Map();
            Map.Init(tileGrid, tileContainer.Connections);
        }

        public void Cleanup()
        {
            foreach (TileControl control in TileControlLookup.Values)
            {
                control.gameObject.SetActive(false);
                control.SetHighlightState(TileControl.HighlightState.None);
            }
        }

        public void AddCharacterToMap(CharacterActorController characterActorController, CharacterPosition atPosition, bool placeAtTile = true)
        {
            Map.AddCharacter(characterActorController.Character, atPosition);
            if (placeAtTile)
            {
                this[atPosition.Location].PlaceAtCenter(characterActorController);
                this[atPosition.Location].Refresh();
            }
        }

        public void UpdateCharacterPosition(CharacterActorController characterActorController, CharacterPosition atPosition)
        {
            CharacterPosition currentPosition = Map.CharacterPositionLookup[characterActorController.Character];
            this[currentPosition.Location].ClearOnTile();

            Map.UpdatePosition(characterActorController.Character, atPosition);

            this[atPosition.Location].PlaceAtCenter(characterActorController);
            this[atPosition.Location].Refresh();

            characterActorController.transform.forward = OrientationUtil.ToVector(atPosition.Orientation);
        }

        public List<TileControl> GetTiles(List<Tile> tiles)
        {
            List<TileControl> tileControls = new List<TileControl>();
            foreach (Tile tile in tiles)
            {
                tileControls.Add(TileControlLookup[tile]);
            }
            return tileControls;
        }

        public List<TileControl> GetTiles(List<TileIdx> indices)
        {
            List<TileControl> tileControls = new List<TileControl>();
            foreach (Tile tile in Map.GetTiles(indices))
            {
                tileControls.Add(TileControlLookup[tile]);
            }
            return tileControls;
        }

        public CharacterActorController GetOnTile(TileControl tile)
        {
            CharacterActorController onTile = null;
            Character character = Map.TileAt(tile.Idx).OnTile;
            if (character != null)
            {
                onTile = EncounterFlowControl.CharacterDirector.CharacterActorLookup[character];
            }
            return onTile;
        }

        public Transform GetTargetTransform(Target target)
        {
            Transform transform = null;

            if (target.TargetType == TargetSelectionType.Character)
            {
                Character character = target.CharacterTarget;
                transform = EncounterFlowControl.CharacterDirector.CharacterActorLookup[character].transform;
            }
            else
            {
                transform = TileControlLookup[Map.TileAt(target.TileTarget)].transform;
            }

            return transform;
        }

        public TileControl GetClosestTileTo(Vector3 position)
        {
            TileControl closestTile = null;
            float closestDistance = float.MaxValue;
            foreach (TileControl tileControl in TileControlLookup.Values)
            {
                float distanceTo = Vector3.Magnitude(tileControl.transform.position - position);
                if (distanceTo < closestDistance)
                {
                    closestDistance = distanceTo;
                    closestTile = tileControl;
                }
            }

            if (closestDistance > 1)
            {
                Debug.Assert(false);
            }

            return closestTile;
        }
    }
}

