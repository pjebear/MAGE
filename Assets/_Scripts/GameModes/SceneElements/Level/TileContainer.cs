using MAGE.GameSystems;
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
        public List<List<TileControl>> Tiles = new List<List<TileControl>>();

        private void OnDestroy()
        {
            ClearTiles();
        }

        //private void Start()
        //{
        //    Tiles = new List<List<TileControl>>();

        //    for (int rowIdx = 0; rowIdx < transform.childCount; ++rowIdx)
        //    {
        //        List<TileControl> tileRow = new List<TileControl>();

        //        Transform rowContainer = transform.GetChild(rowIdx);

        //        for (int tileIdx = 0; tileIdx < rowContainer.childCount; ++tileIdx)
        //        {
        //            TileControl toAdd = rowContainer.GetChild(tileIdx).gameObject.GetComponent<TileControl>();
        //            if (toAdd != null)
        //            {
        //                rowContainer.GetChild(tileIdx).gameObject.SetActive(false);
        //                tileRow.Add(toAdd);
        //            }
        //            else
        //            {
        //                //Debug.LogWarning(rowIdx + " " + tileIdx);
        //            }

        //        }

        //        Tiles.Add(tileRow);
        //    }
        //}


        public void ClearTiles()
        {
            foreach (List<TileControl> row in Tiles)
            {
                foreach (TileControl tileControl in row)
                {
                    Destroy(tileControl.gameObject);
                }
            }

            Tiles.Clear();
        }
    }
}


