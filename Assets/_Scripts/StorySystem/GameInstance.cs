using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using UnityEngine;


/// <summary>
/// Base Class assigned to each save file the character creates.
/// Contains all story/encounter specific data for the players party
/// </summary>
/// 

using WorldSystem.Interface;
using StorySystem.Interface;
using EncounterSystem.Interface;
using UnityEngine;
using WorldSystem.Managers.WorldTransitions;
using Common.EncounterEnums;

namespace WorldSystem
{
    class GameInstance : MonoBehaviour
    {
        private string mFileLocation;
        private static GameInstance mInstance;
        private WorldSystemFacade mWorldSystem;
        private StorySystemFacade mStorySystem;
        private EncounterSystemFacade mEncounterSystem;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            mWorldSystem = new WorldSystemFacade();
            mStorySystem = new StorySystemFacade();
            mEncounterSystem = new EncounterSystemFacade();
        }

        public void DebugFlow()
        {
            mWorldSystem.Initialize();
            mStorySystem.Initialize();
            mEncounterSystem.Initialize();
            mWorldSystem.DebugFlow();
        }

        public void BeginGame()
        {
            mWorldSystem.Initialize();
            mStorySystem.Initialize();
            mEncounterSystem.Initialize();

            mStorySystem.BeginNewStoryArc(StorySystem.Common.StoryArcId.Main);
        }

        public void SaveGameInstance()
        {
            //empty
            UnityEngine.Debug.Log("Saving Game Instance to " + mFileLocation + " ...");
        }

        public void DestroyInstance()
        {
            UnityEngine.Debug.Log("Destroying Game Instance ...");
            mInstance = null;
        }

        private void LoadGameInstance()
        {
            //load from mFileLocation
            //for each instancemanager-> load
            UnityEngine.Debug.Log("Loading Game Instance to " + mFileLocation + " ...");
        }
    }
}


