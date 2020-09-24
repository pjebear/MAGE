using MAGE.GameModes.FlowControl;
using MAGE.GameSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes
{
    class PartyOutfiterModule : MonoBehaviour
    {
        //private Transform mCharacterSpawnPoint;

        //private PartyOutfiterViewControl ViewControl;

        //public override GameModeType GetGameModeType()
        //{
        //    return GameModeType.PartyOutfiter;
        //}

        //public override void Init()
        //{

        //}

        //protected override void CleanUpFlow()
        //{
        //    // nothing to cleanup
        //    Messaging.MessageRouter.Instance.NotifyMessage(new GameModeMessage(GameModeMessage.EventType.ModeTakedown_Complete));
        //}

        //protected override void EndFlow()
        //{
        //    ViewControl.Cleanup();
        //}

        //protected override void SetupFlow()
        //{
        //    ViewControl = new PartyOutfiterViewControl();

        //    mCharacterSpawnPoint = GameObject.Find("SpawnPoint").transform;
        //    ViewControl.Init(mCharacterSpawnPoint);

        //    Messaging.MessageRouter.Instance.NotifyMessage(new GameModeMessage(GameModeMessage.EventType.ModeSetup_Complete));
        //}

        //protected override void StartFlow()
        //{
        //    ViewControl.Start();
        //}
    }
}
