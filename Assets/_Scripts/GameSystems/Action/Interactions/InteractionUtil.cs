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

        public static RelativeOrientation GetRelativeOrientation(Character relative, Character to, Map map)
        {
            RelativeOrientation relativeOrientation = RelativeOrientation.Behind;

            Vector2 displacementVec = map.DisplacementBetween(to, relative);
            Orientation toOrientation = map.GetCharacterPosition(to).Orientation;
            Vector2 relativeDisplacement = GetRelativeDisplacement(displacementVec, toOrientation);

            if (relativeDisplacement.y < 0)
            {
                relativeOrientation = RelativeOrientation.Behind;
            }
            else if (relativeDisplacement.x < 0)
            {
                relativeOrientation = RelativeOrientation.Left;
            }
            else if (relativeDisplacement.x > 0)
            {
                relativeOrientation = RelativeOrientation.Right;
            }
            else
            {
                relativeOrientation = RelativeOrientation.Front;
            }

            return relativeOrientation;
        }

        public static Vector2 GetRelativeDisplacement(Vector2 absoluteDisplacement, Orientation orientation)
        {
            float rotationAmountDeg = 0;

            switch (orientation)
            {
                case Orientation.Forward: rotationAmountDeg = 0; break;
                case Orientation.Right: rotationAmountDeg = 90; break;
                case Orientation.Back: rotationAmountDeg = 180; break;
                case Orientation.Left: rotationAmountDeg = 270; break;
            }

            Quaternion rotationQuat = Quaternion.Euler(0, rotationAmountDeg, 0);
            Vector3 expandedDisplacement = new Vector3(absoluteDisplacement.x, 0, absoluteDisplacement.y);
            expandedDisplacement = rotationQuat * expandedDisplacement;

            return new Vector2(expandedDisplacement.x, expandedDisplacement.z);
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


}
