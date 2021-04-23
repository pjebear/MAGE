using MAGE.GameModes.Combat;
using MAGE.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    class EncounterContainer 
        : MonoBehaviour
        , Messaging.IMessageHandler
    {
        public EncounterScenarioId EncounterScenarioId = EncounterScenarioId.INVALID;

        public Transform Enemies;
        public Transform Allys;
        public Transform WinConditions;
        public Transform LoseConditions;

        private bool mIsEncounterPending = false;
        private bool mIsEncounterVisible = true;

        private void Awake()
        {
            EnableHeirarchy(false);
        }

        private void Start()
        {
            Messaging.MessageRouter.Instance.RegisterHandler(this);
        }

        void OnDestroy()
        {
            Messaging.MessageRouter.Instance.UnRegisterHandler(this);
        }

        public bool GetIsEncounterPending()
        {
            return mIsEncounterPending;
        }

        public void EncounterEnabled()
        {
            EncounterTriggered();
        }

        public void EncounterTriggered()
        {
            mIsEncounterPending = true;
            LevelManagement.LevelMessage availableMessage = new LevelManagement.LevelMessage(LevelManagement.MessageType.EncounterAvailable, this);
            Messaging.MessageRouter.Instance.NotifyMessage(availableMessage);
        }

        public void StartEncounter()
        {
            mIsEncounterPending = false;
            EnableHeirarchy(true);
        }

        public void HandleMessage(MessageInfoBase eventInfoBase)
        {
            if (eventInfoBase.MessageId == LevelManagement.LevelMessage.Id)
            {
                LevelManagement.LevelMessage levelMessage = eventInfoBase as LevelManagement.LevelMessage;
                if (levelMessage.Type == LevelManagement.MessageType.EncounterUpdated)
                {
                    int encounterId = levelMessage.Arg<int>();
                    if (encounterId == (int)EncounterScenarioId)
                    {
                        EncounterInfo info = LevelManagementService.Get().GetEncounterInfo((int)EncounterScenarioId);

                        EnableHeirarchy(info.IsVisible);
                        if (info.IsActive)
                        {
                            EncounterTriggered();
                        }
                    }
                }
                else if (levelMessage.Type == LevelManagement.MessageType.LevelLoaded)
                {
                    EncounterInfo info = LevelManagementService.Get().GetEncounterInfo((int)EncounterScenarioId);

                    EnableHeirarchy(info.IsVisible);
                    if (info.IsActive)
                    {
                        EncounterTriggered();
                    }
                }
            }
        }

        private void EnableHeirarchy(bool enable)
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                transform.GetChild(i).gameObject.SetActive(enable);
            }
        }

        void Update()
        {
            
        }
    }
}
