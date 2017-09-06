using System.Collections.Generic;
using Common.AttributeEnums;
using Common.ActionEnums;
using Common.ActionTypes;
using Common.StatusEnums;
using UnityEngine;
namespace EncounterSystem
{
    using StatusEffects;
    namespace Action
    {
        static class ActionUtil
        {
            public static ActionOrientation GetActionOrienation(Vector3 casterPosition, Vector3 targetPosition, Vector3 targetForward)
            {
                Vector3 displacement = targetPosition - casterPosition;
                displacement.y = 0f;
                displacement.Normalize();
                targetForward.y = 0f;
                targetForward.Normalize();

                float angle = 180 * Mathf.Asin(Vector3.Dot(targetForward, displacement)) / Mathf.PI;
                if (angle < 45)
                {
                    return ActionOrientation.Frontal;
                }
                else if (angle < 90 && angle > 45)
                {
                    return ActionOrientation.Peripheral;
                }
                else
                {
                    return ActionOrientation.Rear;
                }

            }
        }

        public struct AnimationInfo
        {
            public string ChargeAnimationId;
            public string PreActionAnimationId;
            public string ActionAnimationId;
            public AnimationInfo(string chargeAnimationId, string preActionAnimationId, string actionAnimationId)
            {
                ChargeAnimationId = chargeAnimationId;
                PreActionAnimationId = preActionAnimationId;
                ActionAnimationId = actionAnimationId;
            }
        }

        public struct ActionInfo
        {
            public ActionIndex ActionIndex;
            public string Name;

            // Caster relevant information
            public int ChargeTime;
            public List<ActionModifier> Modifiers;
            public ResourceChange ActionCost;
            public List<Prerequisite> Prerequisites;
            public ActionRootEffect RootEffect;

            // Applied to targets of action
            public List<StatusEffectIndex> StatusEffects;
            public ActionResourceChangeInformation BaseResourceChangeInfo;


            public ActionInfo(ActionIndex factoryIndex, string name,
                int chargeTime, List<ActionModifier> modifiers, ResourceChange actionCost, List<Prerequisite> prerequisites,
                ActionResourceChangeInformation baseResourceInformation, List<StatusEffectIndex> statusEffects, ActionRootEffect rootEffect)
            {
                ActionIndex = factoryIndex;
                Name = name;

                ChargeTime = chargeTime;
                Modifiers = new List<ActionModifier>();
                Modifiers.AddRange(modifiers);
                ActionCost = actionCost;
                Prerequisites = new List<Prerequisite>();
                Prerequisites.AddRange(prerequisites);

                BaseResourceChangeInfo = baseResourceInformation;
                StatusEffects = new List<StatusEffectIndex>();
                StatusEffects.AddRange(statusEffects);

                RootEffect = rootEffect;
            }
        }
    }
}
