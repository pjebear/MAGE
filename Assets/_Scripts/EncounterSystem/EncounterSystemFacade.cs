using Common.EncounterEnums;
using Common.EncounterTypes;
using Common.UnitTypes;
using EncounterSystem.Factory;
using UnityEngine;
using WorldSystem;
using WorldSystem.Common;
using WorldSystem.Interface;

namespace EncounterSystem.Interface
{
    class EncounterSystemFacade
    {
        private static EncounterSystemFacade mInstance;
        public static EncounterSystemFacade Instance()
        {
            if (mInstance == null)
            {
                UnityEngine.Debug.Log("Asking for EncounterSystemFacade before initialized");
                GameObject go = new GameObject();
                Object.DontDestroyOnLoad(go);
                GameInstance gi = go.AddComponent<GameInstance>();
                gi.DebugFlow();
            }
            return mInstance;
        }

        public EncounterSystemFacade()
        {
            UnityEngine.Debug.Log("Creating EncounterSystemsFacade...");
            mInstance = this;
        }

        public void Initialize()
        {

        }

        // Called from World Systems
        public void BeginEncounter(EncounterScenarioId toLoad, MapLocationId currentLocation, UnitRoster unitRoster)
        {
            EncounterFactory.CheckoutEncounterBluePrint(toLoad, currentLocation, unitRoster, out mEncounterBluePrint);
            mEncounterState = new EncounterState();
        }

        public void EncounterFinished()
        {
            WorldSystemFacade.Instance().EncounterComplete(mEncounterState.SuccessState, mEncounterBluePrint.ScenarioId);
        }

        // Called from Encounter System
        public EncounterBluePrint GetEncounterBluePrint()
        {
            return mEncounterBluePrint;
        }

        public EncounterState GetEncounterState()
        {
            return mEncounterState;
        }

        private EncounterBluePrint mEncounterBluePrint;
        private EncounterState mEncounterState;
    }
}
