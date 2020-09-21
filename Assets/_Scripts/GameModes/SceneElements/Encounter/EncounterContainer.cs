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
        public EncounterScenarioId EncounterScenarioId;

        public int MaxUserPlayers;
        public PlacementInfo PlacementInfo;
        public Transform Tiles;
        public Transform AlliesContainer;
        public Transform EnemiesContainer;
        public bool IsEncounterPending;

        private void Awake()
        {
            EnableHeirarchy(false);
        }

        private void Start()
        {
            if (LevelManagementService.Get().GetEncounterInfo((int)EncounterScenarioId).IsActive)
            {
                EncounterEnabled();
            }
            Messaging.MessageRouter.Instance.RegisterHandler(this);
        }

        void OnDestroy()
        {
            Messaging.MessageRouter.Instance.UnRegisterHandler(this);
        }

        public void EncounterEnabled()
        {
            EncounterTriggered();
        }

        public void EncounterTriggered()
        {
            IsEncounterPending = true;
            LevelManagement.LevelMessage availableMessage = new LevelManagement.LevelMessage(LevelManagement.MessageType.EncounterAvailable, this);
            Messaging.MessageRouter.Instance.NotifyMessage(availableMessage);
        }

        public void StartEncounter()
        {
            IsEncounterPending = false;
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
                        if (LevelManagementService.Get().GetEncounterInfo((int)EncounterScenarioId).IsActive)
                        {
                            EncounterEnabled();
                        }
                        else
                        {
                            EnableHeirarchy(false);
                        }
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
    }
}
