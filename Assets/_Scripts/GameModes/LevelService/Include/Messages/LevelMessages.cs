using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes.LevelManagement
{
    enum MessageType
    {
        AppearanceUpdated,
        PropUpdated,

        NUM
    }

    class LevelMessage : Messaging.MessageInfoBase
    {
        public const int Id = Messaging.MessagingConstants.MESSAGING_ID_OFFSET * (int)Messaging.MessageType.Level;

        public MessageType Type;
        public LevelMessage(MessageType messageType, object arg = null)
            : base(Id, arg)
        {
            Type = messageType;
        }
    }
}
