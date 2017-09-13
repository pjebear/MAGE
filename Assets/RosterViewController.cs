using Common.UnitTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldSystem.Interface;

namespace Screens.Roster
{
    class RosterViewController : MonoBehaviour
    {
        public enum ScreenIndex
        {
            Invalid = -1,

            UnitSelection,
            Profession,
            Equipment,
            Status,
            
            NUM
        }

        public UnitSelectorManager UnitSelectorManager;
        public UnitProfessionManager UnitProfessionManager;
        public UnitEquipmentManager UnitEquipmentManager;
        public UnitStatusManager UnitStatusManager;
        public UnitBasePanel BasePanel;

        private ScreenIndex mCurrentIndex;
        private UnitRoster rRoster;

        private void Awake()
        {
            mCurrentIndex = ScreenIndex.Invalid;
        }

        // Use this for initialization


        // Use this for initialization
        void Start()
        {
            rRoster = WorldSystemFacade.Instance().GetPlayerRoster();
            UnitSelectorManager.Initialize(rRoster, () => { SetCurrentScreen(ScreenIndex.Status); }, () => { SetCurrentScreen(ScreenIndex.Equipment); }, () => { SetCurrentScreen(ScreenIndex.Profession); });

            UnitStatusManager.Initialize(() => { SetCurrentScreen(ScreenIndex.UnitSelection); });
            UnitStatusManager.gameObject.SetActive(false);

            UnitEquipmentManager.Initialize(() => { SetCurrentScreen(ScreenIndex.UnitSelection); });
            UnitEquipmentManager.gameObject.SetActive(false);

            BasePanel.gameObject.SetActive(false);
            SetCurrentScreen(ScreenIndex.UnitSelection);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (mCurrentIndex == ScreenIndex.UnitSelection)
                {
                    WorldSystemFacade.Instance().PartyScreenFinished();
                }
            }
        }

        public void SetCurrentScreen(ScreenIndex index)
        {
            // deactivate previous Screen
            switch (mCurrentIndex)
            {
                case (ScreenIndex.UnitSelection):
                    UnitSelectorManager.Hide();
                    UnitSelectorManager.gameObject.SetActive(false);
                    break;

                case (ScreenIndex.Equipment):
                    //UnitEquipmentManager.Hide();
                    UnitEquipmentManager.gameObject.SetActive(false);
                    break;

                case (ScreenIndex.Status):
                    //UnitStatusManager.Hide();
                    UnitStatusManager.gameObject.SetActive(false);
                    break;

                case (ScreenIndex.Profession):
                    //UnitProfessionManager.Hide();
                    UnitProfessionManager.gameObject.SetActive(false);
                    break;
            }

            mCurrentIndex = index;
            switch (mCurrentIndex)
            {
                case (ScreenIndex.UnitSelection):
                    BasePanel.gameObject.SetActive(false);
                    UnitSelectorManager.gameObject.SetActive(true);
                    UnitSelectorManager.Refresh(rRoster);
                    break;

                case (ScreenIndex.Equipment):
                    BasePanel.gameObject.SetActive(true);
                    BasePanel.DisplayUnit(rRoster.Roster[UnitSelectorManager.SelectedUnit].GetUnitPanelPayload());
                    UnitEquipmentManager.gameObject.SetActive(true);
                    UnitEquipmentManager.DisplayUnit(rRoster.Roster[UnitSelectorManager.SelectedUnit]);
                    break;

                case (ScreenIndex.Status):
                    BasePanel.gameObject.SetActive(true);
                    BasePanel.DisplayUnit(rRoster.Roster[UnitSelectorManager.SelectedUnit].GetUnitPanelPayload());
                    UnitStatusManager.gameObject.SetActive(true);
                    UnitStatusManager.DisplayUnit(rRoster.Roster[UnitSelectorManager.SelectedUnit]);
                    break;

                case (ScreenIndex.Profession):
                    BasePanel.gameObject.SetActive(true);
                    BasePanel.DisplayUnit(rRoster.Roster[UnitSelectorManager.SelectedUnit].GetUnitPanelPayload());
                    UnitProfessionManager.gameObject.SetActive(true);
                    break;
            }
        }
    }
}

