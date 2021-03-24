using MAGE.GameModes.Encounter;
using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Stats;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MAGE.GameSystems
{
    class Map
    {
        private const string TAG = "Map";
        public Dictionary<Character, CharacterPosition> CharacterPositionLookup;
        public HashSet<Character> CharactersInMap;
        public List<List<Tile>> mTiles;
        public List<List<TileConnections>> mTileConnections;
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

        public void Init(List<List<Tile>> tiles, List<List<TileConnections>> connections)
        {
            mTiles = tiles;
            mTileConnections = connections;
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

        public List<TileIdx> GetTargetedTiles(TileIdx casterLocation, TargetSelection targetSelection)
        {
            TileIdx centralTile = new TileIdx();
            if (targetSelection.FocalTarget.TargetType == TargetSelectionType.Character)
            {
                centralTile = GetCharacterPosition(targetSelection.FocalTarget.CharacterTarget).Location;
            }
            else if (targetSelection.FocalTarget.TargetType == TargetSelectionType.Tile)
            {
                centralTile = targetSelection.FocalTarget.TileTarget;
            }

            TeamSide teamSide = TeamSide.INVALID;
            Character onTile = TileAt(casterLocation).OnTile;
            Debug.Assert(onTile != null);
            if (onTile != null)
            {
                teamSide = onTile.TeamSide;
            }

            return mActionCalculator.CalculateTilesInRange(casterLocation, centralTile, targetSelection.SelectionRange, teamSide);
        }

        public List<Character> GetTargetedCharacters(CharacterPosition casterPosition, TargetSelection targetSelection)
        {
            TileIdx centralTile = new TileIdx();
            if (targetSelection.FocalTarget.TargetType == TargetSelectionType.Character)
            {
                centralTile = GetCharacterPosition(targetSelection.FocalTarget.CharacterTarget).Location;
            }
            else if (targetSelection.FocalTarget.TargetType == TargetSelectionType.Tile)
            {
                centralTile = targetSelection.FocalTarget.TileTarget;
            }

            TeamSide teamSide = TeamSide.INVALID;
            Character onTile = TileAt(casterPosition.Location).OnTile;
            Debug.Assert(onTile != null);
            if (onTile != null)
            {
                teamSide = onTile.TeamSide;
            }

            List<TileIdx> tiles = mActionCalculator.CalculateTilesInRange(casterPosition.Location, centralTile, targetSelection.SelectionRange, teamSide);

            return GetCharactersOnTiles(tiles);
        }

        public List<Character> GetCharactersOnTiles(List<TileIdx> tiles)
        {
            List<Character> onTiles = new List<Character>();
            foreach (TileIdx tile in tiles)
            {
                Character onTile = TileAt(tile).OnTile;
                if (onTile != null)
                {
                    onTiles.Add(onTile);
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

        public List<Tile> GetTiles(List<TileIdx> indices)
        {
            List<Tile> tiles = new List<Tile>();
            foreach (TileIdx idx in indices)
            {
                tiles.Add(TileAt(idx));
            }

            return tiles;
        }

        public TileIdx GetFocalPositionForTeam(TeamSide teamSide)
        {
            float averageX = 0;
            float averageY = 0;
            int numCharacters = 0;
            foreach (var characterPosPair in CharacterPositionLookup.Where(x => x.Key.TeamSide == teamSide))
            {
                TileIdx location = characterPosPair.Value.Location;
                averageX += location.x;
                averageY += location.y;
                ++numCharacters;
            }

            TileIdx focalPoint = new TileIdx();
            if (numCharacters > 0)
            {
                focalPoint.x = Mathf.RoundToInt(averageX / (float)numCharacters);
                focalPoint.y = Mathf.RoundToInt(averageY / (float)numCharacters);
            }

            return focalPoint;
        }

        public Tile TileAt(TileIdx tileIdx)
        {
            return mTiles[tileIdx.y][tileIdx.x];
        }

        public bool IsValidIdx(TileIdx idx)
        {
            return !(idx.x < 0 || idx.x >= Width || idx.y < 0 || idx.y >= Length);
        }

        public bool IsPathObstructed(TileIdx idx, Orientation toSide)
        {
            return !mTileConnections[idx.y][idx.x].Connections[(int)toSide];
        }
    }
}

