using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.AI
{
    // These need work for sure

    enum PositioningType
    {
        LoneWolf,
        FrontLine,
        Support
    }

    enum AggressionType
    {
        Normal,
        Aggressive,
        Defensive,
        Cowardly
    }
        
    // All modifiers will exist in the range of -1 (retreat) to 1 (advance). 
    static class AIConstants
    {
        /* @normalizedDistanceToEnemies: 
         * > 1: behind allied line relative to enemies
         * 1 > > 0: between allies and enemies
         * < 0 behind enemies relative to allies
         */
        private static readonly float FRONTLINE_MODIFIER = 0.5f;
        public static float GetPositionModifier(PositioningType preference, float normalizedDistanceToEnemies)
        {
            
            float modifier = 0f;
            switch (preference)
            {
                case (PositioningType.LoneWolf):
                    if (normalizedDistanceToEnemies > 1)
                    {
                        modifier = 1f;
                    }
                    else if (normalizedDistanceToEnemies > 0)
                    {
                        modifier = 0.5f;
                    }
                    else
                    {
                        //behind enemy lines. Perfect
                    }
                    break;

                case (PositioningType.FrontLine):
                    if (normalizedDistanceToEnemies > 1)
                    {
                        modifier = FRONTLINE_MODIFIER;
                    }
                    else if (normalizedDistanceToEnemies > 0)
                    {
                        modifier = normalizedDistanceToEnemies * normalizedDistanceToEnemies;
                    }
                    else
                    {
                        modifier = -FRONTLINE_MODIFIER;
                    }
                    break;

                case (PositioningType.Support):
                    if (normalizedDistanceToEnemies < 1)
                    {
                        modifier = -0.5f;
                    }
                    break;
            }
            return modifier;
        }

        public static float GetAggressionModifier(AggressionType personality)
        {
            switch (personality)
            {
                case (AggressionType.Normal):
                    return .25f;
                case (AggressionType.Aggressive):
                    return 1f;
                case (AggressionType.Defensive):
                    return 0f;
                case (AggressionType.Cowardly):
                    return -1f;
                default:
                    return 0;
            }
        }


        public static float GetHealthModifier(float percentHealth)
        {
            if (percentHealth >= 0.75f)
            {
                return 1f;
            }
            else if (percentHealth >= 0.5f)
            {
                return .5f;
            }
            else if (percentHealth >= 0.25f)
            {
                return 0f;
            }
            else
            {
                return -1f;
            }
        }
    }
}
