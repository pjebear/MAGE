using MAGE.GameSystems;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Actions
{
    static class ActionResponderFactory
    {
        public static ActionResponderBase CheckoutResponder(ActionResponseId type)
        {
            ActionResponderBase actionResponder = null;

            switch (type)
            {
                case (ActionResponseId.Riptose):
                    actionResponder = new RiptoseResponder();
                    break;

                case (ActionResponseId.HealOnHurtListener):
                    actionResponder = new HealOnHurtResponder();
                    break;

                case (ActionResponseId.BloodScent):
                    actionResponder = new BloodScentResponder();
                    break;
                case (ActionResponseId.Avenger):
                    actionResponder = new AvengerResponder();
                    break;

                default:
                    UnityEngine.Debug.Assert(false);
                    break;
            }

            return actionResponder;
        }
    }
}



