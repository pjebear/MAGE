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


    class MonoConversion<T,U> : DeferredConversion<IDeferredVar<T>,U> where T : MonoBehaviour where U : MonoBehaviour
    {
        public MonoConversion(IDeferredVar<T> toConvert)
        {
            mVar = toConvert;
        }
        protected override U Convert()
        {
            return mVar.Get().GetComponent<U>();
        }
    }
}
