using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

static class AnimationUtil
{
    public static AnimationId InteractionResultTypeToAnimationId(InteractionResultType resultType)
    {
        AnimationId animationId = AnimationId.INVALID;

        switch (resultType)
        {
            case InteractionResultType.Block:
                animationId = AnimationId.Block;
                break;

            case InteractionResultType.Dodge:
                animationId = AnimationId.Dodge;
                break;

            case InteractionResultType.Parry:
                animationId = AnimationId.Parry;
                break;

            case InteractionResultType.Hit:
            case InteractionResultType.Partial:
            case InteractionResultType.Resist:
                animationId = AnimationId.Hurt;
                break;

            case InteractionResultType.Miss:
                animationId = AnimationId.Dodge;
                break;

            default:
                Debug.Assert(false);
                break;
        }

        return animationId;
    }
}

