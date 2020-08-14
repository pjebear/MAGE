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
    class MainMenuModule : GameModeBase
    {
        public bool AutoStart = true;

        MainMenuViewControl ViewControl;

        public override GameModeType GetGameModeType()
        {
            return GameModeType.MainMenu;
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
            ViewControl = new MainMenuViewControl();
            Messaging.MessageRouter.Instance.NotifyMessage(new GameModeMessage(GameModeMessage.EventType.ModeSetup_Complete));
        }

        protected override void StartMode()
        {
            ViewControl.Start();

            if (AutoStart)
            {
                MAGE.GameServices.WorldService.Get().PrepareNewGame();
                GameModesModule.Instance.Explore();
            }
        }
    }


}

