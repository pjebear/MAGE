using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

using EncounterSystem.EncounterFlow;
using Common.ActionEnums;
using Common.ActionTypes;
namespace EncounterSystem
{
    class EncounterScreenManager : MonoBehaviour
    {

        public GameObject EncounterManagerPrefab = null;
        private InputManager mEncounterManager = null;
        public Button MoveButton = null;
        public Button ActButton = null;
        public Button WaitButton = null;
        public Button ConfirmButton = null;
        public Button CancelButton = null;

        private void Awake()
        {
            mEncounterManager = Instantiate(EncounterManagerPrefab).GetComponent<InputManager>();
        }

        void Start()
        {

        }

        void Update()
        {
            
        }

        //called from objective manager
        public void OnClick_EncounterSuccessful()
        {
            Debug.Log("Encounter Completed! Returning to world");
            SceneManager.LoadScene("WorldScreen");
        }
        public void OnClick_EncounterFailed()
        {
            Debug.Log("Encounter Failed.. Returning to Title Screen");
            SceneManager.LoadScene("TitleScreen");
        }

        public void OnClick_Move()
        {
            //mEncounterManager.BeginMovementSelectionPhase();
        }

        public void OnClick_Act(CharacterActionIndex actionIndex)
        {
           // mEncounterManager.BeginActionSelectionPhase(actionIndex);
        }

        public void OnClick_Wait()
        {
            mEncounterManager.WaitSelected();
        }

        public void OnClick_Confirm()
        {
            //mEncounterManager.OnConfirm();
            ConfirmButton.gameObject.SetActive(false);
            CancelButton.gameObject.SetActive(false);
        }

        public void OnClick_Cancel()
        {
            //mEncounterManager.OnCancel();
            ConfirmButton.gameObject.SetActive(false);
            CancelButton.gameObject.SetActive(false);

        }

        public void ToggleButtonVisability(bool active)
        {
            MoveButton.gameObject.SetActive(active);
            ActButton.gameObject.SetActive(active);
            WaitButton.gameObject.SetActive(active);
        }

        public void SetMoveButtonInteractable(bool interactable)
        {
            MoveButton.interactable = interactable;
        }

        public void SetActButtonInteractable(bool interactable)
        {
            ActButton.interactable = interactable;
        }

        public void RequestConfirmation()
        {
            ConfirmButton.gameObject.SetActive(true);
            CancelButton.gameObject.SetActive(true);
        }
        void ButtonClicked(int clicked)
        {

        }

        public void AssignActions(Dictionary<ActionContainerCategory, List<ScreenActionPayload>> actions)
        {
            ActButton.GetComponent<ScreenActButtonCollapser>().AssignActionPayloads(actions);
            ActButton.onClick.AddListener(delegate { ButtonClicked(0); });
            
        }
    }
}

