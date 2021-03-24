using MAGE.GameSystems.Appearances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    [CreateAssetMenu(fileName ="New SkinTone", menuName = "Props/SkinTone")]
    class SkinTone : ScriptableObject
    {
        public SkinToneType Type;
        public Texture2D HeadTexture;
        public Texture2D BodyTexture;
    }
}
