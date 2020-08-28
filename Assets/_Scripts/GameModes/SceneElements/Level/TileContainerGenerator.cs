using MAGE.GameSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    class TileContainerGenerator : MonoBehaviour
    {
        public TileControl TilePrefab;

        public TileContainer GenerateTiles(Transform centerTileLocation, int width, int length, Transform parent = null)
        {
            TileContainer tileContainer = new GameObject("TileContainer").AddComponent<TileContainer>();
            if (parent != null)
            {
                tileContainer.transform.SetParent(parent);
            }

            Vector3 bottomLeft = new Vector3(centerTileLocation.position.x - width / 2, 0, centerTileLocation.position.z - length / 2);

            for (int z = 0; z < length; ++z)
            {
                GameObject tileRow = new GameObject("Row " + z.ToString());
                tileRow.transform.SetParent(tileContainer.transform);
                tileContainer.Tiles.Add(new List<TileControl>());

                for (int x = 0; x < width; ++x)
                {
                    Vector3 tileCenter = bottomLeft + new Vector3(x, 1000, z);

                    Ray ray = new Ray(tileCenter, Vector3.down);

                    int layerMask = 1 << (int)RayCastLayer.Terrain;
                    RaycastHit terrainHit;
                    if (Physics.Raycast(ray, out terrainHit, 2000, layerMask))
                    {
                        Vector3 rayHit = terrainHit.point + Vector3.up * .1f; // slightly off ground
                        Vector3 normal = terrainHit.normal;

                        TileControl toAdd = Instantiate(TilePrefab, rayHit, Quaternion.identity);
                        toAdd.transform.SetParent(tileRow.transform);
                        toAdd.transform.up = normal;
                        tileContainer.Tiles[z].Add(toAdd);

                        RaycastHit treeHit;
                        if (Physics.CheckBox(rayHit, new Vector3(.5f, .5f, .5f), Quaternion.identity, 1 << (int)RayCastLayer.Trees, QueryTriggerInteraction.Ignore))
                        {
                            toAdd.SetHighlightState(TileControl.HighlightState.TargetSelect);
                        }
                    }
                }
            }

            return tileContainer;
        }
    }
}


