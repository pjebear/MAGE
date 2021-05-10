using MAGE.Common;
using MAGE.GameSystems;
using MAGE.GameSystems.Appearances;
using MAGE.GameSystems.Characters;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    [RequireComponent(typeof(CharacterPickerControl))]
    class ActorSpawner 
        : MonoBehaviour
        , Messaging.IMessageHandler
    {
        public interface IRefreshListener
        {
            void OnActorRefresh();
        }

        public bool RefreshOnStart = true;

        public Appearance Appearance;
        public string Name;

        private HashSet<IRefreshListener> mRefreshListeners = new HashSet<IRefreshListener>();

        private void Awake()
        {
           
        }

        private void OnDestroy()
        {
            if (Messaging.MessageRouter.Instance != null)
            {
                Messaging.MessageRouter.Instance.UnRegisterHandler(this);
            }
            else
            {
                Logger.Log(LogTag.Level, "ActorSpawner", "MessageRouter not initialized", LogLevel.Warning);
            }
        }

        private void Start()
        {
            if (Messaging.MessageRouter.Instance != null)
            {
                Messaging.MessageRouter.Instance.RegisterHandler(this);
            }
            else
            {
                Logger.Log(LogTag.Level, "ActorSpawner", "MessageRouter not initialized", LogLevel.Warning);
            }

            if (RefreshOnStart)
            {
                Refresh();
            }
        }

        public void Refresh()
        {
            CharacterPickerControl characterPickerControl = GetComponent<CharacterPickerControl>();
            if (characterPickerControl.IsNPC())
            {
                Appearance = LevelManagementService.Get().GetNPCAppearance((NPCPropId)characterPickerControl.GetCharacterId());
            }
            else
            {
                Appearance = CharacterService.Get().GetCharacter(characterPickerControl.GetCharacterId()).GetAppearance();
            }
            

            if (Appearance != null)
            {
                Body currentBody = GetComponentInChildren<Body>();
                if (currentBody == null || currentBody.BodyType != Appearance.BodyType)
                {
                    Destroy(currentBody.gameObject);

                    string bodyPath = "Props/Bodies/";
                    switch (Appearance.BodyType)
                    {
                        case BodyType.HumanoidMale: bodyPath += "Humanoid/HumanoidMale"; break;
                        case BodyType.Bear_0: bodyPath += "Bear/Bear_0"; break;

                    }
                    Body newBody = Instantiate(Resources.Load<Body>(bodyPath), transform);
                }

                // TEMP: only try to outfit humans
                if (GetComponentInChildren<ActorOutfitter>() != null)
                {
                    GetComponentInChildren<ActorOutfitter>().UpdateAppearance(Appearance);
                }
            }
            
            Name = GetComponent<CharacterPickerControl>().GetActorName();
            gameObject.name = Name;

            NotifyRefresh();
        }

        // IMessageHandler
        public void HandleMessage(Messaging.MessageInfoBase messageInfoBase)
        {
            switch (messageInfoBase.MessageId)
            {
                case GameModes.LevelManagement.LevelMessage.Id:
                {
                    LevelManagement.LevelMessage message = messageInfoBase as LevelManagement.LevelMessage;
                    switch (message.Type)
                    {
                        case LevelManagement.MessageType.AppearanceUpdated:
                        {
                            if (message.Arg<int>() == GetComponent<CharacterPickerControl>().GetCharacterId())
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
                case CharacterMessage.Id:
                {
                    CharacterMessage message = messageInfoBase as CharacterMessage;
                    switch (message.Type)
                    {
                        case CharacterMessage.MessageType.CharacterUpdated:
                        {
                            if (message.Arg<int>() == GetComponent<CharacterPickerControl>().GetCharacterId())
                            {
                                Refresh();
                            }
                        }
                        break;
                    }
                }
                break;
            }
        }

        private void NotifyRefresh()
        {
            HashSet<IRefreshListener> copy = new HashSet<IRefreshListener>(mRefreshListeners);
            foreach (IRefreshListener listener in copy)
            {
                listener.OnActorRefresh();
            }
        }

        public void RegisterListener(IRefreshListener refreshListener)
        {
            mRefreshListeners.Add(refreshListener);
        }

        public void UnRegisterListener(IRefreshListener refreshListener)
        {
            mRefreshListeners.Remove(refreshListener);
        }
    }
}



