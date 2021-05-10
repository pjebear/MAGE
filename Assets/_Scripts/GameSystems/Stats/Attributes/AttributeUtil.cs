
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Stats
{
    static class AttributeUtil
    {
        public static string ToString(AttributeCategory category, int index)
        {
            string attributeName = "NAME";

            switch (category)
            {
                case AttributeCategory.PrimaryStat: attributeName = ((PrimaryStat)index).ToString(); break;
                case AttributeCategory.SecondaryStat: attributeName = ((SecondaryStat)index).ToString(); break;
                case AttributeCategory.TertiaryStat: attributeName = ((TertiaryStat)index).ToString(); break;
                case AttributeCategory.Resource: attributeName = ((ResourceType)index).ToString(); break;
                case AttributeCategory.Status: attributeName = ((StatusType)index).ToString(); break;
                case AttributeCategory.Allignment: attributeName = ((AllignmentType)index).ToString(); break;
                default: Debug.Assert(false); break;
            }

            return attributeName;
        }

        public static int ResourceFromAttribtues(ResourceType resourceType, Attributes stats)
        {
            float resourceValue = stats[AttributeCategory.Resource][(int)resourceType].Value;

            switch (resourceType)
            {
                case (ResourceType.Health):
                    resourceValue += stats[PrimaryStat.Might];
                    break;
                case (ResourceType.Mana):
                    resourceValue += stats[PrimaryStat.Magic];
                    break;
                case (ResourceType.Endurance):
                    resourceValue += stats[PrimaryStat.Finese];
                    break;
                case (ResourceType.Clock):
                    resourceValue += stats[TertiaryStat.MaxClockGuage];
                    break;
                case (ResourceType.Actions):
                    resourceValue += stats[TertiaryStat.Actions];
                    break;
                case (ResourceType.MovementRange):
                    resourceValue += stats[TertiaryStat.Movement];
                    break;
            }

            return (int)resourceValue;
        }

        public static Resource.ScaleType GetScaleTypeForResource(ResourceType resourceType)
        {
            Resource.ScaleType scaleType = Resource.ScaleType.Scale;

            switch (resourceType)
            {
                case ResourceType.MovementRange:
                case ResourceType.Actions:
                {
                    scaleType = Resource.ScaleType.Flat;
                }
                break;
                default:
                {
                    scaleType = Resource.ScaleType.Scale;
                }
                break;
            }

            return scaleType;
        }
    }
}
