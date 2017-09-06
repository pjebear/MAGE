using Common.EncounterEnums;
using Common.UnitTypes;
using EncounterSystem.Interface;
using StorySystem.Common;
using StorySystem.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                mInstance = new WorldSystemFacade();
                UnityEngine.Debug.Log("Accessing World System Facade before properly initialized");
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

        public void DebugFlow()
        {
            EncounterSystemFacade.Instance().BeginEncounter(EncounterScenarioId.Debug, Common.MapLocationId.INVALID, mPartyManager.GetRoster());
        }

        public void AddCharacterToRoster(CharacterBase character)
        {
            mPartyManager.AddCharacter(character);
        }

        public void RemoveCharacterFromRoster(CharacterBase character)
        {
            mPartyManager.RemoveCharacter(character);
        }

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

        public UnitRoster GetPlayerRoster()
        {
            return mPartyManager.GetRoster();
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
