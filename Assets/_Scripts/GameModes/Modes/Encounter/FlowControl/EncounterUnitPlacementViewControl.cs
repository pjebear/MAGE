using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class EncounterUnitPlacementViewControl 
    : UIContainerControl
    , IInputHandler
{
    private string TAG = "EncounterUnitPlacementViewControl";

    private List<DB.DBCharacter> mCharactersToPlace = new List<DB.DBCharacter>();
    private int mCharacterIdx = 0;
    private List<Tile> mPlacementTiles = new List<Tile>();
    private Dictionary<int, TileIdx> mPlacedCharacterLookup = new Dictionary<int, TileIdx>();

    private TileSelectionStack mTileSelectionStack = new TileSelectionStack();
    private int mAvailableTileLayer = 0;
    private int mHoveredTileLayer = 1;
    
    private Tile mHoveredTile = null;
    
    public void Init()
    {
        mAvailableTileLayer = mTileSelectionStack.AddLayer(new List<Tile>(), Tile.HighlightState.MovementSelect);
        mHoveredTileLayer = mTileSelectionStack.AddLayer(new List<Tile>(), Tile.HighlightState.AOESelect);
    }

    public void Start()
    {
        mCharactersToPlace = DB.DBHelper.LoadCharactersOnTeam(TeamSide.AllyHuman);

        foreach (TileIdx tileIdx in EncounterModule.Model.AllySpawnPoints)
        {
            mPlacementTiles.Add(EncounterModule.Map[tileIdx]);
        }

        mTileSelectionStack.UpdateLayer(mAvailableTileLayer, mPlacementTiles);
        mTileSelectionStack.DisplayTiles();

        EncounterModule.CameraDirector.FocusTarget(mPlacementTiles[0].transform);

        InputManager.Instance.RegisterHandler(this, false);
        UIManager.Instance.PostContainer(UIContainerId.EncounterUnitPlacementView, this);
    }

    public void Cleanup()
    {
        InputManager.Instance.ReleaseHandler(this);
        mTileSelectionStack.HideTiles();
        UIManager.Instance.RemoveOverlay(UIContainerId.EncounterUnitPlacementView);
    }

    public void HandleComponentInteraction(int containerId, IUIInteractionInfo interactionInfo)
    {
        switch (containerId)
        {
            case (int)UIContainerId.EncounterUnitPlacementView:
            {
                if (interactionInfo.InteractionType == UIInteractionType.Click)
                {
                    switch (interactionInfo.ComponentId)
                    {
                        case (int)EncounterUnitPlacementView.ComponentId.CharacterSelectLeftBtn:
                        {
                            CycleCharacter(-1);
                        }
                        break;

                        case (int)EncounterUnitPlacementView.ComponentId.CharacterSelectRightBtn:
                        {
                            CycleCharacter(1);
                        }
                        break;

                        case (int)EncounterUnitPlacementView.ComponentId.AutoFillBtn:
                        {
                            AutoFillCharacters();
                        }
                        break;

                        case (int)EncounterUnitPlacementView.ComponentId.ConfirmBtn:
                        {
                            CommitCharactersToGame();
                            EncounterEventRouter.Instance.NotifyEvent(new EncounterEvent(EncounterEvent.EventType.UnitPlacementComplete));
                        }
                        break;
                    }
                }
            }
            break;
        }
    }

    public string Name()
    {
        return TAG;
    }

    public IDataProvider Publish()
    {
        EncounterUnitPlacementView.DataProvider dataProvider = new EncounterUnitPlacementView.DataProvider();

        dataProvider.character = mCharactersToPlace[mCharacterIdx].CharacterInfo.Name;

        return dataProvider;
    }

    // IInputHandler
    public void OnMouseHoverChange(GameObject mouseHover)
    {
        Tile tile = GetTileFromObj(mouseHover);

        List<Tile> hoverSelection = new List<Tile>();
        if (tile != null && mPlacementTiles.Contains(tile))
        {
            mHoveredTile = tile;
            hoverSelection.Add(mHoveredTile);
        }
        else
        {
            mHoveredTile = null;
        }

        mTileSelectionStack.UpdateLayer(mHoveredTileLayer, hoverSelection);
    }

    public void OnKeyPressed(InputSource source, int key, InputState state)
    {
        if (source == InputSource.Mouse && state == InputState.Down)
        {
            if ((MouseKey)key == MouseKey.Left && mHoveredTile != null)
            {
                PlaceSelectedCharacterAtTile(mHoveredTile);
                CycleCharacter(1);
            }
        }
    }


    public void OnMouseScrolled(float scrollDelta)
    {
        // empty... For now
    }

    // IInputHandler End

    private void CycleCharacter(int direction)
    {
        int newIdx = mCharacterIdx + direction;
        if (newIdx < 0) newIdx = mCharactersToPlace.Count - 1;
        if (newIdx == mCharactersToPlace.Count) newIdx = 0;

        if (newIdx != mCharacterIdx)
        {
            mCharacterIdx = newIdx;
        }

        UIManager.Instance.Publish(UIContainerId.EncounterUnitPlacementView);
    }

    private void UpdateSelectableTiles()
    {
        List<Tile> availableTiles = new List<Tile>();
        foreach (TileIdx tileIdx in EncounterModule.Model.AllySpawnPoints)
        {
            if (!mPlacedCharacterLookup.Values.Contains(tileIdx))
            {
                availableTiles.Add(EncounterModule.Map[tileIdx]);
            }
        }

        mTileSelectionStack.UpdateLayer(mAvailableTileLayer, availableTiles);
    }

    private void CommitCharactersToGame()
    {

    }

    private void AutoFillCharacters()
    {
        // Reset everything first
        foreach (Tile tile in mPlacementTiles)
        {
            tile.OnTile = null;
        }

        for (; mCharacterIdx < mCharactersToPlace.Count; ++mCharacterIdx)
        {
            PlaceSelectedCharacterAtTile(mPlacementTiles[mCharacterIdx]);
        }
    }

    private void PlaceSelectedCharacterAtTile(Tile tile)
    {
        Logger.Assert(tile != null, LogTag.UI, TAG, "Attempting to place character when hovered tile is null", LogLevel.Warning);
        if (tile == null)
        {
            return;
        }

        int selectedCharacterId = mCharactersToPlace[mCharacterIdx].Id;

        if (mPlacedCharacterLookup.Keys.Contains(selectedCharacterId)) // character has already been placed on the map
        {
            Logger.Assert(tile.Idx != mPlacedCharacterLookup[selectedCharacterId], LogTag.UI, TAG, "Attempting to place character on same tile it is already on");
            if (tile.Idx == mPlacedCharacterLookup[selectedCharacterId])
            {
                return;
            }
            else // update the tile the character is on
            {
                EncounterActorController controller = EncounterModule.Map[mPlacedCharacterLookup[selectedCharacterId]].OnTile;

                EncounterModule.Map.PlaceAtTile(tile.Idx, controller);

                mPlacedCharacterLookup[selectedCharacterId] = tile.Idx;
            }
        }
        else
        {
            mPlacedCharacterLookup.Add(selectedCharacterId, tile.Idx);

            EncounterModule.CharacterDirector.AddCharacter(mCharactersToPlace[mCharacterIdx], TeamSide.AllyHuman, tile.Idx);
        }
    }

    private Tile GetTileFromObj(GameObject obj)
    {
        Tile tile = null;

        if (obj != null)
        {
            tile = obj.GetComponentInParent<Tile>();
        }

        return tile;
    }
}

