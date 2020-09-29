using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using MAGE.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAGE.GameModes
{
    class ModulesContainer : MonoBehaviour
    {
        

        public static ModulesContainer Container = null;

        
        public UIManager UIManager;

        public bool DebugEncounter = false;
        public bool DebugExplore = false;


        private ActorLoader mActorLoader;
        private AudioManager mAudioManager;
        private FlowControl.FlowManager mFlowManager;
        public GameSystemModule mGameSystemModule;

        private void Awake()
        {
            if (Container != null)
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                Container = this;

                DontDestroyOnLoad(gameObject);

                Logger.LogFilters[(int)LogTag.Assets] = false;
                Logger.LogFilters[(int)LogTag.DB] = false;

                Messaging.MessageRouter.Instance = gameObject.AddComponent<Messaging.MessageRouter>();
                GameObject flowControlContainer = new GameObject("FlowControls");
                flowControlContainer.transform.SetParent(transform);
                mFlowManager = flowControlContainer.AddComponent<FlowControl.FlowManager>();
                mActorLoader = gameObject.AddComponent<ActorLoader>();
                mAudioManager = gameObject.AddComponent<AudioManager>();
                mGameSystemModule = gameObject.AddComponent<GameSystemModule>();

                mGameSystemModule.InitModule();
                UIManager.Initialize();
                mFlowManager.Init();

                //if (DebugEncounter)
                //{
                //    MAGE.GameSystems.WorldService.Get().PrepareNewGame();
                //    EncounterCreateParams encounterCreateParams = new EncounterCreateParams();
                //    encounterCreateParams.BottomLeft = new TileIdx(12, 4);
                //    encounterCreateParams.TopRight = new TileIdx(24, 8);
                //    MAGE.GameSystems.WorldService.Get().PrepareEncounter(encounterCreateParams);
                //    GameModesModule.Encounter();
                //}
                //else if (DebugExplore)
                //{
                //    MAGE.GameSystems.WorldService.Get().PrepareNewGame();
                //    GameModesModule.Explore();
                //}
                //else 
                //{
                //    GameModesModule.MainMenu();
                //}
            }

           
        }

        private void Start()
        {
            mFlowManager.BeginFlow(new FlowControl.MainFlow());
        }
    }
}

