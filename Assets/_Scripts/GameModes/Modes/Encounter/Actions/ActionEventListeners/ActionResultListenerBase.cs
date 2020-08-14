using MAGE.GameModes.SceneElements;
using MAGE.GameServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes.Encounter
{
    abstract class ActionResponderBase
    {
        protected EncounterCharacter Listener;
        protected ActionResponseId mResponseId;
        protected ActionResponseInfo mResponseInfo;

        protected ActionResponderBase(EncounterCharacter listener, ActionResponseId responseId)
        {
            Listener = listener;
            mResponseId = responseId;
            mResponseInfo = null;
        }

        protected abstract void OnActionResult(ActionResult actionResult);

        protected bool DiceRoll()
        {
            System.Random random = new System.Random();
            int diceRoll = random.Next(100);
            return diceRoll <= mResponseInfo.PercentChance;
        }

        protected bool InRange(EncounterCharacter target)
        {
            bool inRange = true;

            if (mResponseInfo.Range != ActionResponseInfo.INFINITE_RANGE)
            {
                Tile listenersTile = EncounterModule.Map.GetActorsTile(Listener);
                Tile targetsTile = EncounterModule.Map.GetActorsTile(target);

                inRange = listenersTile.DistanceTo(targetsTile) <= mResponseInfo.Range;
            }

            return inRange;
        }

        protected bool IsListener(EncounterCharacter actor)
        {
            return Listener == actor;
        }

        protected bool IsAlly(EncounterCharacter target)
        {
            return Listener.Team == target.Team;
        }

        protected bool WasHurt(InteractionResult result)
        {
            return result.StateChange.healthChange < 0;
        }

        protected bool WasTargeted(EncounterCharacter actor, ActionResult result)
        {
            return result.TargetResults.Keys.Contains(actor);
        }

        protected bool IsAlive(EncounterCharacter target)
        {
            return target.IsAlive;
        }

        public void RespondToAction(ActionResult result)
        {
            // Get the most up to date version of the info
            mResponseInfo = Listener.GetActionResponseInfo(mResponseId);

            if (DiceRoll())
            {
                OnActionResult(result);
            }
        }
    }
}

