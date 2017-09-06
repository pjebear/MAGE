using EncounterSystem.Character;
using EncounterSystem.EncounterFlow;
using EncounterSystem.Map;
using EncounterSystem.MapEnums;
using EncounterSystem.MapTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EncounterSystem.Screen
{
    using System;
    using Screens.Payloads;
    class UnitPlacementViewController : MonoBehaviour
    {
        public UnitPlacementPanel UnitDisplay;
        public Button ScrollRightButton;
        public Button ScrollLeftButton;
        public Button StartEncounterButton;
        public Text UnitsPlacedText;
        public Text MaxUnitsText;

        private List<CharacterManager> mPotentialunits;
        private Dictionary<int, CharacterManager> mChosenUnits;
        private int mCurrentUnitIdx;
        private int mMaxUnits;
        private InputManager rInputManager;
        private CameraManager rCameraManager;
        private MapTile mCursorTile;

        private void Awake()
        {
            mCurrentUnitIdx = mMaxUnits = 0;
            Debug.Assert(UnitDisplay != null);
            Debug.Assert(ScrollLeftButton != null);
            Debug.Assert(ScrollRightButton != null);
            Debug.Assert(StartEncounterButton != null);
            Debug.Assert(UnitsPlacedText != null);
            Debug.Assert(MaxUnitsText != null);
            mChosenUnits = new Dictionary<int, CharacterManager>();
        }

        private void Start()
        {
            rCameraManager = Camera.main.GetComponent<CameraManager>();
            ScrollLeftButton.onClick.AddListener(delegate { _ScrollUnitSelection(-1); });
            ScrollRightButton.onClick.AddListener(delegate { _ScrollUnitSelection(1); });
            StartEncounterButton.onClick.AddListener(delegate { _BeginEncounter(); });
        }

        public void Initialize(InputManager manager)
        {
            rInputManager = manager;
        }

        public void GetCharacterLineupForEncounter(List<CharacterManager> potentialCharacters, int maxCharacters, int mapSide, List<CharacterManager> mandatoryUnits = null)
        {
            MapManager.Instance.CalculateUnitPlacementTiles(mapSide);
            mPotentialunits = new List<CharacterManager>();
            mPotentialunits.AddRange(potentialCharacters);
            mMaxUnits = maxCharacters;
            mCurrentUnitIdx = 0;
            UpdateDisplay();
            DisplayCurrentUnit();
        }

        public void Update()
        {
            // mouse input
            RaycastHit hit;
            GameObject go = null;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f, 1 << LayerMask.NameToLayer("Map")))
            {
                go = hit.collider.gameObject;
            }
            MapObjectHovered(go);

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

        private void DisplayCurrentUnit()
        {
            bool displaySelector = mPotentialunits.Count > 0;
            UnitDisplay.gameObject.SetActive(displaySelector);
            ScrollLeftButton.gameObject.SetActive(displaySelector);
            ScrollRightButton.gameObject.SetActive(displaySelector);

            // make sure index is still valid
            if (mPotentialunits.Count == 0) return;
            if (mCurrentUnitIdx >= mPotentialunits.Count)
            {
                mCurrentUnitIdx = mPotentialunits.Count - 1;
            }

            CharacterManager character = mPotentialunits[mCurrentUnitIdx];
            UnitPanelPayload payload = character.GetUnitPanelPayload();
            UnitDisplay.DisplayUnit(payload.UnitName, payload.CharacterLevel, payload.ImageAssetPath);
        }

        private void UpdateDisplay()
        {
            UnitsPlacedText.text = mChosenUnits.Count.ToString();
            MaxUnitsText.text = mMaxUnits.ToString();
            StartEncounterButton.interactable = mChosenUnits.Count > 0;

            MapManager.Instance.DisplayUnitPlacementTiles(mChosenUnits.Count < mMaxUnits);
   
        }

        private void _ScrollUnitSelection(int direction)
        {
            mCurrentUnitIdx = (mCurrentUnitIdx + direction) % mPotentialunits.Count;
            DisplayCurrentUnit();
        }

        private void _BeginEncounter()
        {
            List<CharacterManager> chosenUnits = new List<CharacterManager>();
            chosenUnits.AddRange(mChosenUnits.Values);
            MapManager.Instance.DisplayUnitPlacementTiles(false);
            rInputManager.PlacementFinished(chosenUnits);
        }

        //---------------------------------------------------------------------------------------------
        private void OnSelect()
        {
            if (mCursorTile == null) return;
            
            CharacterManager unitSelectCharacter = mPotentialunits[mCurrentUnitIdx];
            Debug.Assert(!mChosenUnits.ContainsKey(unitSelectCharacter.CharacterBase.CharacterID)); // shouldn't be allowed to place same unit twice

            CharacterManager onTile = mCursorTile.GetCharacterOnTile();

            if (onTile != null)
            {
                mChosenUnits.Remove(onTile.CharacterBase.CharacterID);
                mPotentialunits.Add(onTile);
                mCursorTile.RemoveCharacterFromTile();
                onTile.gameObject.SetActive(false);
                
            }
            
            if (mChosenUnits.Count < mMaxUnits)
            {
                mChosenUnits.Add(unitSelectCharacter.CharacterBase.CharacterID, unitSelectCharacter);
                mPotentialunits.Remove(unitSelectCharacter);

                unitSelectCharacter.UpdateCurrentTile(mCursorTile, true);
                unitSelectCharacter.gameObject.SetActive(true);
                SetCursorHighlight(mCursorTile);
                DisplayCurrentUnit();
            }
            UpdateDisplay();
        }

        //---------------------------------------------------------------------------------------------
        private void OnBack()
        {
            if (mCursorTile == null) return;
            CharacterManager onTile = mCursorTile.GetCharacterOnTile();
            if (onTile != null)
            {
                mChosenUnits.Remove(onTile.CharacterBase.CharacterID);
                mPotentialunits.Add(onTile);
                mCursorTile.RemoveCharacterFromTile();
                onTile.gameObject.SetActive(false);
                DisplayCurrentUnit();
            }
            UpdateDisplay();
        }

        //---------------------------------------------------------------------------------------------
        private void MapObjectHovered(GameObject go)
        {
            MapTile tile = null;
            //check if character
            if (go != null)
            {
                CharacterManager character = go.GetComponent<CharacterManager>();
                if (character != null)
                {
                    tile = character.GetCurrentTile();
                }
                else if (go.transform.parent != null)
                {
                    tile = go.transform.parent.GetComponent<MapTile>();
                }
            }

            SetCursorHighlight(tile);
        }

        //---------------------------------------------------------------------------------------------
        private void SetCursorHighlight(MapTile toHighlight)
        {
            if (mCursorTile == toHighlight)
            {
                return;
            }

            // set previous tile to not clear
            if (mCursorTile != null)
            {
                if (MapManager.Instance.IsValidUnitPlacementTile(mCursorTile))
                {
                    if (mCursorTile.GetCharacterOnTile() != null)
                    {
                        mCursorTile.SetTileColor(Color.clear);
                    }
                    else
                    {
                        mCursorTile.SetTileColor(Color.blue);
                    }
                }
                else
                {
                    mCursorTile.SetTileColor(Color.blue);
                }
            }

            if (toHighlight != null)
            {
                if (MapManager.Instance.IsValidUnitPlacementTile(toHighlight))
                {
                    toHighlight.SetTileColor(MapConstants.CursorTileColor);
                    mCursorTile = toHighlight;
                }
                else
                {
                    mCursorTile = null;
                }
            }
            else
            {
                mCursorTile = null;
            }
        }
    }
}
