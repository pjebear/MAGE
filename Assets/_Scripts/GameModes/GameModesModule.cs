﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

class GameModesModule 
    : IAssetManager<GameModeBase>
    , IEventHandler<GameModeEvent>
{
    private string TAG = "GameModesModule";

    public static ActorLoader ActorLoader;
    public static GameModesModule Instance;

    private GameModeType mPendingGameMode = GameModeType.INVALID;
    private GameModeBase mLoadedGameMode = null;

    public void InitModule()
    {
        Logger.Log(LogTag.GameModes, TAG, "::InitModule()");
        Logger.Assert(Instance == null, LogTag.GameModes, TAG, "::InitModule() - Already initialized!");

        Logger.LogFilters[(int)LogTag.Assets] = false;
        Logger.LogFilters[(int)LogTag.DB] = false;

        Instance = this;
        ActorLoader = gameObject.AddComponent<ActorLoader>();

        GameModeEventRouter.Instance.RegisterHandler(this);

        // IAssetManager
        InitializeAssets();

    }

    private void OnDestroy()
    {
        GameModeEventRouter.Instance.UnRegisterListener(this);
    }

    // Load logic 
    public void TransitionTo(GameModeType gameMode)
    {
        mPendingGameMode = gameMode;

        if (mLoadedGameMode != null)
        {
            Logger.Log(LogTag.GameModes, TAG, string.Format("::TransitionTo() - Current [{0}] New [{1}]", mLoadedGameMode.GetGameModeType().ToString(), mPendingGameMode.ToString()));

            GameModeEventRouter.Instance.NotifyEvent(new GameModeEvent(GameModeEvent.EventType.ModeEnd));
            GameModeEventRouter.Instance.NotifyEvent(new GameModeEvent(GameModeEvent.EventType.ModeTakedown_Begin));
        }
        else
        {
            Logger.Log(LogTag.GameModes, TAG, string.Format("::TransitionTo() - Current [NONE] New [{0}]", mPendingGameMode.ToString()));
            GameModeEventRouter.Instance.NotifyEvent(new GameModeEvent(GameModeEvent.EventType.UITakedown_Begin));
        }
    }

    public void LoadGameMode(GameModeType gameMode)
    {
        SceneManager.LoadSceneAsync(gameMode.ToString());

        Instantiate(GetAsset(gameMode.ToString()), transform);

        // Continue loading on 'NotifyGameModeLoaded'
    }

    public void NotifyGameModeLoaded(GameModeBase gameMode)
    {
        Logger.Assert(mLoadedGameMode == null, LogTag.GameModes, TAG, "::NotifyGameModeLoaded() - GameMode already loaded", LogLevel.Warning);

        mLoadedGameMode = gameMode;

        GameModeEventRouter.Instance.NotifyEvent(new GameModeEvent(GameModeEvent.EventType.UISetup_Begin));
    }

    // IEventHandler<GameModeEvent>
    public void HandleEvent(GameModeEvent eventInfo)
    {
        switch (eventInfo.Type)
        {
            case GameModeEvent.EventType.UISetup_Complete:
                {
                    Logger.Log(LogTag.GameModes, TAG, "::HandleEvent() - " + eventInfo.Type.ToString());

                    GameModeEventRouter.Instance.NotifyEvent(new GameModeEvent(GameModeEvent.EventType.ModeSetup_Begin));
                }
                break;

            case GameModeEvent.EventType.ModeSetup_Complete:
                {
                    Logger.Log(LogTag.GameModes, TAG, "::HandleEvent() - " + eventInfo.Type.ToString());

                    GameModeEventRouter.Instance.NotifyEvent(new GameModeEvent(GameModeEvent.EventType.ModeStart));
                }
                break;

            case GameModeEvent.EventType.ModeTakedown_Complete:
                {
                    Logger.Log(LogTag.GameModes, TAG, "::HandleEvent() - " + eventInfo.Type.ToString());

                    Destroy(mLoadedGameMode.gameObject);

                    GameModeEventRouter.Instance.NotifyEvent(new GameModeEvent(GameModeEvent.EventType.UITakedown_Begin));
                }

                break;

            case GameModeEvent.EventType.UITakedown_Complete:
                {
                    Logger.Log(LogTag.GameModes, TAG, "::HandleEvent() - " + eventInfo.Type.ToString());

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

    // Debug
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

    protected override string GetAssetPath()
    {
        return "GameModes";
    }

    protected override void OnInitializeAssets()
    {
        LoadAssets("");
    }
}

