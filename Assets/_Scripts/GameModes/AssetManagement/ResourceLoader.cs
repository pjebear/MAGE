using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.Common
{
    static class ResourceConstants
    {
        public static readonly string PropPath = "Props";

        public static readonly string BodyPath = Path.Combine(PropPath, "Bodies");
        public static readonly string HumanoidBodyPath = Path.Combine(BodyPath, "Humanoid");
        public static readonly string HumanoidSkinTonePath = Path.Combine(HumanoidBodyPath, "SkinMaterials/SkinTones");

        public static readonly string ApparelPath = Path.Combine(PropPath, "Apparel");
        public static readonly string OutfitsPath = Path.Combine(ApparelPath, "Arrangements");
        public static readonly string HeldApparelPath = Path.Combine(ApparelPath, "Held");
        public static readonly string HairPath = Path.Combine(ApparelPath, "Hair");
    }
    static class ResourceLoader
    {
        public static T Load<T, U>(string path, U type)
            where T : UnityEngine.Object
            where U : Enum
        {
            return Load<T>(path, type.ToString());
        }

        public static T Load<T>(string path, string name)
            where T : UnityEngine.Object
        {
            string fullPath = Path.Combine(path, name);
            return Resources.Load<T>(fullPath);
        }
    }
}

