using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    static class ActorAnimationContstants
    {
        public const float ANIM_MOVE_DAMP_TIME = 0.1f;

        public const string ANIM_MOVE_SPEED = "locomotion";
    }

    static class ActorConstants
    {
        public const float MAX_SPEED = 5.0f;
        public const float WALK_SPEED = MAX_SPEED / 4f;

        public const string HUMANOID_MALE_SKIN_PATH = "Props/Bodies/Humanoid/SkinMaterials";
    }

    static class HumanoidActorConstants
    {
        public enum BodyMaterialSlot
        {
            Body,
            Head,
            EyeBrow,
            EyeColor,
            EyeReflection,
            NUM
        }

        public enum HairMaterialSlot
        {
            Color,
            NUM
        }

        public enum Hand
        {
            Left,
            Right,

            NUM
        }

        public enum HeldApparelState
        {
            MeleeHeld,
            RangedHeld,
            Holstered,
            Hidden,

            NUM
        }

        public enum HeldApparelType
        {
            Melee,
            Ranged,

            NUM
        }

        public static readonly List<Color> HairColors = new List<Color>()
        {
            new Color(.388f, .251f, .255f) // brunette
            , new Color(.98f, .941f, .745f) // blonde
            , new Color(0, 0.05f, 0.11f) // black
            , new Color(0.69f, .396f, 0f) // red
            , new Color(0.753f, 0.753f, 0.753f) // silver
            , new Color(0.984f, 0.949f, 0.987f) // White
        };
    }
}
