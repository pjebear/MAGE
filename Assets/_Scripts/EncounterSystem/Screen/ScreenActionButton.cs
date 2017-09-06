using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Common.ActionTypes;

namespace EncounterSystem
{
    class ScreenActionButton : MonoBehaviour
    {
        private Text ButtonText;
        public CharacterActionIndex ActionIndex;
        public EncounterScreenManager mEncounterScreenManager;

        private void Awake()
        {
            ButtonText = GetComponentInChildren<Text>();
            GetComponent<Button>().onClick.AddListener(() => GameObject.Find("EncounterScreenManager").GetComponent<EncounterScreenManager>().OnClick_Act(ActionIndex));
        }

        private void Start()
        {

        }

        public void AssignAction(ScreenActionPayload action)
        {
            ActionIndex = action.ActionIndex;
            ButtonText.text = action.ActionName + "  " + action.ActionCost.Value.ToString();
        }
    }
}
