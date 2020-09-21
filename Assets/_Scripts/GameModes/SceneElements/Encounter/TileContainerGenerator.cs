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
                tileContainer.Connections.Add(new List<TileConnections>());
                tileContainer.Obstructions.Add(new List<bool>());

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

                        bool isObstructed = Physics.CheckBox(rayHit, new Vector3(.5f, .5f, .5f), Quaternion.identity, 1 << (int)RayCastLayer.Trees, QueryTriggerInteraction.Ignore);
                        if (isObstructed)
                        {
                            toAdd.SetHighlightState(TileControl.HighlightState.TargetSelect);
                        }
                        tileContainer.Obstructions[z].Add(isObstructed);

                        TileConnections tileConnections = new TileConnections();
                        tileConnections.Connections[(int)Orientation.Forward] =
                            !Physics.CheckBox(rayHit + Vector3.forward * .5f, new Vector3(.25f, 10f, .25f), Quaternion.identity, 1 << (int)RayCastLayer.Trees, QueryTriggerInteraction.Ignore);
                        toAdd.BorderHighlights[(int)Orientation.Forward].SetActive(!tileConnections.Connections[(int)Orientation.Forward]);

                        tileConnections.Connections[(int)Orientation.Back] =
                            !Physics.CheckBox(rayHit + Vector3.back * .5f, new Vector3(.25f, 10f, .25f), Quaternion.identity, 1 << (int)RayCastLayer.Trees, QueryTriggerInteraction.Ignore);
                        toAdd.BorderHighlights[(int)Orientation.Back].SetActive(!tileConnections.Connections[(int)Orientation.Back]);

                        tileConnections.Connections[(int)Orientation.Right] =
                            !Physics.CheckBox(rayHit + Vector3.right * .5f, new Vector3(.25f, 10f, .25f), Quaternion.identity, 1 << (int)RayCastLayer.Trees, QueryTriggerInteraction.Ignore);
                        toAdd.BorderHighlights[(int)Orientation.Right].SetActive(!tileConnections.Connections[(int)Orientation.Right]);

                        tileConnections.Connections[(int)Orientation.Left] =
                            !Physics.CheckBox(rayHit + Vector3.left * .5f, new Vector3(.25f, 10f, .25f), Quaternion.identity, 1 << (int)RayCastLayer.Trees, QueryTriggerInteraction.Ignore);
                        toAdd.BorderHighlights[(int)Orientation.Left].SetActive(!tileConnections.Connections[(int)Orientation.Left]);

                        tileContainer.Connections[z].Add(tileConnections);
                    }
                }
            }

            return tileContainer;
        }

        public TileContainer InitializeContainer(Transform container)
        {
            if (container.gameObject.GetComponent<TileContainer>() == null)
            {
                container.gameObject.AddComponent<TileContainer>();
            }

            TileContainer tileContainer = container.GetComponent<TileContainer>();

            for (int z = 0; z < container.transform.childCount; ++z)
            {
                tileContainer.Tiles.Add(new List<TileControl>());
                tileContainer.Connections.Add(new List<TileConnections>());
                tileContainer.Obstructions.Add(new List<bool>());

                Transform row = container.transform.GetChild(z);
                for (int x = 0; x < row.childCount; ++x)
                {
                    TileControl toAdd = row.GetChild(x).GetComponent<TileControl>();
                    tileContainer.Tiles[z].Add(toAdd);
                    Vector3 tileCenter = toAdd.transform.position;

                    bool isObstructed = Physics.CheckBox(tileCenter, new Vector3(.5f, .5f, .5f), Quaternion.identity, 1 << (int)RayCastLayer.Trees, QueryTriggerInteraction.Ignore);
                    if (isObstructed)
                    {
                        toAdd.SetHighlightState(TileControl.HighlightState.TargetSelect);
                    }
                    tileContainer.Obstructions[z].Add(isObstructed);

                    TileConnections tileConnections = new TileConnections();
                    tileConnections.Connections[(int)Orientation.Forward] =
                        !Physics.CheckBox(tileCenter + Vector3.forward * .5f, new Vector3(.25f, 10f, .25f), Quaternion.identity, 1 << (int)RayCastLayer.Trees, QueryTriggerInteraction.Ignore);
                    toAdd.BorderHighlights[(int)Orientation.Forward].SetActive(!tileConnections.Connections[(int)Orientation.Forward]);

                    tileConnections.Connections[(int)Orientation.Back] =
                        !Physics.CheckBox(tileCenter + Vector3.back * .5f, new Vector3(.25f, 10f, .25f), Quaternion.identity, 1 << (int)RayCastLayer.Trees, QueryTriggerInteraction.Ignore);
                    toAdd.BorderHighlights[(int)Orientation.Back].SetActive(!tileConnections.Connections[(int)Orientation.Back]);

                    tileConnections.Connections[(int)Orientation.Right] =
                        !Physics.CheckBox(tileCenter + Vector3.right * .5f, new Vector3(.25f, 10f, .25f), Quaternion.identity, 1 << (int)RayCastLayer.Trees, QueryTriggerInteraction.Ignore);
                    toAdd.BorderHighlights[(int)Orientation.Right].SetActive(!tileConnections.Connections[(int)Orientation.Right]);

                    tileConnections.Connections[(int)Orientation.Left] =
                        !Physics.CheckBox(tileCenter + Vector3.left * .5f, new Vector3(.25f, 10f, .25f), Quaternion.identity, 1 << (int)RayCastLayer.Trees, QueryTriggerInteraction.Ignore);
                    toAdd.BorderHighlights[(int)Orientation.Left].SetActive(!tileConnections.Connections[(int)Orientation.Left]);

                    tileContainer.Connections[z].Add(tileConnections);
                    
                }
            }

            return tileContainer;
        }
    }
}


