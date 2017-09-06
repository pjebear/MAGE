using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EncounterSystem.Character;
using EncounterSystem.Action;

namespace EncounterSystem.EventSystem
{
    

    class OnTargetedHook : EventHook
    {
        void OnTargeted(OnTargetedPayload payload)
        {
            payload.Targeted = gameObject.GetComponent<CharacterManager>();
            mEventHandler.HandleEvent(EventType.OnTargeted, payload);
        }

        void OnTargetedComplete(OnTargetedPayload payload)
        {
            payload.Targeted = gameObject.GetComponent<CharacterManager>();
            mEventHandler.HandleEvent(EventType.OnTargetedComplete, payload);
        }
    }
}
