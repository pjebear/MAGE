using MAGE.GameModes.Combat;
using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Encounter
{
    class DeferredInteractionResult
        : ConcreteVar<InteractionResult>
        , IDeferredVar<InteractionResultType>
        , IDeferredVar<StateChange>
    {
        InteractionResultType IDeferredVar<InteractionResultType>.Get()
        {
            return mVar.InteractionResultType;
        }

        StateChange IDeferredVar<StateChange>.Get()
        {
            return mVar.StateChange;
        }
    }

    class InteractionResultToAnimation : IDeferredVar<AnimationInfo>
    {
        protected IDeferredVar<InteractionResult> mDeferredVar;
        public InteractionResultToAnimation(IDeferredVar<InteractionResult> deferredVar)
        {
            mDeferredVar = deferredVar;
        }

        public AnimationInfo Get()
        {
            AnimationId animationId = AnimationUtil.InteractionResultTypeToAnimationId(mDeferredVar.Get());

            return AnimationFactory.CheckoutAnimation(animationId);
        }
    }
}
