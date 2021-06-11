using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MAGE.GameSystems.Actions;
using MAGE.GameModes.Encounter;
using MAGE.GameSystems.Stats;

namespace MAGE.GameModes.Combat
{
    [RequireComponent(typeof(ResourcesControl))]
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

        //  ------------------------------------------------------------------------------
        public bool HasResourcesForAction(StateChange actionCost)
        {
            bool hasResources = true;

            hasResources &= (GetComponent<ResourcesControl>().Resources[ResourceType.Health].Current + actionCost.healthChange) > 0;
            hasResources &= (GetComponent<ResourcesControl>().Resources[ResourceType.Mana].Current + actionCost.resourceChange) >= 0;

            foreach (StatusEffect statusEffect in actionCost.statusEffects)
            {
                int countRequirement = statusEffect.StackCount;
                int hasCount = GetComponent<StatusEffectControl>().GetStackCountForStatus(statusEffect.EffectType, GetComponent<ControllableEntity>().Id);

                hasResources &= countRequirement >= hasCount;
            }

            return hasResources;
        }

        //  ------------------------------------------------------------------------------
        public int GetNumAvailableActions()
        {
            return GetComponent<ResourcesControl>().Resources[ResourceType.Actions].Current;
        }

        //  ------------------------------------------------------------------------------
        public void ActionChosen()
        {
            Debug.Assert(GetNumAvailableActions() > 0);

            GetComponent<ResourcesControl>().Resources[ResourceType.Actions].Modify(-1);
        }

        //  ------------------------------------------------------------------------------
        public void OnActionPerformed(StateChange actionCost)
        {
            GetComponent<ResourcesControl>().ApplyStateChange(actionCost);
        }

        //  ------------------------------------------------------------------------------
        public int GetAvailableMovementRange()
        {
            int range = 0;

            if (GetComponent<StatsControl>().Attributes[StatusType.Rooted] == 0)
            {
                range = GetComponent<ResourcesControl>().Resources[ResourceType.MovementRange].Current;
            }            

            return range;
        }

        //  ------------------------------------------------------------------------------
        public void OnMovementPerformed(int movementRange)
        {
            if (GetAvailableMovementRange() < movementRange)
            {
                Debug.LogWarningFormat("CombatCharacter::OnMovementPerformed() - Max Range {0} less than movementPerformed {1}", GetAvailableMovementRange(), movementRange);
                movementRange = GetAvailableMovementRange();
            }

            GetComponent<ResourcesControl>().Resources[ResourceType.MovementRange].Modify(-movementRange);
        }
    }
}
