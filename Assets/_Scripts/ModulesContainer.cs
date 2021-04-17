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

                Logger.LogFilters[(int)LogTag.Assets] = true;
                Logger.LogFilters[(int)LogTag.DB] = true;

                Messaging.MessageRouter.Instance = gameObject.AddComponent<Messaging.MessageRouter>();
                GameObject flowControlContainer = new GameObject("FlowControls");
                flowControlContainer.transform.SetParent(transform);
                mFlowManager = flowControlContainer.AddComponent<FlowControl.FlowManager>();
                mActorLoader = gameObject.AddComponent<ActorLoader>();
                mAudioManager = gameObject.AddComponent<AudioManager>();
                mGameSystemModule = gameObject.AddComponent<GameSystemModule>();

                mGameSystemModule.InitModule();
                UIManager.Initialize();
                UIManager.Fade(false, 0);
                mFlowManager.Init();
            }
        }

        private void Start()
        {
            mFlowManager.BeginFlow(new FlowControl.MainFlow());
        }
    }
}

