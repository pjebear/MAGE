using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

static class ActionResultListenerFactory
{
    public static ActionResponderBase CheckoutListener(EncounterCharacter listenerOwner, ActionResponseId type)
    {
        ActionResponderBase listener = null;

        switch (type)
        {
            case (ActionResponseId.Riptose):
                listener = new RiptoseListener(listenerOwner);    
            break;

            case (ActionResponseId.HealOnHurtListener):
                listener = new HealOnHurtListener(listenerOwner);
                break;

            case (ActionResponseId.BloodScent):
                listener = new BloodScentListener(listenerOwner);
                break;

            default:
                Debug.Assert(false);
                break;
        }

        return listener;
    }
}

