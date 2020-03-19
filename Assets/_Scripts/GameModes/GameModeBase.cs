using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

abstract class GameModeBase 
    : MonoBehaviour
    , IEventHandler<GameModeEvent>
{
    public abstract GameModeType GetGameModeType();

    private void Start()
    {
        GameModeEventRouter.Instance.RegisterHandler(this);
        GameModesModule.Instance.NotifyGameModeLoaded(this);
    }

    private void OnDestroy()
    {
        GameModeEventRouter.Instance.UnRegisterListener(this);
    }

    public virtual void HandleEvent(GameModeEvent eventInfo)
    {
        switch (eventInfo.Type)
        {
            case GameModeEvent.EventType.ModeSetup_Begin:
                {
                    SetupMode();
                }
                break;

            case GameModeEvent.EventType.ModeTakedown_Begin:
                {
                    
                    CleanUpMode();
                }
                break;

            case GameModeEvent.EventType.ModeStart:
                {
                    StartMode();
                }
                break;

            case GameModeEvent.EventType.ModeEnd:
                {

                    EndMode();
                }
                break;
        }
    }

    protected abstract void SetupMode();
    protected abstract void StartMode();
    protected abstract void EndMode();
    protected abstract void CleanUpMode();
}

