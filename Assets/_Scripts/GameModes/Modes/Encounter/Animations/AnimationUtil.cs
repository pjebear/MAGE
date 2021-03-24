using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes
{
    static class AnimationUtil
    {
        public static AnimationId InteractionResultTypeToAnimationId(InteractionResult result)
        {
            AnimationId animationId = AnimationId.INVALID;

            switch (result.InteractionResultType)
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
                {
                    if (!result.StateChange.IsBeneficial())
                    {
                        animationId = AnimationId.Hurt;
                    }
                }    
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
}


