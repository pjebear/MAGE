using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Combat
{
    abstract class ActionResponderBase
    {
        protected CombatEntity mResponder;
        protected float Range = 10f;
        protected int PercentChance = 100;

        protected ActionResponderBase(CombatEntity responder)
        {
            mResponder = responder;
        }

        protected abstract List<ActionResponseBase> GetResponsesToResult(ActionResult actionResult);

        protected bool DiceRoll(int percentChance)
        {
            System.Random random = new System.Random();
            int diceRoll = random.Next(100);
            return diceRoll <= percentChance;
        }

        protected bool InRange(Transform other, float range)
        {
            bool inRange = true;

            float distance = (other.position - mResponder.transform.position).magnitude;
            if (range != ActionResponseInfo.INFINITE_RANGE)
            {
                inRange = distance <= range;
            }

            return inRange;
        }

        protected bool IsResponder(CombatEntity other)
        {
            return mResponder == other;
        }

        protected bool IsAlly(CombatEntity other)
        {
            return mResponder.TeamSide == other.TeamSide;
        }

        protected bool WasHurt(InteractionResult result)
        {
            return result.StateChange.healthChange < 0;
        }

        protected bool WasResponderTargeted(ActionResult result)
        {
            return result.TargetResults.Keys.Contains(mResponder.GetComponent<CombatTarget>());
        }

        protected bool IsAlive(CombatEntity entity)
        {
            bool isAlive = false;

            ResourcesControl resourcesControl = entity.GetComponent<ResourcesControl>();
            Debug.Assert(resourcesControl);
            if (resourcesControl != null)
            {
                isAlive = resourcesControl.IsAlive();
            }

            return isAlive;
        }

        public List<ActionResponseBase> RespondToActionResult(ActionResult result)
        {
            if (DiceRoll(PercentChance))
            {
                return GetResponsesToResult(result);
            }
            else
            {
                return new List<ActionResponseBase>();
            }
        }
    }
}

