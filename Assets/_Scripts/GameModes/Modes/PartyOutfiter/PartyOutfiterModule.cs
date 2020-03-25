using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class PartyOutfiterModule : GameModeBase
{
    private Transform mCharacterSpawnPoint;

    private PartyOutfiterViewControl ViewControl;

    public override GameModeType GetGameModeType()
    {
        return GameModeType.PartyOutfiter;
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
        ViewControl = new PartyOutfiterViewControl();
       
        mCharacterSpawnPoint = GameObject.Find("SpawnPoint").transform;
        ViewControl.Init(mCharacterSpawnPoint);

        GameModeEventRouter.Instance.NotifyEvent(new GameModeEvent(GameModeEvent.EventType.ModeSetup_Complete));
    }

    protected override void StartMode()
    {
        ViewControl.Start();
    }
}

