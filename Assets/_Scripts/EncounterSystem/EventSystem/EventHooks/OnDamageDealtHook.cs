using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EncounterSystem.Character;
using EncounterSystem.Action;

namespace EncounterSystem.EventSystem
{
    class OnDamageDealtHook : EventHook
    {
        void OnDamageDealt(OnDamagedPayload payload)
        {
            UnityEngine.Debug.Log("OnDamageDealt Hook recieved event call");
            mEventHandler.HandleEvent(EventType.OnDamageDealt, payload);
        }
    }
}
