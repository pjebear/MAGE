using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ExplorationModule : GameModeBase, IInputHandler
{
    protected override void SetupMode()
    {
        InputManager.Instance.RegisterHandler(this, false);
        GameModeEventRouter.Instance.NotifyEvent(new GameModeEvent(GameModeEvent.EventType.ModeSetup_Complete));
    }

    protected override void CleanUpMode()
    {
        InputManager.Instance.ReleaseHandler(this);
        GameModeEventRouter.Instance.NotifyEvent(new GameModeEvent(GameModeEvent.EventType.ModeTakedown_Complete));
    }

    protected override void StartMode()
    {
        
    }

    protected override void EndMode()
    {
        
    }

    public override GameModeType GetGameModeType()
    {
        return GameModeType.Exploration;
    }

    public void OnMouseHoverChange(GameObject mouseHover)
    {
        // empty
    }

    public void OnKeyPressed(InputSource source, int key, InputState state)
    {
        if (source == InputSource.Keyboard && key == (int)KeyCode.O && state == InputState.Down)
        {
            GameModesModule.Instance.Outfit();
        }
    }
}

