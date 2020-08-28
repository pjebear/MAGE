using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Actions
{
    abstract class ActionResponderBase
    {
        protected ActionResponseId mResponseId;

        protected ActionResponderBase(ActionResponseId responseId)
        {
            mResponseId = responseId;
        }

        protected abstract List<ActionResponseBase> GetResponsesToResult(Character responder, ActionResponseInfo responseInfo, ActionResult actionResult, Map map);

        protected bool DiceRoll(int percentChance)
        {
            System.Random random = new System.Random();
            int diceRoll = random.Next(100);
            return diceRoll <= percentChance;
        }

        protected bool InRange(Character responder, Character other, int range, Map map)
        {
            bool inRange = true;

            float distance = map.DistanceBetween(responder, other);
            if (range != ActionResponseInfo.INFINITE_RANGE)
            {
                inRange = distance <= range;
            }

            return inRange;
        }

        protected bool IsResponder(Character responder, Character other)
        {
            return responder == other;
        }

        protected bool IsAlly(Character responder, Character other)
        {
            return responder.TeamSide == other.TeamSide;
        }

        protected bool WasHurt(InteractionResult result)
        {
            return result.StateChange.healthChange < 0;
        }

        protected bool WasCharacterATarget(Character character, ActionResult result)
        {
            return result.TargetResults.Keys.Contains(character);
        }

        protected bool IsAlive(Character character)
        {
            return character.IsAlive;
        }

        public List<ActionResponseBase> RespondToActionResult(Character responder, ActionResult result, Map map)
        {
            // Get the most up to date version of the info
            ActionResponseInfo responseInfo = responder.GetActionResponseInfo(mResponseId);

            if (DiceRoll(responseInfo.PercentChance))
            {
                return GetResponsesToResult(responder, responseInfo, result, map);
            }
            else
            {
                return new List<ActionResponseBase>();
            }
        }
    }
}

