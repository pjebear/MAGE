using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Characters
{
    class CharacterMessage : Messaging.MessageInfoBase
    {
        public enum MessageType
        {
            CharacterUpdated,

            NUM
        }

        public const int Id = Messaging.MessagingConstants.MESSAGING_ID_OFFSET * (int)Messaging.MessageType.Character;

        public MessageType Type;
        public CharacterMessage(MessageType messageType, object arg = null)
            : base(Id, arg)
        {
            Type = messageType;
        }
    }
}
