using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes.Combat
{
    class ActionResult
    {
        public CombatEntity Initiator;
        public ActionId ActionId;
        public ActionInfo ActionInfo;
        public InteractionResult InitiatorResult;
        public Dictionary<CombatTarget, InteractionResult> TargetResults;

        public ActionResult(CombatEntity initiator, ActionInfo actionInfo, InteractionResult initiatorResult, 
            Dictionary<CombatTarget, InteractionResult> targetResults)
        {
            Initiator = initiator;
            ActionId = actionInfo.ActionId;
            ActionInfo = actionInfo;
            InitiatorResult = initiatorResult;
            TargetResults = targetResults;
        }
    }
}

