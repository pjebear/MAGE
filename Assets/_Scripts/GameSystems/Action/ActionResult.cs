using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Actions
{
    class ActionResult
    {
        public Character Initiator;
        public ActionId ActionId;
        public ActionInfo ActionInfo;
        public InteractionResult InitiatorResult;
        public Dictionary<Character, InteractionResult> TargetResults;

        public ActionResult(Character initiator, ActionInfo actionInfo, InteractionResult initiatorResult, 
            Dictionary<Character, InteractionResult> targetResults)
        {
            Initiator = initiator;
            ActionId = actionInfo.ActionId;
            ActionInfo = actionInfo;
            InitiatorResult = initiatorResult;
            TargetResults = targetResults;
        }
    }
}

