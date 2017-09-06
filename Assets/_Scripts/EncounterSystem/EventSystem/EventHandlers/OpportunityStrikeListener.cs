using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Common.ActionEnums;
using Common.StatusEnums;
using EncounterSystem.Map;

namespace EncounterSystem.EventSystem
{
    using Action;
    using Character;
    using MapTypes;

    class OpportunityStrikeListener : ListenerHandler
    {
        ActionBase mOpportunityStrikeAction;
        CharacterManager mStruckUnit = null;

        protected override void Awake()
        {
            mListenedForEvents = new List<EventType>()
            {
                EventType.OnAct
            };
            mListenerRange = 2f;
            mOpportunityStrikeAction = ActionFactory.Instance.CheckoutAction(ActionIndex.OPPORTUNITY_STRIKE);
        }

        public override void HandleEvent(EventType type, object payload)
        {
            UnityEngine.Debug.Log("OpportunityStrikeListener recieved event call for " + type.ToString());
            switch (type)
            {
                case (EventType.OnAct):
                    {
                        OnActPayload onActPayload = payload as OnActPayload;
                        if (onActPayload.Actor.IsPlayerControlled ^ mListener.IsPlayerControlled) // enemy character
                        {
                            mStruckUnit = onActPayload.Actor;
                            UnityEngine.Debug.Log("OpportunityStrikeListener Executing action " + mOpportunityStrikeAction.ActionInfo.ActionIndex.ToString());
                            StartCoroutine(mOpportunityStrikeAction.ExececuteAction(mListener, new List<MapTile>() { onActPayload.Actor.GetCurrentTile() }, (CharacterManager) => { }));
                        }
                        break;
                    }
                case (EventType.OnActComplete):
                    {
                        OnActPayload onActPayload = payload as OnActPayload;
                        if (mStruckUnit != null && onActPayload.Actor == mStruckUnit)
                        {
                            foreach (StatusEffectIndex index in mOpportunityStrikeAction.ActionInfo.StatusEffects)
                            {
                                mStruckUnit.AttemptRemoveStatusEffect(index);
                            }
                            mStruckUnit = null;
                        }
                        break;
                    }
            }
        }
    }
}
