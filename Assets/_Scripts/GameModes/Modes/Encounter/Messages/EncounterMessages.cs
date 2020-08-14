using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes.Encounter
{
    class EncounterMessage : Messaging.MessageInfoBase
    {
        public const int Id = Messaging.MessagingConstants.MESSAGING_ID_OFFSET * (int)Messaging.MessageType.Encounter;

        public enum EventType
        {
            EncounterBegun,
            IntroComplete,
            UnitPlacementComplete,
            ClockProgressed,
            TurnBegun,
            ActionResolved,
            MoveResolved,
            TurnFinished,
            EncounterOver,
            CharacterKO,

            NUM
        }

        public EventType Type;
        public EncounterMessage(EventType type, object arg = null)
            : base (Id, arg)
        {
            Type = type;
        }
    }
}
