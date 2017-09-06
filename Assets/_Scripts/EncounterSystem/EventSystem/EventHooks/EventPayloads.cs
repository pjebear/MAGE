using EncounterSystem.Action;
using EncounterSystem.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EncounterSystem.EventSystem
{
    enum EventType
    {
        OnAct,
        OnActComplete,
        OnTargeted,
        OnTargetedComplete,
        OnDamageTaken,
        OnDamageDealt,
        NUM
    }

    class OnActPayload
    {
        public CharacterManager Actor;
        public ActionBase Action;

        public OnActPayload(ActionBase action)
        {
            Action = action;
        }
    }

    class OnDamagedPayload
    {
        public CharacterManager DamageDealer;
        public CharacterManager DamageTaker;

        public OnDamagedPayload(CharacterManager dealer, CharacterManager taker)
        {
            DamageDealer = dealer;
            DamageTaker = taker;
        }
    }

    class OnTargetedPayload
    {
        public CharacterManager Actor;
        public ActionBase Action;
        public CharacterManager Targeted;
        public OnTargetedPayload(CharacterManager actor, ActionBase action)
        {
            Actor = actor;
            Action = action;
        }
    }
}
