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