using MAGE.GameModes.Encounter;
using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAGE.GameSystems
{
    class Map
    {
        private const string TAG = "Map";
        public Dictionary<Character, CharacterPosition> CharacterPositionLookup;
        public HashSet<Character> CharactersInMap;
        public List<List<Tile>> mTiles;
        public int Width = 0;
        public int Length = 0;

        MovementTileCalculator mMovementCalculator;
        ActionTileCalculator mActionCalculator;

        public Map()
        {
            CharacterPositionLookup = new Dictionary<Character, CharacterPosition>();
            CharactersInMap = new HashSet<Character>();
            mMovementCalculator = new MovementTileCalculator(this);
            mActionCalculator = new ActionTileCalculator(this);
        }

        public void InitTiles(List<List<Tile>> tiles)
        {
            mTiles = tiles;
            Length = mTiles.Count;
            Width = Length > 0 ? mTiles[0].Count : 0;
        }

        public void AddCharacter(Character character, CharacterPosition position)
        {
            Logger.Assert(!CharactersInMap.Contains(character), LogTag.Map, TAG, string.Format("AddCharacter() - {0} has already been added to map", character.Name));
            if (!CharactersInMap.Contains(character))
            {
                CharactersInMap.Add(character);
                CharacterPositionLookup.Add(character, position);
                TileAt(position.Location).OnTile = character;
            }
        }

        public void UpdatePosition(Character character, CharacterPosition position)
        {
            Logger.Assert(CharactersInMap.Contains(character), LogTag.Map, TAG, string.Format("UpdatePosition() - {0} doesn't exist in map", character.Name));
            if (CharactersInMap.Contains(character))
            {
                TileAt(CharacterPositionLookup[character].Location).OnTile = null;
                CharacterPositionLookup[character] = position;
                TileAt(position.Location).OnTile = character;
            }
        }

        public CharacterPosition GetCharacterPosition(Character character)
        {
            CharacterPosition characterPosition = new CharacterPosition();

            Logger.Assert(CharactersInMap.Contains(character), LogTag.Map, TAG, string.Format("GetCharacterPosition() - {0} doesn't exist in map", character.Name));
            if (CharactersInMap.Contains(character))
            {
                characterPosition = CharacterPositionLookup[character];
            }

            return characterPosition;
        }

        public float DistanceBetween(Character lhs, Character rhs)
        {
            return TileIdx.DistanceBetween(CharacterPositionLookup[lhs].Location, CharacterPositionLookup[rhs].Location);
        }

        public float DistanceBetween(TileIdx lhs, TileIdx rhs)
        {
            return TileIdx.DistanceBetween(lhs, rhs);
        }

        public Vector2 DisplacementBetween(Character lhs, Character rhs)
        {
            return TileIdx.Displacement(CharacterPositionLookup[lhs].Location, CharacterPositionLookup[rhs].Location);
        }

        public Vector2 DisplacementBetween(TileIdx lhs, TileIdx rhs)
        {
            return TileIdx.Displacement(lhs, rhs);
        }

        public float ElevationDifference(TileIdx lhs, TileIdx rhs)
        {
            return TileAt(lhs).Elevation - TileAt(rhs).Elevation;
        }

        public List<Character> GetTargetedCharacters(CharacterPosition casterPosition, TargetSelection targetSelection)
        {
            List<Character> onTiles = new List<Character>();

            TileIdx centralTile = new TileIdx();
            if (targetSelection.FocalTarget.TargetType == TargetSelectionType.Character)
            {
                centralTile = GetCharacterPosition(targetSelection.FocalTarget.CharacterTarget).Location;
            }
            else if (targetSelection.FocalTarget.TargetType == TargetSelectionType.Tile)
            {
                centralTile = targetSelection.FocalTarget.TileTarget;
            }

            List<Tile> tiles = mActionCalculator.CalculateTilesInRange(casterPosition.Location, centralTile, targetSelection.SelectionRange);
            foreach (Tile tile in tiles)
            {
                if (tile.OnTile != null)
                {
                    onTiles.Add(tile.OnTile);
                }
            }

            return onTiles;
        }

        public List<Tile> GetMovementTilesForCharacter(Character character)
        {
            TileIdx currentLocation = CharacterPositionLookup[character].Location;
             mMovementCalculator.CalculatePathTiles(
                currentLocation
                , (int)character.CurrentAttributes[TertiaryStat.Movement]
                , (int)character.CurrentAttributes[TertiaryStat.Jump]
                , character.TeamSide);

            return mMovementCalculator.GetValidMovementTiles();
        }

        public Tile TileAt(TileIdx tileIdx)
        {
            return mTiles[tileIdx.y][tileIdx.x];
        }

        public bool IsValidIdx(TileIdx idx)
        {
            return !(idx.x < 0 || idx.x >= Width || idx.y < 0 || idx.y >= Length);
        }
    }
}

