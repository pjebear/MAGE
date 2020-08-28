using MAGE.GameSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes
{
    abstract class GameModeBase
        : MonoBehaviour
        , Messaging.IMessageHandler
    {
        public abstract GameModeType GetGameModeType();

        private void Start()
        {
            Messaging.MessageRouter.Instance.RegisterHandler(this);
            GameModesModule.Instance.NotifyGameModeLoaded(this);
        }

        private void OnDestroy()
        {
            Messaging.MessageRouter.Instance.UnRegisterHandler(this);
        }

        public virtual void HandleMessage(Messaging.MessageInfoBase messageInfo)
        {
            switch (messageInfo.MessageId)
            {
                case GameModeMessage.Id:
                {
                    GameModeMessage gameModeMessage = messageInfo as GameModeMessage;

                    switch (gameModeMessage.Type)
                    {
                        case GameModeMessage.EventType.ModeSetup_Begin:
                        {
                            SetupMode();
                        }
                        break;

                        case GameModeMessage.EventType.ModeTakedown_Begin:
                        {
                            CleanUpMode();
                        }
                        break;

                        case GameModeMessage.EventType.ModeStart:
                        {
                            StartMode();
                        }
                        break;

                        case GameModeMessage.EventType.ModeEnd:
                        {
                            EndMode();
                        }
                        break;
                    }
                }
                break;
            }
        }

        public abstract void Init();
        public abstract LevelId GetLevelId();
        protected abstract void SetupMode();
        protected abstract void StartMode();
        protected abstract void EndMode();
        protected abstract void CleanUpMode();
    }
}


