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

            public static float MainAllignment = 1f;
            public static float SubAllignment = .3f;

            public static float LevelUpHpGain = 25f;
            public static float LevelUpMpGain = 10f;
            public static float LevelUpEndGain = 5f;

            public static float MightToHealth = 2f;
            public static float FineseToEndurance = 2f;
            public static float MagicToMana = 3f;

            public static float FortitudeHealthModifier = 0.01f;
        }

        static class AttributeUtil
        {

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
                        Mathf.Floor(resources[resourceIndex - 3] * maxResourceValue / resources[resourceIndex]) // current resource *= resource Increase
                        : maxResourceValue;
                    resources[resourceIndex] = maxResourceValue;
                }
            }

            public static float StatToMaxResource(float[] stats, int resourceIndex)
            {
                float newValue = 0f;
                switch (resourceIndex)
                {
                    case ((int)Resource.MaxHealth):
                        newValue = stats[(int)PrimaryStat.Might] * AttributeConstants.MightToHealth * (1 + stats[(int)SecondaryStat.Fortitude] * AttributeConstants.FortitudeHealthModifier); // 100 fortitude = +100 percent health
                        break;
                    case ((int)Resource.MaxMana):
                        newValue = stats[(int)PrimaryStat.Magic] * AttributeConstants.MagicToMana;
                        break;
                    case ((int)Resource.MaxEndurance):
                        newValue = stats[(int)PrimaryStat.Finese] * AttributeConstants.FineseToEndurance;
                        break;
                    default:
                        Debug.LogError("Invalid Resource supplied to StatToMaxResource. Got " + resourceIndex.ToString());
                        return 0f;
                }
                return Mathf.Floor(newValue);
            }

            public static AllignmentType[] GetSubAllignments(AllignmentType primaryAllignment)
            {
                AllignmentType[] subAllignments = new AllignmentType[2] { AllignmentType.Unalligned, AllignmentType.Unalligned };
                switch (primaryAllignment)
                {
                    case (AllignmentType.Fire):
                        subAllignments[0] = AllignmentType.Sky;
                        subAllignments[1] = AllignmentType.Earth;
                        break;

                    case (AllignmentType.Water):
                        subAllignments[0] = AllignmentType.Sky;
                        subAllignments[1] = AllignmentType.Earth;
                        break;

                    case (AllignmentType.Sky):
                        subAllignments[0] = AllignmentType.Fire;
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

            public static AllignmentType GetMainAllignment(float[] allignments)
            {
                for (int allignmentIndex = 0; allignmentIndex < (int)AttributeType.NUM; ++allignmentIndex)
                {
                    if (allignments[allignmentIndex] == AttributeConstants.MainAllignment)
                    {
                        return (AllignmentType)allignmentIndex;
                    }
                }
                Debug.Log("Character is not alligned to any allignment\n " + allignments);
                return AllignmentType.NUM;
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
