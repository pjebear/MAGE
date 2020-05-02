using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class TileContainerGenerator : MonoBehaviour
{
    public Terrain Terrain;
    public Tile TilePrefab;

    private void Start()
    {
        GenerateTiles();
    }

    public void GenerateTiles()
    {
        GameObject tileContainer = new GameObject("Tiles");
        tileContainer.transform.SetParent(transform.parent);

        for (int z = 0; z < Terrain.Length; ++z)
        {
            GameObject tileRow = new GameObject("Row " + z.ToString());
            tileRow.transform.SetParent(tileContainer.transform);
            for (int x = 0; x < Terrain.Width; ++x)
            {
                Ray ray = new Ray(new Vector3(x + .5f, 100, z + .5f), Vector3.down);

                int layerMask = 1 << 10;
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 200, layerMask))
                {
                    Vector3 tileCenter = hit.point + Vector3.up * .1f;
                    Vector3 normal = hit.normal;

                    Tile toAdd = Instantiate(TilePrefab, tileCenter, Quaternion.identity);
                    toAdd.transform.SetParent(tileRow.transform);
                    toAdd.Init(new TileIdx(x, z));
                    toAdd.transform.up = normal;
                }
            }
        }
    }
}

