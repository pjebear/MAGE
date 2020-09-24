using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes.FlowControl
{
    class FlowMessage : Messaging.MessageInfoBase
    {
        public const int Id = Messaging.MessagingConstants.MESSAGING_ID_OFFSET * (int)Messaging.MessageType.Flow;

        public enum EventType
        {
            FlowEvent,
            Transition,

            Notify, 
            Query, 

            LoadFlowControl,
            UnLoadFlowControl,

            NUM
        }

        public EventType Type;

        public FlowMessage(EventType type, object arg = null)
            : base(Id, arg)
        {
            Type = type;
        }
    }
}
