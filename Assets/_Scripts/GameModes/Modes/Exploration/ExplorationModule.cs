using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ExplorationModule : GameModeBase
{
    ExplorationMenuViewControl MenuControl;

    protected override void SetupMode()
    {
        MenuControl = new ExplorationMenuViewControl();

        GameModeEventRouter.Instance.NotifyEvent(new GameModeEvent(GameModeEvent.EventType.ModeSetup_Complete));
    }

    protected override void CleanUpMode()
    {
        GameModeEventRouter.Instance.NotifyEvent(new GameModeEvent(GameModeEvent.EventType.ModeTakedown_Complete));
    }

    protected override void StartMode()
    {
        MenuControl.Show();
    }

    protected override void EndMode()
    {
        MenuControl.Hide();
    }

    public override GameModeType GetGameModeType()
    {
        return GameModeType.Exploration;
    }
}

