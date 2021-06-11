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
    class ConcreteVar<T> : IDeferredVar<T>
    {
        protected T mVar;
        public ConcreteVar(T var)
        {
            Set(var);
        }
        public ConcreteVar()
        {
            mVar = default;
        }
        public void Set(T var)
        {
            mVar = var;
        }

        public T Get()
        {
            return mVar;
        }
    }

    abstract class DeferredConversion<T, U> : ConcreteVar<T>, IDeferredVar<U>
    {
        protected abstract U Convert();

        U IDeferredVar<U>.Get()
        {
            return Convert();
        }
    }

    class DeferredTransform<T> : DeferredConversion<IDeferredVar<T>, Transform> where T : MonoBehaviour
    {
        public DeferredTransform(IDeferredVar<T> toConvert)
        {
            mVar = toConvert;
        }
        protected override Transform Convert()
        {
            return mVar.Get()?.transform;
        }
    }

    class DeferredTargetPosition : DeferredConversion<IDeferredVar<Target>, Vector3> 
    {
        public DeferredTargetPosition(IDeferredVar<Target> toConvert)
        {
            mVar = toConvert;
        }
        protected override Vector3 Convert()
        {
            return mVar.Get().GetTargetPoint();
        }
    }

    class DeferredTargetAnimator : DeferredConversion<IDeferredVar<Target>, ActorAnimator>
    {
        public DeferredTargetAnimator(IDeferredVar<Target> toConvert)
        {
            mVar = toConvert;
        }
        protected override ActorAnimator Convert()
        {
            return mVar.Get().FocalTarget?.GetComponent<ActorAnimator>();
        }
    }

    class TargetPositionConversion : DeferredConversion<Target, Vector3>
    {
        public TargetPositionConversion(Target toConvert)
        {
            mVar = toConvert;
        }
        protected override Vector3 Convert()
        {
            return mVar.GetTargetPoint();
        }
    }

    class PositionConversion<T> : DeferredConversion<T, Vector3> where T : MonoBehaviour
    {
        public PositionConversion(T toConvert)
        {
            mVar = toConvert;
        }
        protected override Vector3 Convert()
        {
            return mVar.transform.position;
        }
    }

    class MonoConversion<T, U> : DeferredConversion<T, U> where T : MonoBehaviour where U : MonoBehaviour
    {
        public MonoConversion(T toConvert)
        {
            mVar = toConvert;
        }
        protected override U Convert()
        {
            return mVar.GetComponent<U>();
        }
    }

    class DeferredMonoConversion<T, U> : DeferredConversion<IDeferredVar<T>, U> where T : MonoBehaviour where U : MonoBehaviour
    {
        public DeferredMonoConversion(IDeferredVar<T> toConvert)
        {
            mVar = toConvert;
        }
        protected override U Convert()
        {
            return mVar.Get().GetComponent<U>();
        }
    }

    class DeferredStateChange : IDeferredVar<StateChange>
    {
        public IDeferredVar<int> HealthChange;
        public IDeferredVar<int> ResourceChange;
        public IDeferredVar<List<StatusEffectId>> StatusEffects;

        public StateChange Get()
        {
            int healthChange = 0;
            if (HealthChange != null)
            {
                healthChange = HealthChange.Get();
            }

            int resourceChange = 0;
            if (ResourceChange != null)
            {
                resourceChange = ResourceChange.Get();
            }

            List<StatusEffect> statusEffects = new List<StatusEffect>();
            if (StatusEffects != null)
            {
                foreach (StatusEffectId statusEffectId in StatusEffects.Get())
                {
                    statusEffects.Add(StatusEffectFactory.CheckoutStatusEffect(statusEffectId));
                }
            }

            return new StateChange(StateChangeType.ActionTarget, healthChange, resourceChange, statusEffects);
        }
    }

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
