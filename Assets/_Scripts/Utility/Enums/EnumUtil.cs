using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.Utility.Enums
{
    static class EnumUtil
    {
        public static T StringToEnum<T>(string toConvert) where T : System.Enum
        {
            T picked = default;

            bool foundValue = false;

            foreach (T enumVal in Enum.GetValues(typeof(T)))
            {
                if (enumVal.ToString() == toConvert)
                {
                    foundValue = true;
                    picked = enumVal;
                    break;
                }
            }

            Debug.Assert(foundValue, string.Format("Failed to find value of {0} in Enum {1}", toConvert, typeof(T).Name));
            return picked;
        }
    }
}
