using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldSystem;
using Screens.Payloads;
using EncounterSystem.Screen;
using WorldSystem.Character;
using UnityEngine.UI;

public class PartyDisplay : MonoBehaviour {


    private Dictionary<UnitSelector, UnitPanelPayload> mUnitSelectors;
    private UnitPanel mUnitPanel;

    private CharacterBase mSelectedUnit;

    public UnitSelector UnitSelectorPrefab = null;
    public UnitPanel UnitPanelPrefab = null;
    public Button TalentSelectorButton = null;
    public TalentTreeSelector FootmanTalentTree = null;

    public float StartX = 100f;
    public float StartY = -90f;
    public float XOffset = 150;
    public float YOffset = -130f;

    public float UnitPanelX = 350f;
    public float UnitPanelY = -400;

    public int MaxPerRow = 5;

    private void Awake()
    {
        mUnitSelectors = new Dictionary<UnitSelector, UnitPanelPayload>();
        
    }

    // Use this for initialization
    void Start () {

        //var partyRoster = GameInstance.Instance().GetRoster();
        //int counter = 0;
        //foreach (CharacterBase character in partyRoster.Values)
        //{
        //    float xLoc = StartX + (counter % MaxPerRow) * XOffset;
        //    float yLoc = StartY + (counter / MaxPerRow) * YOffset;
        //    UnitPanelPayload panelLayout = character.GetUnitPanelPayload();
        //    UnitSelector selector = Instantiate(UnitSelectorPrefab, transform).GetComponent<UnitSelector>();
        //    selector.GetComponent<RectTransform>().localPosition = new Vector3(xLoc, yLoc);
        //    selector.Initialize(panelLayout.UnitName, panelLayout.ImageAssetPath);
        //    selector.GetComponent<Button>().onClick.AddListener(() => { UpdateInfoPanel(panelLayout); mSelectedUnit = character; });
        //    counter++;
        //}

        //mUnitPanel = Instantiate(UnitPanelPrefab, transform).GetComponent<UnitPanel>();
        //mUnitPanel.transform.localPosition = new Vector3(UnitPanelX, UnitPanelY);
        //mUnitPanel.gameObject.SetActive(false);
        //TalentSelectorButton.onClick.AddListener(() => { _SwitchToTalentScreen(); });
        //TalentSelectorButton.gameObject.SetActive(false);

    }
	
    void UpdateInfoPanel(UnitPanelPayload payload)
    {
        mUnitPanel.DisplayUnit(payload);
        TalentSelectorButton.gameObject.SetActive(true);
    }

    public void _SwitchToTalentScreen()
    {
        gameObject.SetActive(false);
       FootmanTalentTree.Initialize(mSelectedUnit);
    }
}
