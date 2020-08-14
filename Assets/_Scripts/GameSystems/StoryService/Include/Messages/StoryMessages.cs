using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameServices.Story
{
    enum MessageType
    {
        StoryArcUpdated,

        NUM
    }

    class StoryMessage : Messaging.MessageInfoBase
    {
        public const int Id = Messaging.MessagingConstants.MESSAGING_ID_OFFSET * (int)Messaging.MessageType.Story;

        public MessageType Type;
        public StoryMessage(MessageType messageType, object arg)
            : base(Id, arg)
        {
            Type = messageType;
        }
    }
}
