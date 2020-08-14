using MAGE.GameModes.Encounter;
using MAGE.GameServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
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

}