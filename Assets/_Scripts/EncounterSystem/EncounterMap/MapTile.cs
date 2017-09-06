
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EncounterSystem.MapTypes
{
    using Character;
    public class MapTile : MonoBehaviour
    {
        private CharacterManager mCharacterOnTile = null;
        private int mId;
        public int Id { get { return mId; } set { mId = value; } }
        MeshRenderer mMeshRenderer;
        private void Awake()
        {
            mMeshRenderer = GetComponentInChildren<MeshRenderer>();
            if (mMeshRenderer == null) Debug.LogError("MapTile: No mesh Renderer attached to child");
        }

        public void SetTileColor(Color color)
        {
            mMeshRenderer.material.color = color;
        }
        public void ResetTile()
        {
            mMeshRenderer.material.color = Color.clear;
        }

        public Vector3 GetTileCenter()
        {

            return transform.GetChild(0).transform.position;
        }

        public TileIndex GetLocalMapIndex()
        {
            return new TileIndex((int)transform.localPosition.x, (int)transform.localPosition.z);
        }

        public void SetCharacterOnTile(CharacterManager onTile)
        {
            if (mCharacterOnTile != null)
            {
                Debug.LogError("Attempting to assign character to tile with a character already on it. Tile:" + GetLocalMapIndex());
            }
            else
            {
                mCharacterOnTile = onTile;
            }
        }

        public void PlaceCharacterAtTileCenter()
        {
            if (mCharacterOnTile != null)
            {
                mCharacterOnTile.transform.position = GetTileCenter();
            }
        }

        public CharacterManager RemoveCharacterFromTile()
        {
            CharacterManager toRemove = mCharacterOnTile;
            mCharacterOnTile = null;
            return toRemove;
        }

        public CharacterManager GetCharacterOnTile()
        {
            return mCharacterOnTile;
        }
    }
}
