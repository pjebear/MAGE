using System.Collections;
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


    public static int Distance(TileIdx lhs, TileIdx rhs)
    {
        int xDiff = Mathf.Abs(lhs.x - rhs.x);
        int yDiff = Mathf.Abs(lhs.y - rhs.y);

        return xDiff + yDiff;
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
    public Material[] Materials;
    public MeshRenderer Highlight;

    public ActorController OnTile;
    public TileIdx Idx;

    private void Awake()
    {
        SetHighlightState(HighlightState.None);
    }

    public void Init(TileIdx tileIdx)
    {
        Idx = tileIdx;
        TextMesh.text = string.Format("[{0},{1}]", Idx.x, Idx.y);
    }

    public float DistanceTo(Tile otherTile)
    {
        return Vector3.Distance(transform.localPosition, otherTile.transform.localPosition);
    }

    public void PlaceAtCenter(ActorController actor)
    {
        OnTile = actor;
        actor.transform.position = transform.position;
    }

    public void SetHighlightState(HighlightState state)
    {
        State = state;

        int matIdx = (int)state;

        Highlight.material = Materials[matIdx];
    }
}
