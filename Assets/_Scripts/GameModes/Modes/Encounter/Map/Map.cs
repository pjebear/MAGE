using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Map : MonoBehaviour
{
    public Tile TilePrefab;
    public int Width = 4;
    public int Length = 4;

    public Transform TileContainer;

    public Dictionary<ActorController, Tile> ActorPositionLookup;
    public HashSet<ActorController> ActorsInMap;
    public Tile[,] Tiles;

    public static Map Instance;

    Tile this[TileIdx idx]
    {
        get
        {
            return Tiles[idx.y, idx.x];
        }
        set
        {
            Tiles[idx.y, idx.x] = value;
        }
    }

    private void Awake()
    {
        ActorPositionLookup = new Dictionary<ActorController, Tile>();
        ActorsInMap = new HashSet<ActorController>();

        Tile[] tiles = GetComponentsInChildren<Tile>();
        int size = (int)Mathf.Sqrt(tiles.Length);
        Width = size;
        Length = size;

        Tiles = new Tile[Length, Width];

        for (int i = 0; i < TileContainer.childCount; ++i)
        {
            Transform row = TileContainer.GetChild(i);

            int z = (int)row.localPosition.z;
            foreach (Tile tile in row.GetComponentsInChildren<Tile>())
            {
                int x = (int)tile.transform.localPosition.x;
                Tiles[z, x] = tile;

                TileIdx index = new TileIdx(x, z);
                tile.Init(index);
            }
        }

        Instance = this;
    }

    public void Initialize()
    {
        Tiles = new Tile[Length, Width];
        for (int z = 0; z < Length; z++)
        {
            GameObject row = new GameObject("row " + z);
            row.transform.SetParent(transform);
            row.transform.Translate(Vector3.forward * z);

            for (int x = 0; x < Width; ++x)
            {
                TileIdx forTile = new TileIdx(x, z);
                this[forTile] = Instantiate(TilePrefab, row.transform, false);
                this[forTile].transform.localPosition = Vector3.right * x;

                this[forTile].Init(forTile);
            }
        }
    }

    public void PlaceAtTile(TileIdx tileIdx, ActorController actor)
    {
        // Clear previous tile
        if (ActorPositionLookup.ContainsKey(actor))
        {
            ActorPositionLookup[actor].OnTile = null;
        }

        Debug.Assert(this[tileIdx].OnTile == null);
        Tile atIdx = this[tileIdx];
        atIdx.PlaceAtCenter(actor);
        ActorPositionLookup[actor] = atIdx; 
    }

    public Tile GetActorsTile(EncounterCharacter actor)
    {
        ActorController controller = EncounterModule.ActorDirector.ActorControllerLookup[actor];

        return ActorPositionLookup[controller];
    }

    public List<EncounterCharacter> GetActors(TargetSelection targetSelection)
    {
        List<EncounterCharacter> onTiles = new List<EncounterCharacter>();

        if (targetSelection.FocalTarget.TargetType == TargetSelectionType.Actor)
        {
            onTiles.Add(targetSelection.FocalTarget.ActorTarget);
        }
        else if (targetSelection.FocalTarget.TargetType == TargetSelectionType.Tile)
        {
            EncounterCharacter onTile = this[targetSelection.FocalTarget.TileTarget].OnTile.mActor;
            if (onTile != null)
            {
                onTiles.Add(onTile);
            }
        }

        foreach (Target target in targetSelection.PeripheralTargets)
        {
            if (target.TargetType == TargetSelectionType.Actor)
            {
                onTiles.Add(target.ActorTarget);
            }
            else if (target.TargetType == TargetSelectionType.Tile)
            {
                EncounterCharacter onTile = this[target.TileTarget].OnTile.mActor;
                if (onTile != null)
                {
                    onTiles.Add(onTile);
                }
            }
        }

        return onTiles;
    }

    public TileSelection GetMovementTilesForActor(EncounterCharacter actor)
    {
        Tile actorsTile = GetActorsTile(actor);

        TileSelection selection = GetTilesInRange(actorsTile.Idx, (int)actor.Attributes[AttributeCategory.Stat][(int)TertiaryStat.Movement].Value);

        selection.SelectionType = Tile.HighlightState.MovementSelect;

        return selection;
    }

    public TileSelection GetTilesInRange(TileIdx centerPoint, int range)
    {
        List<Tile> inRange = new List<Tile>();

        for (int i = 0; i < Length; ++i)
        {
            for (int j = 0; j < Width; ++j)
            {
                if (Tiles[i,j].Idx != centerPoint
                    && TileIdx.Distance(Tiles[i,j].Idx, centerPoint) <= range)
                {
                    inRange.Add(Tiles[i, j]);
                }
            }
        }

        return new TileSelection(inRange, Tile.HighlightState.None);
    }
}
