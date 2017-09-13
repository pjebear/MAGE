using Common.EncounterEnums;
using Common.EquipmentEnums;
using Common.EquipmentTypes;
using Common.UnitTypes;
using EncounterSystem.Interface;
using Screens.Payloads;
using StorySystem.Common;
using StorySystem.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using WorldSystem.Character;
using WorldSystem.Managers;
using WorldSystem.Managers.WorldTransitions;

namespace WorldSystem.Interface
{
    class WorldSystemFacade
    {
        private PartyManager mPartyManager;
        private TransitionManager mTransitionManager;

        static WorldSystemFacade mInstance;

        public static WorldSystemFacade Instance()
        {
            if (mInstance == null)
            {
                UnityEngine.Debug.Log("Accessing World System Facade before properly initialized");
                GameObject go = new GameObject();
                UnityEngine.Object.DontDestroyOnLoad(go);
                GameInstance gi = go.AddComponent<GameInstance>();
                gi.DebugFlow();
            }
            return mInstance;
        }

        public WorldSystemFacade()
        {
            UnityEngine.Debug.Log("Creating WorldSystemFacade...");
            mInstance = this;
            mPartyManager = new PartyManager();
            mTransitionManager = new TransitionManager();
        }

        public void Initialize()
        {
            //mPartyManager.Initialize();
            mTransitionManager.Initialize(new MapTransition());
        }

        public void DebugEncounterFlow()
        {
            EncounterSystemFacade.Instance().BeginEncounter(EncounterScenarioId.Debug, Common.MapLocationId.INVALID, mPartyManager.GetRoster());
        }

        public void DebugRosterFlow()
        {
            mTransitionManager.SetNextTransition(new RosterScreenTransition());
            mTransitionManager.Transition();
        }

        #region RosterManagement
        public void AddCharacterToRoster(CharacterBase character)
        {
            mPartyManager.AddCharacter(character);
        }

        public void RemoveCharacterFromRoster(CharacterBase character)
        {
            mPartyManager.RemoveCharacter(character);
        }

        public List<InventoryEquipmentPayload> GetValidEquipmentForSlot(CharacterBase forCharacter, WornEquipmentSlot equipmentSlot)
        {
            return mPartyManager.GetValidEquipmentForSlot(forCharacter, equipmentSlot);
        }

        public void EquipCharacterInSlot(int characterId, KeyValuePair<int,int> equipmentIndex, WornEquipmentSlot slot)
        {
            mPartyManager.EquipCharacterInSlot(characterId, equipmentIndex, slot);
        }

        public void UnequipCharacterSlot(int characterId, WornEquipmentSlot slot)
        {
            mPartyManager.UnequipCharacterSlot(characterId, slot);
        }

        public void AddEquipmentToInventory(EquipmentBase equipment)
        {
            mPartyManager.AddEquipmentToInventory(equipment);
        }

        public UnitRoster GetPlayerRoster()
        {
            return mPartyManager.GetRoster();
        }

        #endregion

        #region EncounterManagement
        public void BeginRandomEncounter()
        {
            mTransitionManager.SetNextTransition(new EncounterTransition(EncounterScenarioId.Random));
            mTransitionManager.Transition();
        }

        public void EncounterComplete(EncounterSuccessState successState, EncounterScenarioId encounterId)
        {
            if (successState == EncounterSuccessState.Loss)
            {
                mTransitionManager.SetNextTransition(new MapTransition());
            }
            else
            {
                mPartyManager.ResetPostEncounter();
            }

            StorySystemFacade.Instance().NotifyStoryEvent(new StoryEventPayload(StoryEventType.Encounter, (int)encounterId));
            mTransitionManager.TransitionFinished();
        }

        #endregion

        public void OpenPartyScreen()
        {
            mTransitionManager.SetNextTransition(new RosterScreenTransition());
            mTransitionManager.Transition();
        }

        public void PartyScreenFinished()
        {
            mTransitionManager.TransitionFinished();
        }

        public void SetNextTransition(WorldTransitionBase transition)
        {
            mTransitionManager.SetNextTransition(transition);
        }

        public void ProgressStory()
        {
            mTransitionManager.Transition();
        }
    }
}
