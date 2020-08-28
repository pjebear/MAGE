using MAGE.GameModes.LevelManagement;
using MAGE.GameModes.SceneElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes
{
    class GameModesModule
        : MonoBehaviour
        , Messaging.IMessageHandler
    {
        private string TAG = "GameModesModule";

        public static GameModesModule Instance;

        public static ActorLoader ActorLoader;
        public static AudioManager AudioManager;

        private GameModeType mPendingGameMode = GameModeType.INVALID;
        private GameModeBase mLoadedGameMode = null;

        private AssetLoader<GameModeBase> mGameModeLoader = null;

        public void InitModule()
        {
            Logger.Log(LogTag.GameModes, TAG, "::InitModule()");
            Logger.Assert(Instance == null, LogTag.GameModes, TAG, "::InitModule() - Already initialized!");

            Logger.LogFilters[(int)LogTag.Assets] = false;
            Logger.LogFilters[(int)LogTag.DB] = false;

            Instance = this;
            ActorLoader = gameObject.AddComponent<ActorLoader>();
            AudioManager = GetComponent<AudioManager>();

            Messaging.MessageRouter.Instance.RegisterHandler(this);

            mGameModeLoader = new AssetLoader<GameModeBase>("GameModes");
            mGameModeLoader.LoadAssets();
        }

        private void OnDestroy()
        {
            Messaging.MessageRouter.Instance.UnRegisterHandler(this);
        }

        // Load logic 
        public void TransitionTo(GameModeType gameMode)
        {
            mPendingGameMode = gameMode;

            if (mLoadedGameMode != null)
            {
                Logger.Log(LogTag.GameModes, TAG, string.Format("::TransitionTo() - Current [{0}] New [{1}]", mLoadedGameMode.GetGameModeType().ToString(), mPendingGameMode.ToString()));

                Messaging.MessageRouter.Instance.NotifyMessage(new GameModeMessage(GameModeMessage.EventType.ModeEnd));
                Messaging.MessageRouter.Instance.NotifyMessage(new GameModeMessage(GameModeMessage.EventType.ModeTakedown_Begin));
            }
            else
            {
                Logger.Log(LogTag.GameModes, TAG, string.Format("::TransitionTo() - Current [NONE] New [{0}]", mPendingGameMode.ToString()));
                Messaging.MessageRouter.Instance.NotifyMessage(new GameModeMessage(GameModeMessage.EventType.UITakedown_Begin));
            }
        }

        public void LoadGameMode(GameModeType gameMode)
        {
            Instantiate(mGameModeLoader.GetAsset(gameMode.ToString()), transform);

            // Continue loading on 'NotifyGameModeLoaded'
        }

        public void NotifyGameModeLoaded(GameModeBase gameMode)
        {
            Logger.Assert(mLoadedGameMode == null, LogTag.GameModes, TAG, "::NotifyGameModeLoaded() - GameMode already loaded", LogLevel.Warning);

            mLoadedGameMode = gameMode;
            mLoadedGameMode.Init();
            GameSystems.LevelId levelId = mLoadedGameMode.GetLevelId();

            MAGE.GameModes.ILevelManagerService levelManagerService = MAGE.GameModes.LevelManagementService.Get();
            if (levelId != GameSystems.LevelId.INVALID)
            {
                Level loadedLevel = levelManagerService.GetLoadedLevel();
                if (loadedLevel == null || loadedLevel.LevelId != levelId)
                {
                    levelManagerService.LoadLevel(levelId);
                }
            }
            else
            {
                levelManagerService.UnloadLevel();
            }

            Messaging.MessageRouter.Instance.NotifyMessage(new GameModeMessage(GameModeMessage.EventType.UISetup_Begin));
        }

        // IMessageHandler
        public void HandleMessage(Messaging.MessageInfoBase messageInfo)
        {
            switch (messageInfo.MessageId)
            {
                case GameModeMessage.Id:
                {
                    GameModeMessage gameModeMessage = messageInfo as GameModeMessage;

                    switch (gameModeMessage.Type)
                    {
                        case GameModeMessage.EventType.UISetup_Complete:
                        {
                            Logger.Log(LogTag.GameModes, TAG, "::HandleMessage() - " + gameModeMessage.Type.ToString());

                            Messaging.MessageRouter.Instance.NotifyMessage(new GameModeMessage(GameModeMessage.EventType.ModeSetup_Begin));
                        }
                        break;

                        case GameModeMessage.EventType.ModeSetup_Complete:
                        {
                            Logger.Log(LogTag.GameModes, TAG, "::HandleMessage() - " + gameModeMessage.Type.ToString());

                            Messaging.MessageRouter.Instance.NotifyMessage(new GameModeMessage(GameModeMessage.EventType.ModeStart));
                        }
                        break;

                        case GameModeMessage.EventType.ModeTakedown_Complete:
                        {
                            Logger.Log(LogTag.GameModes, TAG, "::HandleMessage() - " + gameModeMessage.Type.ToString());

                            Destroy(mLoadedGameMode.gameObject);

                            Messaging.MessageRouter.Instance.NotifyMessage(new GameModeMessage(GameModeMessage.EventType.UITakedown_Begin));
                        }

                        break;

                        case GameModeMessage.EventType.UITakedown_Complete:
                        {
                            Logger.Log(LogTag.GameModes, TAG, "::HandleMessage() - " + gameModeMessage.Type.ToString());

                            if (mPendingGameMode != GameModeType.INVALID)
                            {
                                GameModeType toLoad = mPendingGameMode;
                                mPendingGameMode = GameModeType.INVALID;
                                LoadGameMode(toLoad);
                            }
                        }
                        break;
                    }
                }
                break;
            }
        }

        // Debug
        public void MainMenu()
        {
            TransitionTo(GameModeType.MainMenu);
        }

        public void Explore()
        {
            TransitionTo(GameModeType.Exploration);
        }

        public void Encounter()
        {
            TransitionTo(GameModeType.Encounter);
        }

        public void Outfit()
        {
            TransitionTo(GameModeType.PartyOutfiter);
        }

        public void Quit()
        {
            TransitionTo(GameModeType.MainMenu);
        }
    }
}


