using MAGE.GameSystems;
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
    class ActorSpawner 
        : MonoBehaviour
        , Messaging.IMessageHandler
    {
        public interface IRefreshListener
        {
            void OnActorRefresh();
        }

        public CharacterPicker CharacterPicker;

        public Actor Actor = null;
        public Actor SpawnerPlaceHolder = null;
        public bool RefreshOnStart = true;
        public bool RefreshOnUpdate = true;

        public Appearance Appearance;
        public string Name;

        private HashSet<IRefreshListener> mRefreshListeners = new HashSet<IRefreshListener>();

        private void Awake()
        {
            SpawnerPlaceHolder = GetComponentInChildren<Actor>();
            Logger.Assert(SpawnerPlaceHolder != null, LogTag.Character, "ActorSpawner", "No Placeholder actor found in spawner", LogLevel.Warning);

            Actor = SpawnerPlaceHolder;
        }

        private void OnDestroy()
        {
            Messaging.MessageRouter.Instance.UnRegisterHandler(this);
        }

        private void Start()
        {
            Messaging.MessageRouter.Instance.RegisterHandler(this);

            if (RefreshOnStart)
            {
                Refresh();
            }
        }

        public void Refresh()
        {
            Appearance = CharacterPicker.GetAppearance();
            
            if (Appearance != null)
            {
                SpawnerPlaceHolder.gameObject.SetActive(false);

                Actor oldActor = Actor;

                Actor = ActorLoader.Instance.CreateActor(Appearance, transform);

                if (oldActor != null)
                {
                    Actor.gameObject.layer = oldActor.gameObject.layer;
                    if (oldActor != SpawnerPlaceHolder)
                    {
                        Destroy(oldActor.gameObject);
                    }
                }
            }
            else
            {
                SpawnerPlaceHolder.gameObject.SetActive(true);
                Actor = SpawnerPlaceHolder;
            }

            Name = CharacterPicker.GetActorName();
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
                            if (message.Arg<int>() == CharacterPicker.GetActorId())
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



