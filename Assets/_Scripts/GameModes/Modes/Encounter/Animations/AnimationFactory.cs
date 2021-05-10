using MAGE.GameSystems;
using System.Collections.Generic;

namespace MAGE.GameModes
{
    static class AnimationFactory
    {
        public static AnimationInfo CheckoutAnimation(AnimationId id)
        {
            int numFrames = 60;
            int syncFrame = 30;
            string animationName = "";
            switch (id)
            {
                case AnimationId.SwordSwing:
                {
                    numFrames = 55;
                    syncFrame = 18;
                }
                break;
                case AnimationId.BowDraw:
                {
                    numFrames = 35;
                    syncFrame = 35;
                }
                break;
                case AnimationId.Hurt:
                {
                    numFrames = 25;
                    syncFrame = 3;
                }
                break;
                case AnimationId.Block:
                case AnimationId.Cleave:
                case AnimationId.Parry:
                
                    numFrames = 75;
                    syncFrame = 26;
                    break;

                case AnimationId.Dodge:
                    numFrames = 48;
                    syncFrame = 35;
                    break;

                case AnimationId.Cast:
                    numFrames = 50;
                    syncFrame = 50;
                    break;
            }

            switch (id)
            {
                case AnimationId.SwordSwing:
                    animationName = "attack1Right";
                    break;
                case AnimationId.Cleave:
                    animationName = "parryRight";
                    break;
                case AnimationId.BowDraw:
                    animationName = "bowShoot1Left";
                    break;
                case AnimationId.Block:
                    animationName = "blockLeft";
                    break;
                case AnimationId.Parry:
                    animationName = "parryRight";
                    break;
                case AnimationId.Dodge:
                    animationName = "dodge";
                    break;
                case AnimationId.Hurt:
                    animationName = "gotHit";
                    break;
                case AnimationId.Cast:
                    animationName = "cast3";
                    break;
            }

            SFXId sFXId = SFXId.INVALID;
            switch (id)
            {
                case AnimationId.SwordSwing: sFXId = SFXId.WeaponSwing; break;
                case AnimationId.Cleave: sFXId = SFXId.WeaponSwing; break;
                case AnimationId.Block: sFXId = SFXId.ShieldBlock; break;
                case AnimationId.Dodge: sFXId = SFXId.Dodge; break;
                case AnimationId.Parry: sFXId = SFXId.Parry; break;
                case AnimationId.Hurt: sFXId = SFXId.Slash; break;
                case AnimationId.BowDraw: sFXId = SFXId.ArrowDraw; break;
                case AnimationId.Cast: sFXId = SFXId.Cast; break;
                case AnimationId.Faint: sFXId = SFXId.MaleDeath; break;
            }

            return new AnimationInfo(animationName == "" ? id.ToString() : animationName, numFrames, syncFrame, sFXId);
        }
    }
}


