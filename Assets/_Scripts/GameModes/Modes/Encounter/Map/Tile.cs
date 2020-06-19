﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TileIdx
{
    public int x;
    public int y;

    public TileIdx(int _x, int _y)
    {
        x = _x;
        y = _y;
    }


    public static int ManhattanDistance(TileIdx lhs, TileIdx rhs)
    {
        int xDiff = Mathf.Abs(lhs.x - rhs.x);
        int yDiff = Mathf.Abs(lhs.y - rhs.y);

        return xDiff + yDiff;
    }

    public static Vector2 Displacement(TileIdx lhs, TileIdx rhs)
    {
        return new Vector2(rhs.x - lhs.x, rhs.y - lhs.y);
    }

    public static bool operator==(TileIdx lhs, TileIdx rhs)
    {
        return lhs.x == rhs.x && lhs.y == rhs.y;
    }

    public static bool operator !=(TileIdx lhs, TileIdx rhs)
    {
        return !(lhs == rhs);
    }
}

class Tile : MonoBehaviour
{
    public enum HighlightState
    {
        None,

        MovementSelect,
        TargetSelect,
        AOESelect,

        NUM
    }
    public HighlightState State { get; private set; }

    public TextMesh TextMesh;
    public TextMesh OnTileName;
    public Material[] Materials;
    public MeshRenderer Highlight;

    public EncounterActorController OnTile;
    public TileIdx Idx;

    private void Awake()
    {
        SetHighlightState(HighlightState.None);
    }

    public void Init(TileIdx tileIdx)
    {
        Idx = tileIdx;
        RefreshName();
    }

    private void RefreshName()
    {
        TextMesh.text = string.Format("[{0},{1}] \n{2}", Idx.y, Idx.x, OnTile != null ? OnTile.EncounterCharacter.Name : "EMPTY");
        name = string.Format("[{0},{1}]", Idx.y, Idx.x);
    }

    public int ManhattanDistance(Tile otherTile)
    {
        return TileIdx.ManhattanDistance(Idx, otherTile.Idx);
    }

    public float DistanceTo(Tile otherTile)
    {
        return Vector3.Distance(transform.localPosition, otherTile.transform.localPosition);
    }

    public float ElevationDifference(Tile otherTile)
    {
        return transform.position.y - otherTile.transform.position.y;
    }

    public void PlaceAtCenter(EncounterActorController actor)
    {
        OnTile = actor;
        actor.transform.position = transform.position;
        RefreshName();
    }

    public void ClearOnTile()
    {
        OnTile = null;
        RefreshName();
    }

    public void SetHighlightState(HighlightState state)
    {
        State = state;

        int matIdx = (int)state;

        Highlight.material = Materials[matIdx];
    }
}
