using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    abstract class PropBase 
        : MonoBehaviour
        , Messaging.IMessageHandler
    {
        protected PropInfo PropInfo;

        // MonoBehaviour
        public virtual void Start()
        {
            Messaging.MessageRouter.Instance.RegisterHandler(this);

            List<Collider> colliders = new List<Collider>();
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                colliders.Add(collider);
            }
            else 
            {
                colliders.AddRange(GetComponentsInChildren<Collider>());
            }

            Debug.Assert(colliders.Count > 0);
            foreach (Collider propCollider in colliders)
            {
                propCollider.gameObject.layer = LayerMask.NameToLayer("Interactible");
            }
        }

        void OnDestroy()
        {
            Messaging.MessageRouter.Instance.UnRegisterHandler(this);
        }

        // Public
        public abstract void OnInteractionStart();
        public abstract void OnInteractionEnd();
        public virtual bool IsPropInteractable() { return PropInfo.IsInteractible; }
        public PropType GetPropType() { return PropInfo.Tag.PropType; }
        // Protected
        public abstract int GetPropId();
        protected virtual void Refresh()
        {
            ILevelManagerService levelManagerService = LevelManagementService.Get();
            PropInfo = levelManagerService.GetPropInfo(GetPropId());

            gameObject.SetActive(PropInfo.IsActive);
        }

        // IMessageHandler
        public void HandleMessage(Messaging.MessageInfoBase messageInfoBase)
        {
            switch (messageInfoBase.MessageId)
            {
                case LevelManagement.LevelMessage.Id:
                {
                    LevelManagement.LevelMessage message = messageInfoBase as LevelManagement.LevelMessage;
                    switch (message.Type)
                    {
                        case LevelManagement.MessageType.PropUpdated:
                        {
                            if (message.Arg<int>() == GetPropId())
                            {
                                Refresh();
                            }
                        }
                        break;
                        case LevelManagement.MessageType.LevelLoaded:
                        {
                            Refresh();
                        }
                        break;
                    }
                }
                break;
            }
        }
    }
}
