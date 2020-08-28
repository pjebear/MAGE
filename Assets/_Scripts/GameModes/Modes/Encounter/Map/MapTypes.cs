using MAGE.GameModes.SceneElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes.Encounter
{
    class TileSelection
    {
        public List<TileControl> Selection;
        public TileControl.HighlightState SelectionType;
        public static TileSelection EmptySelection
        {
            get
            {
                return new TileSelection(new List<TileControl>(), TileControl.HighlightState.None);
            }
        }

        public TileSelection(List<TileControl> selection, TileControl.HighlightState selectionType)
        {
            Selection = selection;
            SelectionType = selectionType;
        }

        public void HighlightSelection()
        {
            foreach (TileControl tile in Selection)
            {
                tile.SetHighlightState(SelectionType);
            }
        }

        public void ClearSelection()
        {
            foreach (TileControl tile in Selection)
            {
                tile.SetHighlightState(TileControl.HighlightState.None);
            }
        }
    }

    class TileSelectionStack
    {
        private List<TileSelection> mSelectionStack = new List<TileSelection>();
        private bool mIsDisplaying = false;

        public TileSelection this[int idx]
        {
            get
            {
                return mSelectionStack[idx];
            }
        }

        public int Count
        {
            get
            {
                return mSelectionStack.Count;
            }
        }

        public int AddLayer(List<TileControl> tiles, TileControl.HighlightState highlightState)
        {
            mSelectionStack.Add(new TileSelection(tiles, highlightState));

            return mSelectionStack.Count - 1;
        }

        public void RemoveLayer()
        {
            mSelectionStack.RemoveAt(mSelectionStack.Count - 1);
        }

        public void Reset()
        {
            if (mIsDisplaying)
            {
                HideTiles();
            }

            mSelectionStack.Clear();

            if (mIsDisplaying)
            {
                DisplayTiles();
            }
        }

        public void UpdateLayer(int layer, List<TileControl> tiles)
        {
            bool isDisplaying = mIsDisplaying;
            if (isDisplaying)
            {
                HideTiles();
            }

            mSelectionStack[layer].Selection = tiles;

            if (isDisplaying)
            {
                DisplayTiles();
            }

            mIsDisplaying = isDisplaying;
        }

        public void RefreshDisplay()
        {
            HideTiles();
            DisplayTiles();
        }

        public void HideTiles()
        {
            mIsDisplaying = false;
            foreach (TileSelection selection in mSelectionStack)
            {
                selection.ClearSelection();
            }
        }

        public void DisplayTiles()
        {
            mIsDisplaying = true;
            foreach (TileSelection selection in mSelectionStack)
            {
                selection.HighlightSelection();
            }
        }
    }
}
