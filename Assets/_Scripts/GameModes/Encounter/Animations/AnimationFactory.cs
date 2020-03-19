using System.Collections.Generic;

static class AnimationFactory
{
    public static AnimationPlaceholder CheckoutAnimation(AnimationId id)
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

        return new AnimationPlaceholder(id.ToString(), numFrames, syncFrame);
    }
}
