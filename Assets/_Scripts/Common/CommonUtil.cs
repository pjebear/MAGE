using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using Common.AttributeEnums;
namespace Common.CommonUtil
{
    namespace AttributeUtil
    {
        static class AttributeConstants
        {
            public static float DefaultMovement = 3f;
            public static float DefaultJump = 3f;
            public static float DefaultSpeed = 7;
            public static float DefaultEnduranceRecovery = 0.25f;

            public static float DefaultMight = 20f;
            public static float DefaultFinese = 20f;
            public static float DefaultMagic = 20f;

            public static float DefaultFortitude = 50f;
            public static float DefaultAttunement = 50f;
        }

        static class AttributeUtil
        {
            public const float MIGHT_TO_HEALTH_SCALAR = 4f;
            public const float FINESE_TO_ENDURANCE_SCALAR = 2f;
            public const float MAGIC_TO_MANA_SCALAR = 3f;

            public static void CalculateResourcesFromStats(float[] stats, float[] resources, bool HasInitialValues = true)
            {
                Debug.Assert(stats.Count() == (int)CharacterStats.NUM, "Stat array provided to CalculateResources invalid Size");
                Debug.Assert(resources.Count() == (int)Resource.NUM, "Resource array provided to CalculateResources invalid Size");

                float maxResourceValue = 0f;
                for (int resourceIndex = (int)Resource.MaxHealth; resourceIndex < (int)Resource.NUM; resourceIndex++)
                {
                    maxResourceValue = StatToMaxResource(stats, resourceIndex);
                    // if current resource already has value, scale it with max resource change. Else set to max value
                    resources[resourceIndex - 3] =
                        HasInitialValues ?
                        resources[resourceIndex - 3] * maxResourceValue / resources[resourceIndex] // current resource *= resource Increase
                        : maxResourceValue;
                    resources[resourceIndex] = maxResourceValue;
                }
            }

            public static float StatToMaxResource(float[] stats, int resourceIndex)
            {
                switch (resourceIndex)
                {
                    case ((int)Resource.MaxHealth):
                        return stats[(int)PrimaryStat.Might] * MIGHT_TO_HEALTH_SCALAR * (1 + stats[(int)SecondaryStat.Fortitude] / 100f); // 100 fortitude = +100 percent health
                    case ((int)Resource.MaxMana):
                        return stats[(int)PrimaryStat.Magic] * MAGIC_TO_MANA_SCALAR;
                    case ((int)Resource.MaxEndurance):
                        return stats[(int)PrimaryStat.Finese] * FINESE_TO_ENDURANCE_SCALAR;
                    default:
                        Debug.LogError("Invalid Resource supplied to StatToMaxResource. Got " + resourceIndex.ToString());
                        return 0f;
                }
            }

            public static AllignmentType[] GetSubAllignments(AllignmentType primaryAllignment)
            {
                AllignmentType[] subAllignments = new AllignmentType[2] { AllignmentType.Unalligned, AllignmentType.Unalligned };
                switch (primaryAllignment)
                {
                    case (AllignmentType.Fire):
                        subAllignments[0] = AllignmentType.Light;
                        subAllignments[1] = AllignmentType.Earth;
                        break;

                    case (AllignmentType.Water):
                        subAllignments[0] = AllignmentType.Sky;
                        subAllignments[1] = AllignmentType.Dark;
                        break;

                    case (AllignmentType.Sky):
                        subAllignments[0] = AllignmentType.Light;
                        subAllignments[1] = AllignmentType.Water;
                        break;

                    case (AllignmentType.Earth):
                        subAllignments[0] = AllignmentType.Water;
                        subAllignments[1] = AllignmentType.Fire;
                        break;

                    case (AllignmentType.Light):
                        subAllignments[0] = AllignmentType.Fire;
                        subAllignments[1] = AllignmentType.Sky;
                        break;

                    case (AllignmentType.Dark):
                        subAllignments[0] = AllignmentType.Earth;
                        subAllignments[1] = AllignmentType.Water;
                        break;
                }
                return subAllignments;
            }
        }
    }
    
    

    static class ProfessionUtil
    {
        
    }
}

namespace ActionUtil
{
    static class ActionConstants
    {
        public static readonly int InstantChargeTime = -1;
    }
}
