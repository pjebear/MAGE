using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MAGE.GameSystems.Actions;
using MAGE.GameModes.Encounter;

namespace MAGE.GameModes.Combat
{
    class ActionsControl : MonoBehaviour
    {
        public List<ActionId> Actions = new List<ActionId>();
        public List<ActionResponderBase> ActionResponders = new List<ActionResponderBase>();

        //  ------------------------------------------------------------------------------
        public List<ActionResponseBase> RespondToAction(ActionResult actionResult)
        {
            List<ActionResponseBase> responses = new List<ActionResponseBase>();

            foreach (ActionResponderBase responder in ActionResponders)
            {
                responses.AddRange(responder.RespondToActionResult(actionResult));
            }


            return responses;
        }
    }
}
