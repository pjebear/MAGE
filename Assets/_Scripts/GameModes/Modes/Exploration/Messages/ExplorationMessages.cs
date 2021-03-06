﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes.Exploration
{
    class ExplorationMessage : Messaging.MessageInfoBase
    {
        public const int Id = Messaging.MessagingConstants.MESSAGING_ID_OFFSET + (int)Messaging.MessageType.Exploration;
        public enum EventType
        {
            CinematicMomentTriggered,
            EncounterTriggered,
            InteractionStart,
            InteractionEnd,
            ScenarioTriggered,
            TravelTriggered,

            NUM
        }

        public EventType Type;
        public ExplorationMessage(EventType type, object arg = null)
            : base(Id, arg)
        {
            Type = type;
        }
    }
}
