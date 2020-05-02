using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class MainMenuModule : GameModeBase
{
    public bool AutoStart = true;

    MainMenuViewControl ViewControl;

    public override GameModeType GetGameModeType()
    {
        return GameModeType.MainMenu;
    }

    protected override void CleanUpMode()
    {
        // nothing to cleanup
        GameModeEventRouter.Instance.NotifyEvent(new GameModeEvent(GameModeEvent.EventType.ModeTakedown_Complete));
    }

    protected override void EndMode()
    {
        ViewControl.Cleanup();
    }

    protected override void SetupMode()
    {
        ViewControl = new MainMenuViewControl();
        GameModeEventRouter.Instance.NotifyEvent(new GameModeEvent(GameModeEvent.EventType.ModeSetup_Complete));
    }

    protected override void StartMode()
    {
        ViewControl.Start();

        if (AutoStart)
        {
            GameSystemModule.Instance.PrepareNewGame();
            GameModesModule.Instance.Explore();
        }
    }
}

