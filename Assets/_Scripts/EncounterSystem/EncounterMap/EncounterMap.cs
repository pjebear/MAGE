using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EncounterSystem
{
    namespace Map
    {
        using MapTypes;
        class EncounterMap : MonoBehaviour
        {
            public int[] StartingZoneTilesA;
            public int[] StartingZoneTilesB;
            public int[] StartingZoneTilesC;

            public int MapWidth { get; private set; }
            public int MapLength { get; private set; }
            public GameObject MapTilePrefab = null;
            private List<List<MapTile>> mMapTiles;
            private Dictionary<int, MapTile> mMapTileLookup;
            public Dictionary<int, MapTile> MapTileLookup { get { return mMapTileLookup; } }
            public List<List<MapTile>> MapTiles { get { return mMapTiles; } }

            public bool finishedInitialization = false;
            // Use this for initialization

            void Awake()
            {
                Debug.Log("EncounterMap: Loading Render Map...");
                mMapTiles = new List<List<MapTile>>();
                mMapTileLookup = new Dictionary<int, MapTile>();

                //hack
                {
                    MapWidth = 10;
                    MapLength = 10;
                }
            }

            public void Initialize()
            {
                Debug.Log("EncounterMap: Creating map Tiles");
                Transform tileContainer = transform.GetChild(0); //
                int tileIndices = -1;
                for (int i = 0; i < MapLength; i++)
                {
                    mMapTiles.Add(new List<MapTile>());
                    for (int j = 0; j < MapWidth; j++)
                    {
                        RaycastHit hitOut;
                        Vector3 origin = new Vector3(j + 0.5f, 10, i + 0.5f);
                        if (Physics.Raycast(origin, Vector3.down, out hitOut, 100f))
                        {
                            Debug.DrawLine(origin, hitOut.point, Color.red, 1000f);
                            GameObject go = (Instantiate(MapTilePrefab, tileContainer) as GameObject);
                            go.transform.localPosition = new Vector3(j, hitOut.point.y, i);
                            go.transform.up = hitOut.normal;

                            MapTile tile = go.GetComponent<MapTile>();
                            ++tileIndices;
                            tile.Id = tileIndices;
                            mMapTiles[i].Add(tile);
                            mMapTileLookup.Add(tileIndices, tile);
                        }
                    }
                }
                gameObject.transform.position = new Vector3(-(float)MapWidth / 2f, 0, -(float)MapLength / 2f);
                Debug.Log("EncounterMap: Finished Initialization");
                finishedInitialization = true;
                //load render map
            }

            public bool InRange(TileIndex index)
            {
                return !(index.x < 0 || index.x >= MapWidth || index.y < 0 || index.y >= MapLength);
            }

            public MapTile GetTileAt(TileIndex index)
            {
                if (InRange(index))
                    return MapTiles[index.y][index.x];
                return null;
            }

            public List<MapTile> GetTilesForStartingZone(int zone)
            {
                int[] startingTileIds = null;
                switch (zone)
                {
                    default:
                    case (0):
                        startingTileIds = StartingZoneTilesA;
                        break;

                    case (1):
                        startingTileIds = StartingZoneTilesB;
                        break;

                    case (2):
                        startingTileIds = StartingZoneTilesC;
                        break;
                }
                int numStartingTiles = startingTileIds.Length;
                Debug.Assert(numStartingTiles > 0);

                List<MapTile> startingTiles = new List<MapTile>();
                for (int i = 0; i < numStartingTiles; ++i)
                {
                    Debug.Assert(mMapTileLookup.ContainsKey(startingTileIds[i]));
                    startingTiles.Add(mMapTileLookup[startingTileIds[i]]);
                }
                return startingTiles;
            }
        }
    }
}

