using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.AI
{
    enum ActionValuePolicyType
    {
        HealthChange,
        HighHealthPercent,
        LowHealthPercent,
        AvoidFrontalOrientationPolicy,

        NUM
    }

    static class HealthChangePolicy
    {
        public static float PercentChangeToValue = 1;

        public static float CalculateActionValue(Character performingAction, CharacterPosition positioning, ActionInfo actionInfo, List<Character> actionTargets, Map map)
        {
            float value = 0;

            foreach (Character target in actionTargets)
            {
                if (target.IsAlive)
                {
                    Resource health = target.CurrentResources[ResourceType.Health];
                    StateChange stateChange = actionInfo.GetTargetStateChange(performingAction, target);

                    float newHealth = health.Current + stateChange.healthChange;
                    newHealth = Math.Max(0, newHealth);
                    newHealth = Math.Min(health.Max, newHealth);
                    float percentHealthChange = (newHealth - health.Current) / health.Max;

                    if (percentHealthChange != 0)
                    {
                        bool isDesiredHealthChange = percentHealthChange < 0 ^ target.TeamSide == performingAction.TeamSide;
                        value += Math.Abs(PercentChangeToValue * percentHealthChange) * (isDesiredHealthChange ? 1 : -1);
                    }
                }
            }

            return value;
        }
    }

    static class HealthPercentPolicy
    {
        public static float PercentToValue = 1;

        public static float CalculateActionValue(Character performingAction, CharacterPosition positioning, ActionInfo actionInfo, List<Character> actionTargets, Map map, bool highHealth)
        {
            float value = 0;

            foreach (Character target in actionTargets)
            {
                if (target.IsAlive)
                {
                    Resource health = target.CurrentResources[ResourceType.Health];
                    value += (highHealth ? health.Ratio : 1 -health.Ratio) * PercentToValue;
                }
            }

            return value;
        }
    }

    static class AvoidFrontalOrientationPolicy
    {
        public static float LateralReward = 0.5f;
        public static float BehindReward = 1;

        public static float CalculateActionValue(Character performingAction, CharacterPosition positioning, ActionInfo actionInfo, List<Character> actionTargets, Map map)
        {
            float value = 0;

            foreach (Character target in actionTargets)
            {
                RelativeOrientation relative = InteractionUtil.GetRelativeOrientation(performingAction, target, map);
                if (relative == RelativeOrientation.Behind)
                {
                    value += BehindReward;
                }
                else if (relative == RelativeOrientation.Left || relative == RelativeOrientation.Right)
                {
                    value += LateralReward;
                }
            }

            return value;
        }
    }
}
