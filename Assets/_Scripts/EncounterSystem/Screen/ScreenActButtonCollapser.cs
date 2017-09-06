using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Common.ActionEnums;
using Common.ActionTypes;
namespace EncounterSystem
{
    class ScreenActButtonCollapser : MonoBehaviour
    {

        private bool mCollapsed;
        private Button mButton;
        public ScreenActionButton ScreenActionButtonPrefab;
        List<ScreenActionButton> mCollapsableList;
        Dictionary<ActionContainerCategory, List<ScreenActionPayload>> mActionPayloads;
        int TEMP_SIZE_OF_LIST;
        // Use this for initialization
        private void Awake()
        {
            mButton = GetComponent<Button>();
            mCollapsableList = new List<ScreenActionButton>();
            float height = ScreenActionButtonPrefab.GetComponent<RectTransform>().rect.height;
            float width = ScreenActionButtonPrefab.GetComponent<RectTransform>().rect.width;
            for (int i = 0; i < 10; i++)
            {
                ScreenActionButton actionButton = Instantiate(ScreenActionButtonPrefab, transform);
                actionButton.transform.localPosition = new Vector3(width, (i + 1) * -height, 0);
                actionButton.gameObject.SetActive(false);
                mCollapsableList.Add(actionButton);
            }
        }

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void AssignActionPayloads(Dictionary<ActionContainerCategory, List<ScreenActionPayload>> actionPayloads)
        {
            TEMP_SIZE_OF_LIST = 0;
            mActionPayloads = actionPayloads;
            foreach (var actionList in mActionPayloads)
            {
                foreach (ScreenActionPayload action in actionList.Value)
                {
                    mCollapsableList[TEMP_SIZE_OF_LIST].AssignAction(action);
                    mCollapsableList[TEMP_SIZE_OF_LIST].GetComponent<Button>().interactable = action.CanUse;
                    TEMP_SIZE_OF_LIST++;
                }
            }
        }

        public void ToggleCollapse()
        {
            mCollapsed = !mCollapsed;
            for (int i = 0; i < TEMP_SIZE_OF_LIST; i++)
            {
                mCollapsableList[i].gameObject.SetActive(mCollapsed);
            }
        }
    }
}

