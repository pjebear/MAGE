using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

static class InteractionUtil
{
    public static RelativeOrientation GetRelativeOrientation(Transform relative, Transform to)
    {
        RelativeOrientation relativeOrientation = RelativeOrientation.Behind;

        Vector3 displacementVec = relative.localPosition - to.localPosition;

        Vector3 toForward = to.forward;

        float angleDeg = Vector3.SignedAngle(toForward, displacementVec, Vector3.up);

        if (angleDeg == 0)
        {
            relativeOrientation = RelativeOrientation.Front;
        }
        else if (Mathf.Abs(angleDeg) <= 90)
        {
            if (angleDeg < 0)
            {
                relativeOrientation = RelativeOrientation.Left;
            }
            else
            {
                relativeOrientation = RelativeOrientation.Right;
            }
        }
        else
        {
            relativeOrientation = RelativeOrientation.Behind;
        }

        return relativeOrientation;
    }

    public static Orientation GetOrientation(Transform transform)
    {
        Orientation orientation = Orientation.Back;

        if (transform.forward == Vector3.forward)
        {
            orientation = Orientation.Forward;
        }
        else if (transform.forward == Vector3.right)
        {
            orientation = Orientation.Right;
        }
        else if (transform.forward == Vector3.left)
        {
            orientation = Orientation.Left;
        }
        else
        {
            orientation = Orientation.Back;
        }

        return orientation;
    }

    public static SFXId GetSFXForInteractionResult(InteractionResultType resultType)
    {
        SFXId sFXId = SFXId.INVALID;

        switch (resultType)
        {
            case InteractionResultType.Block: sFXId = SFXId.ShieldBlock; break;
            case InteractionResultType.Dodge: sFXId = SFXId.Dodge; break;
            case InteractionResultType.Parry: sFXId = SFXId.Parry; break;
            case InteractionResultType.Hit: sFXId = SFXId.Slash; break;
        }
        return sFXId;
    }
}

