using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes
{
    class GameModeMessage : Messaging.MessageInfoBase
    {
        public const int Id = Messaging.MessagingConstants.MESSAGING_ID_OFFSET * (int)Messaging.MessageType.GameModes;

        public enum EventType
        {
            UISetup_Begin,
            UISetup_Complete,
            ModeSetup_Begin,
            ModeSetup_Complete,

            ModeStart,
            ModeEnd,

            ModeTakedown_Begin,
            ModeTakedown_Complete,
            UITakedown_Begin,
            UITakedown_Complete,

            FadeOut,
            FadeIn,

            SetupBegin = UISetup_Begin,
            SetupEnd = ModeSetup_Complete,

            NUM
        }

        public EventType Type;

        public GameModeMessage(EventType type, object arg = null)
            : base(Id, arg)
        {
            Type = type;
        }
    }
}

