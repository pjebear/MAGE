using MAGE.GameServices;
using MAGE.GameServices.Character;

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
        public CharacterType CharacterType = CharacterType.INVALID;

        // Create Character
        public int CreateCharacterId = CharacterConstants.INVALID_ID;

        // Story Character
        public StoryCharacterId StoryCharacterId = StoryCharacterId.INVALID;

        // Scenario Character
        public ScenarioId ScenarioId = ScenarioId.INVALID;
        public int ScenarioCharacterOffset = 0;

        // PropId
        public NPCPropId NPCId = NPCPropId.None;

        public Actor Actor = null;
        public Actor SpawnerPlaceHolder = null;
        public bool RefreshOnStart = true;
        public bool RefreshOnUpdate = true;

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

        public virtual int GetAppearanceId()
        {
            int appearanceId = -1;

            if (CharacterType != CharacterType.INVALID)
            {
                if (StoryCharacterId != StoryCharacterId.INVALID)
                {
                    appearanceId = GameServices.CharacterService.Get().GetCharacterAppearanceId((int)StoryCharacterId);
                }
                else if (ScenarioId != ScenarioId.INVALID)
                {
                    int characterId = CharacterUtil.ScenarioIdToDBId(ScenarioId, ScenarioCharacterOffset);
                    appearanceId = GameServices.CharacterService.Get().GetCharacterAppearanceId(characterId);
                }
                else if (CreateCharacterId != CharacterConstants.INVALID_ID)
                {
                    appearanceId = GameServices.CharacterService.Get().GetCharacterAppearanceId(CreateCharacterId);
                }
            }
            else if (NPCId != NPCPropId.None)
            {
                appearanceId = LevelManagementService.Get().GetPropInfo((int)NPCId).AppearanceId;
            }

            return appearanceId;
        }

        public virtual int GetActorId()
        {
            int actorId = -1;

            if (CharacterType != CharacterType.INVALID)
            {
                if (StoryCharacterId != StoryCharacterId.INVALID)
                {
                    actorId = (int)StoryCharacterId;
                }
                else if (ScenarioId != ScenarioId.INVALID)
                {
                    actorId = CharacterUtil.ScenarioIdToDBId(ScenarioId, ScenarioCharacterOffset);
                }
                else if (CreateCharacterId != CharacterConstants.INVALID_ID)
                {
                    actorId = CreateCharacterId;
                }
            }
            else if (NPCId != NPCPropId.None)
            {
                actorId = (int)NPCId;
            }

            return actorId;
        }

        public void Refresh()
        {
            int appearanceId = GetAppearanceId();
            if (appearanceId != -1)
            {
                SpawnerPlaceHolder.gameObject.SetActive(false);

                if (Actor != null && Actor != SpawnerPlaceHolder)
                {
                    Destroy(Actor.gameObject);
                }

                Appearance appearance = LevelManagementService.Get().GetAppearance(appearanceId);

                Actor = ActorLoader.Instance.CreateActor(appearance, transform);

            }
            else
            {
                SpawnerPlaceHolder.gameObject.SetActive(true);
                Actor = SpawnerPlaceHolder;
            }
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
                            if (message.Arg<int>() == GetAppearanceId())
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
    }
}



