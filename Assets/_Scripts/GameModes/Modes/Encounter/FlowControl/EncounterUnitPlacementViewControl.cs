using MAGE.GameModes.Encounter;
using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using MAGE.GameSystems.Characters;
using MAGE.UI;
using MAGE.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.FlowControl
{
    class EncounterUnitPlacementViewControl
    : UIContainerControl
    , IInputHandler
    {
        private string TAG = "EncounterUnitPlacementViewControl";

        private List<Character> mCharactersToPlace = new List<Character>();
        private int mCharacterIdx = 0;
        private List<TileControl> mPlacementTiles = new List<TileControl>();
        private Dictionary<int, TileIdx> mPlacedCharacterLookup = new Dictionary<int, TileIdx>();

        private TileSelectionStack mTileSelectionStack = new TileSelectionStack();
        private int mAvailableTileLayer = 0;
        private int mHoveredTileLayer = 1;

        private TileControl mHoveredTile = null;

        public void Init()
        {
            mAvailableTileLayer = mTileSelectionStack.AddLayer(new List<TileControl>(), TileControl.HighlightState.MovementSelect);
            mHoveredTileLayer = mTileSelectionStack.AddLayer(new List<TileControl>(), TileControl.HighlightState.AOESelect);
        }

        public void Start()
        {
            foreach (int characterId in MAGE.GameSystems.DBService.Get().LoadTeam(TeamSide.AllyHuman))
            {
                
                Character character = MAGE.GameSystems.CharacterService.Get().GetCharacter(characterId);
                character.TeamSide = TeamSide.AllyHuman;

                if (EncounterModule.Model.EncounterContext.CharacterPositions.ContainsKey(character.Id))
                {
                    TileIdx atTile = EncounterModule.Model.EncounterContext.CharacterPositions[character.Id];
                    CharacterPosition characterPosition = new CharacterPosition(atTile, Orientation.Forward);
                    EncounterModule.CharacterDirector.AddCharacter(character, characterPosition);
                    mPlacedCharacterLookup.Add(characterId, atTile);
                }
                else
                {
                    mCharactersToPlace.Add(character);
                }
            }

            if (mCharactersToPlace.Count > 0)
            {
                foreach (TileIdx tileIdx in EncounterModule.Model.AllySpawnPoints)
                {
                    mPlacementTiles.Add(EncounterModule.MapControl[tileIdx]);
                }

                mTileSelectionStack.UpdateLayer(mAvailableTileLayer, mPlacementTiles);
                mTileSelectionStack.DisplayTiles();

                EncounterModule.CameraDirector.FocusTarget(mPlacementTiles[0].transform);

                Input.InputManager.Instance.RegisterHandler(this, false);
                UIManager.Instance.PostContainer(UIContainerId.EncounterUnitPlacementView, this);
            }
            else
            {
                Messaging.MessageRouter.Instance.NotifyMessage(new EncounterMessage(EncounterMessage.EventType.UnitPlacementComplete));
            }
        }

        public void Cleanup()
        {
            Input.InputManager.Instance.ReleaseHandler(this);
            mTileSelectionStack.HideTiles();
            UIManager.Instance.RemoveOverlay(UIContainerId.EncounterUnitPlacementView);
        }

        public void HandleComponentInteraction(int containerId, UIInteractionInfo interactionInfo)
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
                                Messaging.MessageRouter.Instance.NotifyMessage(new EncounterMessage(EncounterMessage.EventType.UnitPlacementComplete));
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

        public IDataProvider Publish(int containerId)
        {
            EncounterUnitPlacementView.DataProvider dataProvider = new EncounterUnitPlacementView.DataProvider();

            dataProvider.character = mCharactersToPlace[mCharacterIdx].Name;

            return dataProvider;
        }

        // IInputHandler
        public void OnMouseHoverChange(GameObject mouseHover)
        {
            TileControl tile = GetTileFromObj(mouseHover);

            List<TileControl> hoverSelection = new List<TileControl>();
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
            List<TileControl> availableTiles = new List<TileControl>();
            foreach (TileIdx tileIdx in EncounterModule.Model.AllySpawnPoints)
            {
                if (!mPlacedCharacterLookup.Values.Contains(tileIdx))
                {
                    availableTiles.Add(EncounterModule.MapControl[tileIdx]);
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
            foreach (TileControl tile in mPlacementTiles)
            {
                tile.ClearOnTile();
            }

            for (; mCharacterIdx < mCharactersToPlace.Count; ++mCharacterIdx)
            {
                PlaceSelectedCharacterAtTile(mPlacementTiles[mCharacterIdx]);
            }
        }

        private void PlaceSelectedCharacterAtTile(TileControl tile)
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
                    
                    CharacterActorController controller = EncounterModule.CharacterDirector.CharacterActorLookup[EncounterModule.Model.Characters[selectedCharacterId]];

                    EncounterModule.MapControl.UpdateCharacterPosition(controller, new CharacterPosition(tile.Idx, Orientation.Forward));

                    mPlacedCharacterLookup[selectedCharacterId] = tile.Idx;
                }
            }
            else
            {
                mPlacedCharacterLookup.Add(selectedCharacterId, tile.Idx);

                EncounterModule.CharacterDirector.AddCharacter(mCharactersToPlace[mCharacterIdx], new CharacterPosition(tile.Idx, Orientation.Forward));
            }
        }

        private TileControl GetTileFromObj(GameObject obj)
        {
            TileControl tile = null;

            if (obj != null)
            {
                tile = obj.GetComponentInParent<TileControl>();
            }

            return tile;
        }
    }

}

