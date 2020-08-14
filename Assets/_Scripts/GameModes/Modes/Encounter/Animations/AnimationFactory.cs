using MAGE.GameServices;
using System.Collections.Generic;

namespace MAGE.GameModes
{
    static class AnimationFactory
    {
        public static AnimationInfo CheckoutAnimation(AnimationId id)
        {
            int numFrames = 60;
            int syncFrame = 30;
            switch (id)
            {
                case AnimationId.SwordSwing:
                case AnimationId.Block:
                case AnimationId.Dodge:
                case AnimationId.Parry:
                case AnimationId.Hurt:
                    numFrames = 60;
                    syncFrame = 30;
                    break;

                case AnimationId.Cast:
                    numFrames = 90;
                    syncFrame = 60;
                    break;
            }

            SFXId sFXId = SFXId.INVALID;
            switch (id)
            {
                case AnimationId.SwordSwing: sFXId = SFXId.WeaponSwing; break;
                case AnimationId.Block: sFXId = SFXId.ShieldBlock; break;
                case AnimationId.Dodge: sFXId = SFXId.Dodge; break;
                case AnimationId.Parry: sFXId = SFXId.Parry; break;
                case AnimationId.Hurt: sFXId = SFXId.Slash; break;
                case AnimationId.BowDraw: sFXId = SFXId.ArrowDraw; break;
                case AnimationId.Cast: sFXId = SFXId.Cast; break;
                case AnimationId.Faint: sFXId = SFXId.MaleDeath; break;
            }

            return new AnimationInfo(id.ToString(), numFrames, syncFrame, sFXId);
        }
    }
}


