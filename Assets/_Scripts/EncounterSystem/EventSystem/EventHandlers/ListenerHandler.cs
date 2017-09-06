using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using EncounterSystem.Character;
namespace EncounterSystem.EventSystem
{
    using Common;
    abstract class ListenerHandler : MonoBehaviour
    {
        protected static int sListenerTagCounter = 1000;
        protected static readonly int sListenerTagIncrement = 1000;
        protected CharacterManager mListener = null;
        protected int mListenerTag;
        protected SphereCollider mListenerCollider;
        protected List<EventType> mListenedForEvents;
        protected float mListenerRange;
        protected bool mIsDirty;
        protected bool mIsSelfListeningOnly = false;

        protected abstract void Awake();

        protected virtual void Start()
        {
            mListenerCollider = gameObject.AddComponent<SphereCollider>();
            mListenerCollider.radius = mListenerRange;
            mListenerCollider.isTrigger = true;

            Debug.Assert(gameObject.transform.parent != null, "Error Setting up Listener. Not Assigned to a character");
            mListener = gameObject.transform.parent.GetComponent<CharacterManager>();
        }

        protected void OnTriggerEnter(Collider other)
        {
            if (mIsSelfListeningOnly)
            {
                return;
            }
            if (other.GetComponent<CharacterManager>())
            {
                EventHook[] eventHooks = other.GetComponents<EventHook>();

                //Add Hook if not already on character
                foreach (EventType type in mListenedForEvents)
                {
                    bool alreadyListening = false;
                    foreach (EventHook hook in eventHooks)
                    {
                        if (hook.HookTag == mListenerTag + (int)type)
                        {
                            alreadyListening = true;
                        }
                    }
                    if (!alreadyListening)
                    {
                        ApplyEventHook(type, other.gameObject);
                    }
                }
            }
        }

        protected void OnTriggerExit(Collider other)
        {
            if (mIsSelfListeningOnly)
            {
                return;
            }
            if (other.GetComponent<CharacterManager>())
            {
                var eventHooks = other.GetComponents<EventHook>()
                    .Where(hook => (hook.HookTag - mListenerTag < (int)EventType.NUM) && (hook.HookTag - mListenerTag >= 0)).ToList();
                
                if (eventHooks.Count == 0)
                {
                    Debug.Log("No event hooks on character leaving listener range");
                }
                for (int i = 0; i < eventHooks.Count; i++)
                {
                    Destroy(eventHooks[i]);
                }
            }
        }

        protected void ApplyEventHook(EventType type, GameObject toApplyTo)
        {
            switch(type)
            {
                case (EventType.OnTargeted):
                    toApplyTo.AddComponent<OnTargetedHook>().InitializeHook(this, type, mListenerTag + (int)type);
                    break;
                case (EventType.OnTargetedComplete):
                    toApplyTo.AddComponent<OnTargetedHook>().InitializeHook(this, type, mListenerTag + (int)type);
                    break;
                case (EventType.OnAct):
                    toApplyTo.AddComponent<OnActHook>().InitializeHook(this, type, mListenerTag + (int)type);
                    break;
                case (EventType.OnActComplete):
                    toApplyTo.AddComponent<OnActHook>().InitializeHook(this, type, mListenerTag + (int)type);
                    break;
                case (EventType.OnDamageDealt):
                    toApplyTo.AddComponent<OnDamageDealtHook>().InitializeHook(this, type, mListenerTag + (int)type);
                    break;
                case (EventType.OnDamageTaken):
                    toApplyTo.AddComponent<OnDamageTakenHook>().InitializeHook(this, type, mListenerTag + (int)type);
                    break;
                default:
                    Debug.LogError("No Implementation for " + type.ToString() + " added to ApplyEventHook yet");
                    break;
            }
        }

        public abstract void HandleEvent(EventType type, object payload);
        
    }
}
