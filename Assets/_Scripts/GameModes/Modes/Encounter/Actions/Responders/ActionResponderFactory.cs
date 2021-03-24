using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes.Combat
{
    static class ActionResponderFactory
    {
        public static ActionResponderBase CheckoutResponder(CombatEntity owner, ActionResponseId type)
        {
            ActionResponderBase actionResponder = null;

            switch (type)
            {
                case (ActionResponseId.Riptose):
                    actionResponder = new RiptoseResponder(owner);
                    break;
                default:
                    UnityEngine.Debug.Assert(false);
                    break;
            }

            return actionResponder;
        }
    }
}



