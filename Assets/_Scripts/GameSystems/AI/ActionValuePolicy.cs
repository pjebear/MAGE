using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Stats;
using MAGE.GameSystems.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameSystems.AI
{
    enum ActionValuePolicyType
    {
        HealthChange,
        HighHealthPercent,
        LowHealthPercent,
        AvoidFrontalOrientationPolicy,
        Summon,

        NUM
    }

    static class ActionPolicyConstants
    {
        public static float LARGE_WEIGHTING = Mathf.Pow(10, 2);
        public static float MEDIUM_WEIGHTING = Mathf.Pow(10, 1);
        public static float SMALL_WEIGHTING = Mathf.Pow(10,0);
    }

    static class HealthChangePolicy
    {
        public static float PercentChangeToValue = ActionPolicyConstants.MEDIUM_WEIGHTING;

        public static float CalculateActionValue(Character performingAction, CharacterPosition positioning, ActionInfoBase actionInfo, List<Character> actionTargets, Map map)
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
        public static float PercentToValue = ActionPolicyConstants.MEDIUM_WEIGHTING;

        public static float CalculateActionValue(Character performingAction, CharacterPosition positioning, ActionInfoBase actionInfo, List<Character> actionTargets, Map map, bool highHealth)
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
        public static float LateralReward = ActionPolicyConstants.MEDIUM_WEIGHTING / 2;
        public static float BehindReward = ActionPolicyConstants.MEDIUM_WEIGHTING;

        public static float CalculateActionValue(Character performingAction, CharacterPosition positioning, ActionInfoBase actionInfo, List<Character> actionTargets, Map map)
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

    static class FriendlyFirePolicy
    {
        public static float CalculateActionValue(Character performingAction, CharacterPosition positioning, ActionInfoBase actionInfo, List<Character> actionTargets, Map map)
        {
            float value = 0;

            foreach (Character target in actionTargets)
            {
                StateChange stateChange = actionInfo.GetTargetStateChange(performingAction, target);
                if (stateChange.healthChange < 0 && performingAction.TeamSide == target.TeamSide)
                {
                    value += -ActionPolicyConstants.LARGE_WEIGHTING;
                }
            }

            return value;
        }
    }

    static class SummonPolicy
    {
        public static float CalculateActionValue(Character performingAction, CharacterPosition positioning, ActionInfoBase actionInfo, List<Character> actionTargets, Map map)
        {
            float value = 0;
            if (actionInfo.SummonInfo.SummonType != SpecializationType.INVALID)
            {
                float maxDistance = new Vector2(map.Width, map.Length).magnitude;

                TileIdx focalPoint = map.GetFocalPositionForTeam(EncounterUtil.GetOpponentTeamSide(performingAction.TeamSide));
                float distanceTo = TileIdx.DistanceBetween(focalPoint, positioning.Location);

                value = ActionPolicyConstants.SMALL_WEIGHTING * (1 - distanceTo / maxDistance);
            }
            return value;
        }
    }
}
