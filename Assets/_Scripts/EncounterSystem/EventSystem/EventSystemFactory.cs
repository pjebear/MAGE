using EncounterSystem.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EncounterSystem.EventSystem
{
    public enum EventListenerIndex
    {
        BloodScent,
        Intervene,
        OpportunityStrike
    }

    class EventSystemFactory
    {
        public static void CheckoutListenerHandler(EventListenerIndex index, Transform owner)
        {
            GameObject listenerObj = new GameObject(index.ToString() + "_" + owner.gameObject.name);
            switch (index)
            {
                case (EventListenerIndex.BloodScent):
                    listenerObj.AddComponent<BloodScentListener>();
                    break;
                case (EventListenerIndex.Intervene):
                    listenerObj.AddComponent<InterveneListener>();
                    break;
                case (EventListenerIndex.OpportunityStrike):
                    listenerObj.AddComponent<OpportunityStrikeListener>();
                    break;
                default:
                    Debug.LogErrorFormat("No listener bound to {0}", index.ToString());
                    break;
            }
            listenerObj.transform.SetParent(owner);
        }
    }
}
