using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EncounterSystem.EventSystem
{
    

    abstract class EventHook : MonoBehaviour
    {
        public int HookTag { get; private set; }
        protected ListenerHandler mEventHandler;
        protected EventType mEventType;

        public void InitializeHook(ListenerHandler handler, EventType type, int hookTag)
        {
            mEventHandler = handler;
            mEventType = type;
            HookTag = hookTag;
        }
    }
}
