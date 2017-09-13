using Common.UnitTypes;
using Screens.Payloads;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WorldSystem.Character;

namespace Screens.Roster
{
    class UnitSelectorManager : MonoBehaviour
    {
        public float XPadding = 50f;
        public float YPadding = 50f;
        public float XStagger = 50f;
        public float YStagger = 50f;
        public UnitSelector UnitSelectorPrefab;
        public GameObject UnitPanelParent;
        public Text UnitName;
        public Text UnitProfession;
        public Text UnitLevel;
        public Image UnitImage;
        public Image UnitAllignment;
        public GameObject UnitMenu;
        public GameObject UnitSelectorParent;
        public Button StatusButton;
        public Button EquipmentButton;
        public Button ProfessionButton;

        public int SelectedUnit { get; private set; }
        private Dictionary<int, UnitSelector> mUnitSelectorLookup;
        private List<List<UnitSelector>> mUnitSelectorArray;

        // Use this for initialization
        void Awake()
        {
            mUnitSelectorLookup = new Dictionary<int, UnitSelector>();
            mUnitSelectorArray = new List<List<UnitSelector>>();
            mUnitSelectorArray.Add(new List<UnitSelector>());
        }

        public void Initialize(UnitRoster roster, Action _statusClick, Action _equipmentClick, Action _professionClick)
        {
            int xCount = 0;
            int yCount = 0;

            foreach (CharacterBase character in roster.Roster.Values)
            {
                float xPos = XPadding + (XStagger + UnitSelector.SelectorWidth) * xCount;
                if (xPos + UnitSelector.SelectorWidth > Screen.width)
                {
                    mUnitSelectorArray.Add(new List<UnitSelector>());
                    xPos = XPadding;
                    xCount = 0;
                    yCount++;
                }
                float yPos = YPadding + (YStagger + UnitSelector.SelectorHeight) * -(yCount + 1);
                xCount++;
                UnitPanelPayload payload = character.GetUnitPanelPayload();
                UnitSelector selector = Instantiate(UnitSelectorPrefab).GetComponent<UnitSelector>();
                selector.Initialize(
                    Resources.Load<Sprite>(payload.ImageAssetPath),
                    () => { _OnHover(payload.UnitName, payload.ProfessionName, payload.CharacterLevel, Resources.Load<Sprite>(payload.AllignmentAssetPath), Resources.Load<Sprite>(payload.ImageAssetPath)); },
                    () => { _OnHoverExit(); },
                    () => { _OnSelect(character.CharacterID); });
                selector.transform.SetParent(UnitSelectorParent.transform);
                selector.transform.localPosition = new Vector3(xPos, yPos, 0);

                mUnitSelectorLookup.Add(character.CharacterID, selector);
                mUnitSelectorArray[yCount].Add(selector);
            }
            StatusButton.onClick.AddListener(() => { _statusClick(); });
            EquipmentButton.onClick.AddListener(() => { _equipmentClick(); });
            ProfessionButton.onClick.AddListener(() => { _professionClick(); });

            UnitMenu.gameObject.SetActive(false);
            UnitPanelParent.gameObject.SetActive(false);
        }

        public void Refresh(UnitRoster roster)
        {
            foreach (CharacterBase character in roster.Roster.Values)
            {
                UnitPanelPayload payload = character.GetUnitPanelPayload();
                UnitSelector selector = mUnitSelectorLookup[character.CharacterID];
                selector.Initialize(
                    Resources.Load<Sprite>(payload.ImageAssetPath),
                    () => { _OnHover(payload.UnitName, payload.ProfessionName, payload.CharacterLevel, Resources.Load<Sprite>(payload.AllignmentAssetPath), Resources.Load<Sprite>(payload.ImageAssetPath)); },
                    () => { _OnHoverExit(); },
                    () => { _OnSelect(character.CharacterID); });
            }
        }

        public void Hide()
        {
            UnitMenu.gameObject.SetActive(false);
            UnitPanelParent.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

            if (Input.GetMouseButtonDown(1))
            {
                OnBack();
            }
        }

        private void _OnHover(string characterName, string profession, int level, Sprite allignment, Sprite characterImage)
        {
            if (!UnitMenu.gameObject.activeInHierarchy)
            {
                Debug.Log("OnHover");
                UnitPanelParent.gameObject.SetActive(true);
                UnitName.text = characterName;
                UnitProfession.text = profession;
                UnitLevel.text = "Lv. " + level;
                UnitAllignment.sprite = allignment;
                UnitImage.sprite = characterImage;
            }
        }

        private void _OnHoverExit()
        {
            if (!UnitMenu.gameObject.activeInHierarchy)
            {
                Debug.Log("_OnHoverExit");
                UnitPanelParent.gameObject.SetActive(false);
            }
        }

        private void _OnSelect(int characterId)
        {
            if (!UnitMenu.gameObject.activeInHierarchy)
            {
                Debug.Log("_OnSelect");
                UnitMenu.gameObject.SetActive(true);
                UnitMenu.transform.position = Input.mousePosition;
                SelectedUnit = characterId;
            }
        }

        private void OnBack()
        {
            Debug.Log("OnBack");
            UnitMenu.gameObject.SetActive(false);
        }
    }
}

