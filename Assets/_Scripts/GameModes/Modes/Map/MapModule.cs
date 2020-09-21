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
    class MapModule : GameModeBase
    {
        public bool AutoStart = true;

        MapViewControl ViewControl;

        public override GameModeType GetGameModeType()
        {
            return GameModeType.Map;
        }

        public override LevelId GetLevelId()
        {
            return LevelId.INVALID;
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
            ViewControl = new MapViewControl();
            Messaging.MessageRouter.Instance.NotifyMessage(new GameModeMessage(GameModeMessage.EventType.ModeSetup_Complete));
        }

        protected override void StartMode()
        {
            ViewControl.Start();
        }
    }
}

