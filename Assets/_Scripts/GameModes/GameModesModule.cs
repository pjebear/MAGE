using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class GameModesModule 
    : MonoBehaviour
    , IEventHandler<GameModeEvent>
{
    private string TAG = "GameModesModule";

    public static GameModesModule Instance;

    public static ActorLoader ActorLoader;
    public static AudioManager AudioManager;
    public static LevelManager LevelManager;

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
        LevelManager = GetComponent<LevelManager>();

        GameModeEventRouter.Instance = GetComponent<GameModeEventRouter>();
        GameModeEventRouter.Instance.RegisterHandler(this);

        mGameModeLoader = new AssetLoader<GameModeBase>("GameModes");
        mGameModeLoader.LoadAssets();
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
        Instantiate(mGameModeLoader.GetAsset(gameMode.ToString()), transform);

        // Continue loading on 'NotifyGameModeLoaded'
    }

    public void NotifyGameModeLoaded(GameModeBase gameMode)
    {
        Logger.Assert(mLoadedGameMode == null, LogTag.GameModes, TAG, "::NotifyGameModeLoaded() - GameMode already loaded", LogLevel.Warning);

        mLoadedGameMode = gameMode;
        mLoadedGameMode.Init();
        LevelId levelId = mLoadedGameMode.GetLevelId();
        if (levelId != LevelId.INVALID)
        {
            Level loadedLevel = LevelManager.GetLoadedLevel();
            if (loadedLevel == null || loadedLevel.LevelId != levelId)
            {
                LevelManager.LoadLevel(levelId);
            }
        }
        else
        {
            LevelManager.UnloadLevel();
        }

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

