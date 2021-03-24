using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    abstract class PartyAvatarTriggerVolumeBase : MonoBehaviour
    {
        protected virtual void HandleTriggerEntered() { }
        protected virtual void HandleTriggerExited() { }

        protected virtual void HandleCollisionEntered() { }
        protected virtual void HandleCollisionExited() { }

        void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.GetComponent<ThirdPersonActorController>() != null)
            {
                HandleTriggerEntered();
            }
        }

        void OnTriggerExit(Collider collider)
        {
            if (collider.gameObject.GetComponent<ThirdPersonActorController>() != null)
            {
                HandleTriggerExited();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.gameObject.GetComponent<ThirdPersonActorController>() != null)
            {
                HandleCollisionEntered();
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.collider.gameObject.GetComponent<ThirdPersonActorController>() != null)
            {
                HandleCollisionExited();
            }
        }
    }
}
