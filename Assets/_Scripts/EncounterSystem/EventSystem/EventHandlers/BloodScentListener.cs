using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EncounterSystem.Action;
using EncounterSystem.Character;
using Common.ActionEnums;
using Common.StatusEnums;
using Common.StatusTypes;
using UnityEngine;

namespace EncounterSystem.EventSystem
{
    class BloodScentListener : ListenerHandler
    {
        private StatusEffect bloodScent; 
        protected override void Awake()
        {
            mListenedForEvents = new List<EventType>()
            {
                EventType.OnDamageTaken,
                EventType.OnDamageDealt
            };
            mListenerRange = .25f; // self

            bloodScent = StatusEffects.StatusEffectFactory.CheckoutStatusEffect(StatusEffectIndex.BERSERKER_BLOODSCENT);
            mIsSelfListeningOnly = true;
        }

        protected override void Start()
        {
            base.Start();
            foreach (EventType type in mListenedForEvents)
            {
                ApplyEventHook(type, mListener.gameObject);
            }
        }

        public override void HandleEvent(EventType type, object payload)
        {
            UnityEngine.Debug.Log("BloodScent recieved event call for " + type.ToString());
            switch (type)
            {
                case (EventType.OnDamageTaken):
                    {
                        OnDamagedPayload damagedPayload = payload as OnDamagedPayload;
                        UnityEngine.Debug.Assert(damagedPayload.DamageTaker == mListener);

                        mListener.AttemptStatusApplication(bloodScent, 10);

                        break;
                    }
                case (EventType.OnDamageDealt):
                    {
                        OnDamagedPayload damagedPayload = payload as OnDamagedPayload;
                        UnityEngine.Debug.Assert(damagedPayload.DamageDealer == mListener);

                        mListener.AttemptStatusApplication(bloodScent, 10);
                        break;
                    }
            }
        }
    }
}
