using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EncounterSystem.Character;
using EncounterSystem.Action;

namespace EncounterSystem.EventSystem
{
    

    class OnActHook : EventHook
    {
        void OnAct(OnActPayload payload)
        {
            UnityEngine.Debug.Log("OnAct Hook recieved event call");
            payload.Actor = gameObject.GetComponent<CharacterManager>();
            mEventHandler.HandleEvent(EventType.OnAct, payload);
        }

        void OnActComplete(OnActPayload payload)
        {
            UnityEngine.Debug.Log("OnAct Hook recieved event call");
            payload.Actor = gameObject.GetComponent<CharacterManager>();
            mEventHandler.HandleEvent(EventType.OnActComplete, payload);
        }
    }
}
