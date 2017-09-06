using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EncounterSystem.Character;
using EncounterSystem.Action;

namespace EncounterSystem.EventSystem
{
    class OnDamageTakenHook : EventHook
    {
        void OnDamageTaken(OnDamagedPayload payload)
        {
            UnityEngine.Debug.Log("OnDamageTaken Hook recieved event call");
            mEventHandler.HandleEvent(EventType.OnDamageTaken, payload);
        }
    }
}
