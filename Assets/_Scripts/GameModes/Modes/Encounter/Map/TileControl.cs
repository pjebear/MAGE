using MAGE.GameModes.Encounter;
using MAGE.GameSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    class TileControl : MonoBehaviour
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
        public List<GameObject> BorderHighlights = new List<GameObject>();
        public TileIdx Idx { get { return mTile.TileIdx; } }

        private Tile mTile;

        private void Awake()
        {
            SetHighlightState(HighlightState.None);
        }

        public void Init(Tile tile)
        {
            mTile = tile;
            Refresh();
        }

        public void PlaceAtCenter(CharacterActorController actor)
        {
            actor.transform.position = transform.position;
        }

        public void ClearOnTile()
        {
            mTile.OnTile = null;
            RefreshName();
        }

        public void Refresh()
        {
            RefreshName();
        }

        public void SetHighlightState(HighlightState state)
        {
            State = state;

            int matIdx = (int)state;

            Highlight.material = Materials[matIdx];
        }

        // private
        private void RefreshName()
        {
            TextMesh.text = string.Format("[{0},{1}] \n{2}", Idx.y, Idx.x, mTile.OnTile != null ? mTile.OnTile.Name : "EMPTY");
            name = string.Format("[{0},{1}]", Idx.y, Idx.x);
        }
    }

}