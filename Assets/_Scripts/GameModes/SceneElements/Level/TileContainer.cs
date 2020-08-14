using MAGE.GameServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    class TileContainer : MonoBehaviour
    {
        public List<List<Tile>> Tiles;

        private void Start()
        {
            Tiles = new List<List<Tile>>();

            for (int rowIdx = 0; rowIdx < transform.childCount; ++rowIdx)
            {
                List<Tile> tileRow = new List<Tile>();

                Transform rowContainer = transform.GetChild(rowIdx);

                for (int tileIdx = 0; tileIdx < rowContainer.childCount; ++tileIdx)
                {
                    Tile toAdd = rowContainer.GetChild(tileIdx).gameObject.GetComponent<Tile>();
                    if (toAdd != null)
                    {
                        toAdd.Init(new TileIdx(tileIdx, rowIdx));
                        rowContainer.GetChild(tileIdx).gameObject.SetActive(false);
                        tileRow.Add(toAdd);
                    }
                    else
                    {
                        //Debug.LogWarning(rowIdx + " " + tileIdx);
                    }

                }

                Tiles.Add(tileRow);
            }
        }
    }
}


