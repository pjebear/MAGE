﻿using MAGE.GameModes.Encounter;
using MAGE.GameServices;
using MAGE.GameServices.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    class Map
    {
        public TileIdx TileIdxOffset = new TileIdx(0, 0);

        public int Width = 1;
        public int Length = 1;
        public Dictionary<EncounterActorController, Tile> ActorPositionLookup;
        public HashSet<EncounterActorController> ActorsInMap;
        public Tile[,] Tiles;

        MovementTileCalculator mMovementCalculator;
        ActionTileCalculator mActionCalculator;

        public Tile this[TileIdx idx]
        {
            get
            {
                int x = idx.x - TileIdxOffset.x;
                int z = idx.y - TileIdxOffset.y;
                Logger.Assert(x >= 0 && x < Width && z >= 0 && z < Length, LogTag.GameModes, "Map", string.Format("Inavlid TileIdx{0}{1}", idx.x, idx.y));
                return Tiles[z, x];
            }
            set
            {
                Tiles[idx.y - TileIdxOffset.y, idx.x - TileIdxOffset.x] = value;
            }
        }

        public Map()
        {
            ActorPositionLookup = new Dictionary<EncounterActorController, Tile>();
            ActorsInMap = new HashSet<EncounterActorController>();
            mMovementCalculator = new MovementTileCalculator(this);
            mActionCalculator = new ActionTileCalculator(this);
        }

        public void Initialize(TileContainer tileContainer, TileIdx bottomLeft, TileIdx topRight)
        {
            TileIdxOffset = bottomLeft;
            Vector2 tileRange = TileIdx.Displacement(bottomLeft, topRight);
            Width = (int)tileRange.x + 1;
            Length = (int)tileRange.y + 1;

            Tiles = new Tile[Length, Width];

            for (int z = 0; z < Length; z++)
            {
                for (int x = 0; x < Width; ++x)
                {
                    TileIdx tileIdx = new TileIdx(bottomLeft.x + x, bottomLeft.y + z);

                    Tiles[z, x] = tileContainer.Tiles[bottomLeft.y + z][bottomLeft.x + x];
                    Tiles[z, x].gameObject.SetActive(true);
                }
            }
        }

        public void Cleanup()
        {
            for (int z = 0; z < Length; z++)
            {
                for (int x = 0; x < Width; ++x)
                {
                    Tiles[z, x].gameObject.SetActive(false);
                    Tiles[z, x].SetHighlightState(Tile.HighlightState.None);
                }
            }
        }

        public void PlaceAtTile(TileIdx tileIdx, EncounterActorController actor)
        {
            // Clear previous tile
            if (ActorPositionLookup.ContainsKey(actor))
            {
                ActorPositionLookup[actor].ClearOnTile();
            }

            Debug.Assert(this[tileIdx].OnTile == null);
            Tile atIdx = this[tileIdx];
            atIdx.PlaceAtCenter(actor);
            ActorPositionLookup[actor] = atIdx;
        }

        public Tile GetActorsTile(EncounterCharacter actor)
        {
            EncounterActorController controller = EncounterModule.CharacterDirector.CharacterActorLookup[actor];

            return ActorPositionLookup[controller];
        }

        public List<EncounterCharacter> GetActors(TileIdx casterTile, TargetSelection targetSelection)
        {
            List<EncounterCharacter> onTiles = new List<EncounterCharacter>();

            TileIdx centralTile = new TileIdx();
            if (targetSelection.FocalTarget.TargetType == TargetSelectionType.Actor)
            {
                centralTile = GetActorsTile(targetSelection.FocalTarget.ActorTarget).Idx;
            }
            else if (targetSelection.FocalTarget.TargetType == TargetSelectionType.Tile)
            {
                centralTile = targetSelection.FocalTarget.TileTarget;
            }

            List<Tile> tiles = mActionCalculator.CalculateTilesInRange(casterTile, centralTile, targetSelection.SelectionRange);
            foreach (Tile tile in tiles)
            {
                if (tile.OnTile != null)
                {
                    onTiles.Add(tile.OnTile.EncounterCharacter);
                }
            }

            return onTiles;
        }

        public List<Tile> GetMovementTilesForActor(EncounterCharacter actor)
        {
            Tile actorsTile = GetActorsTile(actor);

            RangeInfo movementRangeInfo = new RangeInfo(
                1,
                (int)actor.Attributes[AttributeCategory.Stat][(int)TertiaryStat.Movement].Value,
                (int)actor.Attributes[AttributeCategory.Stat][(int)TertiaryStat.Jump].Value,
                AreaType.Circle);

            List<Tile> movementTiles = GetTilesInRange(actorsTile.Idx, movementRangeInfo);

            return movementTiles;
        }

        public List<Tile> GetTilesInRange(TileIdx centerPoint, RangeInfo rangeInfo)
        {
            List<Tile> inRange = new List<Tile>();

            for (int i = 0; i < Length; ++i)
            {
                for (int j = 0; j < Width; ++j)
                {
                    int manhattanDistance = TileIdx.ManhattanDistance(Tiles[i, j].Idx, centerPoint);
                    if (manhattanDistance >= rangeInfo.MinRange && manhattanDistance <= rangeInfo.MaxRange)
                    {
                        inRange.Add(Tiles[i, j]);
                    }
                }
            }

            return inRange;
        }

        public Tile Relative(TileIdx tileIdx)
        {
            return Tiles[tileIdx.y, tileIdx.x];
        }

        public Tile Absolute(TileIdx tileIdx)
        {
            return this[TruncateIdx(tileIdx)];
        }

        public TileIdx TruncateIdx(TileIdx idx)
        {
            return new TileIdx(idx.x - TileIdxOffset.x, idx.y - TileIdxOffset.y);
        }

        public bool IsValidIdx(TileIdx idx)
        {
            TileIdx truncated = TruncateIdx(idx);
            return !(truncated.x < 0 || truncated.x >= Width || truncated.y < 0 || truncated.y >= Length);
        }
    }
}

