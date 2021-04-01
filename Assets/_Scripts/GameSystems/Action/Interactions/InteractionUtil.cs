using MAGE.GameSystems;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameSystems.Actions
{
    static class InteractionUtil
    {
        public static InteractionResultType GetOwnerResultTypeFromResults(List<InteractionResult> results)
        {
            InteractionResultType interactionResultType = InteractionResultType.Miss;

            foreach (InteractionResult interactionResult in results)
            {
                if (interactionResult.InteractionResultType == InteractionResultType.Hit)
                {
                    interactionResultType = InteractionResultType.Hit;
                    break;
                }
                else
                {
                    // don't break. Keep trying to find a hit
                    interactionResultType = InteractionResultType.Partial;
                }
            }

            return interactionResultType;
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

        public static RelativeOrientation GetRelativeOrientation(Transform relative, Transform to)
        {

            Vector3 toForward = to.forward;
            Vector3 toToRealtive = relative.position - to.position;

            float degreesBetweenForwardAndRelative = Vector3.SignedAngle(toForward, toToRealtive, Vector3.up);

            RelativeOrientation relativeOrientation = RelativeOrientation.Behind;

            if (Mathf.Abs(degreesBetweenForwardAndRelative) < 45)
            {
                relativeOrientation = RelativeOrientation.Front;
            }
            else if (Mathf.Abs(degreesBetweenForwardAndRelative) > 90)
            {
                relativeOrientation = RelativeOrientation.Behind;
            }
            else if (degreesBetweenForwardAndRelative < 0)
            {
                relativeOrientation = RelativeOrientation.Left;
            }
            else
            {
                relativeOrientation = RelativeOrientation.Right;
            }

            return relativeOrientation;
        }
    }


}
