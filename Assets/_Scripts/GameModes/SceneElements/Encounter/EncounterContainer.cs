using MAGE.GameModes.Combat;
using MAGE.GameModes.Encounter;
using MAGE.GameModes.SceneElements.Encounters;
using MAGE.GameSystems;
using MAGE.GameSystems.Mobs;
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
        public EncounterModel EncounterModel = new EncounterModel();

        // potential new script
        public EncounterScenarioId EncounterScenarioId = EncounterScenarioId.INVALID;
        public List<MobId> MobsInEncounter = new List<MobId>();
        public List<ItemId> ItemRewards = new List<ItemId>();
        public int CoinReward = 0;

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
            EncounterModel.IsEncounterActive = true;

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

        void OnTriggerEnter(Collider collider)
        {
            MobControl entered = collider.gameObject.GetComponent<MobControl>();
            if (entered == null)
            {
                entered = collider.gameObject.GetComponentInParent<MobControl>();
            }

            if (entered != null)
            {
                Logger.Log(LogTag.GameModes, "EncounterContainer", "MobControlEntered");

                if (mIsEncounterPending || EncounterModel.IsEncounterActive)
                {
                    MobsInEncounter.Add(entered.GetComponent<MobCharacterControl>().MobId);

                    Level level = LevelManagementService.Get().GetLoadedLevel();
                    ControllableEntity combatCharacter = level.CreateCombatCharacter(entered.transform.position, entered.transform.rotation, Enemies);
                    
                    // Triggers the addition to EncounterModel
                    combatCharacter.GetComponent<CharacterPickerControl>().CharacterId = entered.GetComponent<CharacterPickerControl>().CharacterId;

                    entered.gameObject.SetActive(false);
                }
            }
        }
    }
}
