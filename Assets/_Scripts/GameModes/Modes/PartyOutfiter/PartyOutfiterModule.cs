using MAGE.GameModes.FlowControl;
using MAGE.GameServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes
{
    class PartyOutfiterModule : GameModeBase
    {
        private Transform mCharacterSpawnPoint;

        private PartyOutfiterViewControl ViewControl;

        public override GameModeType GetGameModeType()
        {
            return GameModeType.PartyOutfiter;
        }

        public override LevelId GetLevelId()
        {
            return MAGE.GameModes.LevelManagementService.Get().GetLoadedLevel().LevelId;
        }

        public override void Init()
        {

        }

        protected override void CleanUpMode()
        {
            // nothing to cleanup
            Messaging.MessageRouter.Instance.NotifyMessage(new GameModeMessage(GameModeMessage.EventType.ModeTakedown_Complete));
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

            Messaging.MessageRouter.Instance.NotifyMessage(new GameModeMessage(GameModeMessage.EventType.ModeSetup_Complete));
        }

        protected override void StartMode()
        {
            ViewControl.Start();
        }
    }
}
