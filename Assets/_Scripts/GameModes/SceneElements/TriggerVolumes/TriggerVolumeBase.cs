using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    abstract class TriggerVolumeBase<T> : MonoBehaviour where T : MonoBehaviour
    {
        private void Awake()
        {
            gameObject.layer = GetLayer();
        }

        protected abstract int GetLayer();

        protected virtual void HandleTriggerEntered(T entered) { }
        protected virtual void HandleTriggerExited(T exited) { }

        protected virtual void HandleCollisionEntered(T entered) { }
        protected virtual void HandleCollisionExited(T exited) { }

        void OnTriggerEnter(Collider collider)
        {
            T entered = collider.gameObject.GetComponent<T>();
            if (entered == null)
            {
                entered = collider.gameObject.GetComponentInParent<T>();
            }

            if (entered != null)
            {
                HandleTriggerEntered(entered);
            }
        }

        void OnTriggerExit(Collider collider)
        {
            T exited = collider.gameObject.GetComponent<T>();
            if (exited == null)
            {
                exited = collider.gameObject.GetComponentInParent<T>();
            }
            if (exited != null)
            {
                HandleTriggerExited(exited);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            T entered = collision.collider.gameObject.GetComponent<T>();
            if (entered != null)
            {
                HandleCollisionEntered(entered);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            T exited = collision.collider.gameObject.GetComponent<T>();
            if (exited != null)
            {
                HandleCollisionExited(exited);
            }
        }
    }
}
