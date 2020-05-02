using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class TileSelection
{
    public List<Tile> Selection;
    public Tile.HighlightState SelectionType;
    public static TileSelection EmptySelection
    {
        get
        {
            return new TileSelection(new List<Tile>(), Tile.HighlightState.None);
        }
    }

    public TileSelection(List<Tile> selection, Tile.HighlightState selectionType)
    {
        Selection = selection;
        SelectionType = selectionType;
    }

    public void HighlightSelection()
    {
        foreach (Tile tile in Selection)
        {
            tile.SetHighlightState(SelectionType);
        }
    }

    public void ClearSelection()
    {
        foreach (Tile tile in Selection)
        {
            tile.SetHighlightState(Tile.HighlightState.None);
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

    public int AddLayer(List<Tile> tiles, Tile.HighlightState highlightState)
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

    public void UpdateLayer(int layer, List<Tile> tiles)
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