﻿using MAGE.GameModes.SceneElements;
using MAGE.GameServices;
using MAGE.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAGE.GameModes
{
    class ModulesContainer : MonoBehaviour
    {
        public static ModulesContainer Container = null;

        public GameModesModule GameModesModule;
        public GameSystemModule GameSystemModule;
        public UIManager UIManager;

        public bool DebugEncounter = false;

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

                Messaging.MessageRouter.Instance = gameObject.AddComponent<Messaging.MessageRouter>();

                GameSystemModule.InitModule();
                GameModesModule.InitModule();
                UIManager.Initialize();

                if (DebugEncounter)
                {
                    MAGE.GameServices.WorldService.Get().PrepareNewGame();
                    EncounterCreateParams encounterCreateParams = new EncounterCreateParams();
                    encounterCreateParams.BottomLeft = new TileIdx(12, 4);
                    encounterCreateParams.TopRight = new TileIdx(24, 8);
                    MAGE.GameServices.WorldService.Get().PrepareEncounter(encounterCreateParams);
                    GameModesModule.Encounter();
                }
                else
                {
                    GameModesModule.MainMenu();
                }
            }
        }
    }
}

