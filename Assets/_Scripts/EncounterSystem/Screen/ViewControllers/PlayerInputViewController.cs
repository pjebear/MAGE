using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace EncounterSystem.Screen
{
    using Character;
    using MapTypes;
    using MapEnums;
    using Common.ActionTypes;
    using Common.ActionEnums;
    using EncounterFlow;
    using System.Collections;
    using Map;

    enum ViewControllerState
    {
        HIBERNATE,
        MAP_ROAM,
        UNIT_SELECTED,
        MOVE_TILE_SELECTION,
        MOVE_TILE_CONFIRM,
        ACTION_CATEGORY_SELECTION,

        PRIMARY_ACTION_SELECTION,
        SECONDARY_ACTION_SELECTION,

        ACTION_TARGET_SELECTION,

        ACTION_CONFIRM
    }

    enum UnitSelectedButtonEnums
    {
        Status,
        Wait,
        Act,
        Move,
        NUM
    }

    enum ActionCategorySelectionEnums
    {
        Attack,
        Primary,
        Secondary,
        NUM
    }


    class PlayerInputViewController : MonoBehaviour
    {
        public GameObject ButtonList;
        private Button[] mScreenButtons;
        public UnitPanel LeftUnitPanel;
        public UnitPanel RightUnitPanel;
        public Text ActionText;
        public GameObject UnitMarkerPrefab;

        public bool FinishedLoading;

        private InputManager rInputManager;
        private CameraManager rCameraManager;
        private ViewControllerState mCurrentState;
        private CharacterManager mCurrentCharacter;
        private CharacterManager mSelectedCharacter;
        private MapTile mCursorTile;
        private CharacterActionIndex mSelectedAction;
        private UnitMarkerController mUnitMarkerController;

        private void Awake()
        {
            Debug.Assert(ActionText != null, "No ActionText hooked up");
            Debug.Assert(LeftUnitPanel != null, "No Unit Panel hooked up to left side");
            Debug.Assert(RightUnitPanel != null, "No Unit Panel hooked up to right side");
            Debug.Assert(ButtonList != null && ButtonList.transform.childCount > 0, "Null/Empty List of buttons provided to view control");
            mCurrentState = ViewControllerState.HIBERNATE;
            mCurrentCharacter = null;
            mSelectedCharacter = null;
            mCursorTile = null;
            mUnitMarkerController = null;
            FinishedLoading = false;

            mScreenButtons = new Button[ButtonList.transform.childCount];
            mUnitMarkerController = Instantiate(UnitMarkerPrefab).GetComponent<UnitMarkerController>();
            mUnitMarkerController.gameObject.SetActive(false);

            for (int i = 0; i < mScreenButtons.Count(); i++)
            {
                mScreenButtons[i] = ButtonList.transform.GetChild(i).GetComponent<Button>();
                mScreenButtons[i].gameObject.SetActive(false);
            }

            ActionText.transform.parent.parent.gameObject.SetActive(false);
            LeftUnitPanel.Initialize();
            LeftUnitPanel.Hide();
            RightUnitPanel.Initialize();
            RightUnitPanel.Hide();
        }

        private void Start()
        {
            FinishedLoading = true;
        }

        public void Initialize(InputManager manager)
        {
            rInputManager = manager;
            rCameraManager = Camera.main.gameObject.GetComponent<CameraManager>();
        }

        private void Update()
        {
            // mouse input
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f, 1 << LayerMask.NameToLayer("Map")))
            {
                GameObject go = hit.collider.gameObject;
                if (go != null)
                {
                    //Debug.Log("Raycast:" + gameObject.name);
                    MapObjectHovered(go);
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                OnSelect();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                OnBack();
            }
            else if (Input.GetMouseButtonDown(2))
            {
                Debug.Log("no mouse wheel click enabled");
            }

            // keyboard input
            if (Input.GetKey(KeyCode.W))
            {
                rCameraManager.PanVertical(false);   
            }
            if (Input.GetKey(KeyCode.S))
            {
                rCameraManager.PanVertical(true);
            }
            if (Input.GetKey(KeyCode.D))
            {
                rCameraManager.PanHorizontal(false);
            }
            if (Input.GetKey(KeyCode.A))
            {
                rCameraManager.PanHorizontal(true);
            }
            if (Input.GetKey(KeyCode.Q))
            {
                rCameraManager.Zoom(false);
            }
            if (Input.GetKey(KeyCode.E))
            {
                rCameraManager.Zoom(true);
            }
            if (Input.mouseScrollDelta != Vector2.zero)
            {
                MapManager.Instance.RotateMap(Input.mouseScrollDelta.y);
            }
        }

        public void GetInputForCharacter(CharacterManager characterManager)
        {
            Debug.Assert(mCurrentCharacter == null, "Attempting to Start new turn when turn still progressing");
            mCurrentCharacter = mSelectedCharacter = characterManager;
            ProgressStateFlow(ViewControllerState.UNIT_SELECTED);
            rCameraManager.PanToTarget(characterManager.gameObject);
        }

        public void DisplayCharacter(CharacterManager character)
        {
            mUnitMarkerController.MarkUnit(character.gameObject);
            LeftUnitPanel.DisplayUnit(character.GetUnitPanelPayload());
            rCameraManager.PanToTarget(character.gameObject);
        }

        public void DisplayTarget(CharacterManager character)
        {
            if (character != null)
            {
                RightUnitPanel.DisplayUnit(character.GetUnitPanelPayload());
            }
        }

        public void HideTargetDisplay()
        {
            RightUnitPanel.Hide();
        }

        public void HideCharacterDisplay()
        {
            mUnitMarkerController.gameObject.SetActive(false);
            LeftUnitPanel.Hide();
        }

        public void DisplayActionText(string actionText)
        {
            StartCoroutine(_DisplayActionText(actionText, 2));
        }

        //----------------------------------------------------------------------------------------------
        //--------------------------------------- Screen Event Functions--------------------------------
        //----------------------------------------------------------------------------------------------

        //----------------------------------------------------------------------------------------------
        private void ProgressStateFlow(ViewControllerState nextState)
        {
            mCurrentState = nextState;
            switch (mCurrentState)
            {
                case (ViewControllerState.HIBERNATE):
                    SetNumButtonsActive(0);
                    mUnitMarkerController.Hide();
                    mCurrentCharacter = null;
                    mSelectedCharacter = null;
                    mCursorTile = null;
                    HideCharacterDisplay();
                    RightUnitPanel.Hide();
                    break;
                case (ViewControllerState.MAP_ROAM):
                    SetNumButtonsActive(0);
                    HideCharacterDisplay();
                    //RightUnitPanel.Hide();
                    break;
                case (ViewControllerState.UNIT_SELECTED):
                    // show unit panels
                    SetUnitSelectedButtons();
                    DisplayCharacter(mSelectedCharacter);
                    break;
                case (ViewControllerState.MOVE_TILE_SELECTION):
                    SetNumButtonsActive(0);
                    // Hide unit panels
                    HideCharacterDisplay();
                    rInputManager.CalculateTilePaths(mCurrentCharacter);
                    rInputManager.DisplayMovementTiles(true);
                    break;

                case (ViewControllerState.MOVE_TILE_CONFIRM):
                    // Set Unit Panels
                    LeftUnitPanel.DisplayUnit(mSelectedCharacter.GetUnitPanelPayload());
                    rInputManager.DisplayMovementTiles(false);
                    SetConfirmButtons();
                    break;
                case (ViewControllerState.ACTION_CATEGORY_SELECTION):
                    SetActSelectedButtons();
                    break;
                case (ViewControllerState.PRIMARY_ACTION_SELECTION):
                    SetActionCategoryButtons(mCurrentCharacter.GetScreenActionPayload()[ActionContainerCategory.Primary]);
                    break;
                case (ViewControllerState.SECONDARY_ACTION_SELECTION):
                    SetActionCategoryButtons(mCurrentCharacter.GetScreenActionPayload()[ActionContainerCategory.Secondary]);
                    break;
                case (ViewControllerState.ACTION_TARGET_SELECTION):
                    SetNumButtonsActive(0);
                    rInputManager.CalculateActionTiles(mCurrentCharacter, mSelectedAction);
                    rInputManager.DisplayActionTiles(true);
                    LeftUnitPanel.Hide();
                    RightUnitPanel.Hide();
                    break;
                case (ViewControllerState.ACTION_CONFIRM):
                    // Set unitPanels
                    LeftUnitPanel.DisplayUnit(mSelectedCharacter.GetUnitPanelPayload());
                    if (mCursorTile.GetCharacterOnTile() != null)
                    {
                        RightUnitPanel.DisplayUnit(mCursorTile.GetCharacterOnTile().GetUnitPanelPayload());
                    }
                    rInputManager.DisplayActionTiles(false);
                    rInputManager.CalculateActionAOETiles(mCursorTile, mCurrentCharacter, mSelectedAction);
                    rInputManager.DisplayActionAOETiles(true);
                    SetConfirmButtons();
                    break;
                default:
                    Debug.LogFormat("No Case for {0} in Progress Flow", mCurrentState);
                    break;
            }
        }

        //---------------------------------------------------------------------------------------------
        private void OnSelect()
        {
            if (mCursorTile == null) return;

            switch (mCurrentState)
            {
                case (ViewControllerState.MAP_ROAM):
                        CharacterManager cm = mCursorTile.GetCharacterOnTile();
                        if (cm != null)
                        {
                            mSelectedCharacter = cm;
                            ProgressStateFlow(ViewControllerState.UNIT_SELECTED);
                        }
                    break;

                case (ViewControllerState.ACTION_TARGET_SELECTION):
                        if (rInputManager.IsValidTargetTile(mCursorTile))
                        {
                            ProgressStateFlow(ViewControllerState.ACTION_CONFIRM);
                        }  
                    break;

                case (ViewControllerState.MOVE_TILE_SELECTION):
                        if (rInputManager.IsValidMoveTile(mCursorTile))
                        {
                            ProgressStateFlow(ViewControllerState.MOVE_TILE_CONFIRM);
                        }
                    break;
            }
        }

        //---------------------------------------------------------------------------------------------
        private void MapObjectHovered(GameObject go)
        {
            if (mCurrentState == ViewControllerState.ACTION_CONFIRM || mCurrentState == ViewControllerState.MOVE_TILE_CONFIRM)
            {
                // dont update cursor position
            }
            else
            {
                MapTile tile = null;
                //check if character
                CharacterManager character = go.GetComponent<CharacterManager>();
                if (character != null)
                {
                    tile = character.GetCurrentTile();
                }
                else if (go.transform.parent != null)
                {
                    tile = go.transform.parent.GetComponent<MapTile>();
                }
                
                SetCursorHighlight(tile);
            }
        }

        //---------------------------------------------------------------------------------------------
        private void OnBack()
        {
            if (mCurrentState == ViewControllerState.UNIT_SELECTED)
            {
                ProgressStateFlow(ViewControllerState.MAP_ROAM);
            }
            else if (mCurrentState == ViewControllerState.MAP_ROAM 
                || mCurrentState == ViewControllerState.ACTION_CATEGORY_SELECTION)
            {
                ProgressStateFlow(ViewControllerState.UNIT_SELECTED);
            }
            else if (mCurrentState == ViewControllerState.PRIMARY_ACTION_SELECTION 
                || mCurrentState == ViewControllerState.SECONDARY_ACTION_SELECTION)
            {
                ProgressStateFlow(ViewControllerState.ACTION_CATEGORY_SELECTION);
            }
            else if (mCurrentState == ViewControllerState.MOVE_TILE_SELECTION)
            {
                rInputManager.DisplayMovementTiles(false);
                ProgressStateFlow(ViewControllerState.UNIT_SELECTED);
            }
            else if (mCurrentState == ViewControllerState.ACTION_TARGET_SELECTION)
            {
                rInputManager.DisplayActionTiles(false);
                if (mSelectedAction.ActionType == ActionContainerCategory.Attack)
                {
                    ProgressStateFlow(ViewControllerState.ACTION_CATEGORY_SELECTION);
                }
                else if (mSelectedAction.ActionType == ActionContainerCategory.Primary)
                {
                    ProgressStateFlow(ViewControllerState.PRIMARY_ACTION_SELECTION);
                }
                else
                {
                    ProgressStateFlow(ViewControllerState.SECONDARY_ACTION_SELECTION);
                }
            }
            else if (mCurrentState == ViewControllerState.MOVE_TILE_CONFIRM)
            {
                rInputManager.DisplayMovementTiles(true);
                mCurrentState = ViewControllerState.MOVE_TILE_SELECTION;
            }
            else if (mCurrentState == ViewControllerState.ACTION_CONFIRM)
            {
                rInputManager.DisplayActionAOETiles(false);
                rInputManager.DisplayActionTiles(true);
                mCurrentState = ViewControllerState.ACTION_TARGET_SELECTION;
            }
            else
            {
                Debug.Log("No back state hooked up for " + mCurrentState.ToString());
            }
        }

        //----------------------------------------------------------------------------------------------
        // ---------------------------------- Button Callbacks -------------------------------------
        //---------------------------------------------------------------------------------------------- 

        private void _ButtonSelected(int selection)
        {
            switch (mCurrentState)
            {
                case (ViewControllerState.UNIT_SELECTED):
                    switch (selection)
                    {
                        case ((int)UnitSelectedButtonEnums.Wait):
                            ProgressStateFlow(ViewControllerState.HIBERNATE);
                            rInputManager.WaitSelected();
                            return;
                        case ((int)UnitSelectedButtonEnums.Status):
                            //Put up status Screen
                            return;
                        case ((int)UnitSelectedButtonEnums.Move):
                            ProgressStateFlow(ViewControllerState.MOVE_TILE_SELECTION);
                            return;
                        case ((int)UnitSelectedButtonEnums.Act):
                            ProgressStateFlow(ViewControllerState.ACTION_CATEGORY_SELECTION);
                            return;
                    }
                    break;
                case (ViewControllerState.ACTION_CATEGORY_SELECTION):
                    switch (selection)
                    {
                        case ((int)ActionCategorySelectionEnums.Primary):
                            ProgressStateFlow(ViewControllerState.PRIMARY_ACTION_SELECTION);
                            return;
                        case ((int)ActionCategorySelectionEnums.Secondary):
                            ProgressStateFlow(ViewControllerState.SECONDARY_ACTION_SELECTION);
                            return;
                    }
                    break;
                default:
                    Debug.LogFormat("Button {0} has no case for {1}", selection, mCurrentState.ToString());
                    break;
            }
            
        }

        //----------------------------------------------------------------------------------------------
        private void _ActionSelected(int action, bool autoTileSelect)
        {
            mSelectedAction = CharacterActionIndex.ConvertHash(action);
            if (autoTileSelect)
            {
                mCursorTile = mCurrentCharacter.GetCurrentTile();
                ProgressStateFlow(ViewControllerState.ACTION_CONFIRM);
            }
            else
            {
                ProgressStateFlow(ViewControllerState.ACTION_TARGET_SELECTION);
            }
        }

        //----------------------------------------------------------------------------------------------
        // ---------------------------------- Screen Button States -------------------------------------
        //---------------------------------------------------------------------------------------------- 
        #region ButtonStates
        private void SetUnitSelectedButtons()
        {
            Debug.Assert(mCurrentCharacter != null, "Unit Selected state reached with no current unit");
            Debug.Assert(mSelectedCharacter != null, "Unit Selected state reached with no selected unit");

            // show left unit panel for selected character

            if (mSelectedCharacter != mCurrentCharacter)
            {
                // show status button
                SetNumButtonsActive(1);
                SetButtonState(mScreenButtons[(int)UnitSelectedButtonEnums.Status], UnitSelectedButtonEnums.Status.ToString(),
                    false, delegate { _ButtonSelected((int)UnitSelectedButtonEnums.Status); });
            }
            else // show base menu
            {
                SetNumButtonsActive((int)UnitSelectedButtonEnums.NUM);

                // Status
                SetButtonState(mScreenButtons[(int)UnitSelectedButtonEnums.Status], UnitSelectedButtonEnums.Status.ToString(),
                    false, delegate { _ButtonSelected((int)UnitSelectedButtonEnums.Status); });

                // Wait
                SetButtonState(mScreenButtons[(int)UnitSelectedButtonEnums.Wait], UnitSelectedButtonEnums.Wait.ToString(),
                   true, delegate { _ButtonSelected((int)UnitSelectedButtonEnums.Wait); });

                //Move
                SetButtonState(mScreenButtons[(int)UnitSelectedButtonEnums.Move], UnitSelectedButtonEnums.Move.ToString(),
                  mCurrentCharacter.CanMove, delegate { _ButtonSelected((int)UnitSelectedButtonEnums.Move); });

                //Act
                SetButtonState(mScreenButtons[(int)UnitSelectedButtonEnums.Act], UnitSelectedButtonEnums.Act.ToString(),
                  mCurrentCharacter.CanAct, delegate { _ButtonSelected((int)UnitSelectedButtonEnums.Act); });
            }
        }

        //----------------------------------------------------------------------------------------------
        private void SetActSelectedButtons()
        {
            Debug.Assert(mCurrentCharacter != null, "Unit Selected state reached with no current unit");
            Debug.Assert(mSelectedCharacter != null, "Unit Selected state reached with no selected unit");

            Dictionary<ActionContainerCategory, List<ScreenActionPayload>> actions = mCurrentCharacter.GetScreenActionPayload();

            SetNumButtonsActive((int)ActionCategorySelectionEnums.NUM);

            // Attack
            Debug.Assert(actions[ActionContainerCategory.Attack].Count > 0, "No Attack Ability hooked up for character");
            SetButtonState(mScreenButtons[2], ActionCategorySelectionEnums.Attack.ToString(), 
                actions[ActionContainerCategory.Attack][0].CanUse, 
                delegate { _ActionSelected(actions[ActionContainerCategory.Attack][0].ActionIndex.Hash(), actions[ActionContainerCategory.Attack][0].AutoTargetSelection); });

            //Primary
            SetButtonState(mScreenButtons[1], ActionCategorySelectionEnums.Primary.ToString(),
               actions[ActionContainerCategory.Primary].Count > 0, delegate { ProgressStateFlow(ViewControllerState.PRIMARY_ACTION_SELECTION); });

            //Secondary
            SetButtonState(mScreenButtons[0], ActionCategorySelectionEnums.Secondary.ToString(),
              actions[ActionContainerCategory.Secondary].Count > 0, delegate { ProgressStateFlow(ViewControllerState.SECONDARY_ACTION_SELECTION); });
        }

        //----------------------------------------------------------------------------------------------
        private void SetActionCategoryButtons(List<ScreenActionPayload> actionPayloads)
        {
            SetNumButtonsActive(actionPayloads.Count);
            
            for (int i = 0; i < actionPayloads.Count; ++i)
            {
                ScreenActionPayload payload = actionPayloads[i];
                SetButtonState(mScreenButtons[i], payload.ActionName + " " + (int)payload.ActionCost.Value, 
                    payload.CanUse, delegate { _ActionSelected(payload.ActionIndex.Hash(), payload.AutoTargetSelection); });
            }
        }

        //----------------------------------------------------------------------------------------------
        private void SetConfirmButtons()
        {
            SetNumButtonsActive(2);
            switch(mCurrentState)
            {
                case (ViewControllerState.MOVE_TILE_CONFIRM):
                    SetButtonState(mScreenButtons[1], "Confirm?", true,
                        delegate
                        {
                            mUnitMarkerController.Hide();
                            MapTile selection = mCursorTile;
                            ProgressStateFlow(ViewControllerState.HIBERNATE);
                            rInputManager.MovementChosen(selection);
                            
                        });
                    break;
                case (ViewControllerState.ACTION_CONFIRM):
                    SetButtonState(mScreenButtons[1], "Confirm?", true,
                        delegate
                        {
                            mUnitMarkerController.Hide();
                            CharacterActionIndex selection = mSelectedAction;
                            ProgressStateFlow(ViewControllerState.HIBERNATE);
                            rInputManager.DisplayActionAOETiles(false);
                            rInputManager.ActionChosen(selection);
                        }
                        );
                    break;
                default:
                    Debug.LogFormat("Confirm buttons for state {0} not hooked up", mCurrentState.ToString());
                    break;
            }

            SetButtonState(mScreenButtons[0], "Cancel", true, OnBack);
            
        }

        #endregion

        //----------------------------------------------------------------------------------------------
        //------------------------------------------Helpers--------------------------------------
        //----------------------------------------------------------------------------------------------
        #region Helpers
        //----------------------------------------------------------------------------------------------
        private void SetNumButtonsActive(int numActive)
        {
            Debug.AssertFormat(numActive < mScreenButtons.Count(), "Requested to set {0} buttons active when only {1} buttons available", numActive, mScreenButtons.Count());
            for (int i = 0; i < mScreenButtons.Count(); ++i)
            {
                // Set all Buttons up to numActive, active
                if (i < numActive)
                {
                    mScreenButtons[i].gameObject.SetActive(true);
                    mScreenButtons[i].onClick.RemoveAllListeners();
                }
                else // turn off all buttons greater than numActive
                {
                    if (mScreenButtons[i].gameObject.activeSelf)
                    {
                        mScreenButtons[i].gameObject.SetActive(false);
                    }
                    else // all buttons past this point should be inactive
                    {
                        return;
                    }
                }
            }
        }
        
        //----------------------------------------------------------------------------------------------
        private void SetButtonState(Button toSet, string text, bool interactible, UnityEngine.Events.UnityAction buttonAction = null)
        {
            if (!toSet.gameObject.activeSelf)
            {
                Debug.Log("Attempting to set button state of inactive button. Did you forget to set NumButtonsActive?");
            }

            toSet.GetComponentInChildren<Text>().text = text;
            toSet.interactable = interactible;

            if (buttonAction != null)
            {
                toSet.onClick.AddListener(buttonAction);
            }
            else if (interactible)
            {
                Debug.Log("No button Action hooked up to button set to interactible");
            }
        }
        
        //----------------------------------------------------------------------------------------------
        private void SetCursorHighlight(MapTile toHighlight)
        {
            if (mCursorTile == toHighlight)
            {
                return;
            }

            // set previous tile to not clear
            if (mCursorTile != null)
            {
                Color previousColor = Color.clear;
                switch (mCurrentState)
                {
                    case (ViewControllerState.ACTION_TARGET_SELECTION):
                        if (rInputManager.IsValidTargetTile(mCursorTile))
                        {
                            previousColor = MapConstants.ActionTileColor;
                        }
                        break;
                    case (ViewControllerState.MOVE_TILE_SELECTION):
                        if (rInputManager.IsValidMoveTile(mCursorTile))
                        {
                            previousColor = MapConstants.MoveTileColor;
                        }
                        break;
                }

                mCursorTile.SetTileColor(previousColor);
            }

            if (toHighlight != null)
            {
                bool validTile = false;
                switch (mCurrentState)
                {
                    case (ViewControllerState.ACTION_TARGET_SELECTION):
                        if (rInputManager.IsValidTargetTile(toHighlight))
                        {
                            validTile = true;
                            toHighlight.SetTileColor(MapConstants.CursorTileColor);
                            mCursorTile = toHighlight;
                            if (toHighlight.GetCharacterOnTile() != null)
                            {
                                RightUnitPanel.DisplayUnit(toHighlight.GetCharacterOnTile().GetUnitPanelPayload());
                            }
                            else
                            {
                                RightUnitPanel.Hide();
                            }
                        }
                        else
                        {
                            RightUnitPanel.Hide();
                        }
                        break;
                    case (ViewControllerState.MOVE_TILE_SELECTION):
                        if (rInputManager.IsValidMoveTile(toHighlight))
                        {
                            validTile = true;
                            toHighlight.SetTileColor(MapConstants.CursorTileColor);
                            mCursorTile = toHighlight;
                        }
                        break;
                }
                if (!validTile)
                {
                    mCursorTile = null;
                }
            }
            else
            {
                mCursorTile = null;
            }
        }

        //----------------------------------------------------------------------------------------------
        private IEnumerator _DisplayActionText(string text, float duration)
        {
            ActionText.transform.parent.parent.gameObject.SetActive(true);
            ActionText.text = text;

            yield return new WaitForSeconds(duration);
            ActionText.transform.parent.parent.gameObject.SetActive(false);
        }
        #endregion
    }
}
