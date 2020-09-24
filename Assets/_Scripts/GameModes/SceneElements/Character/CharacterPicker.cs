using MAGE.GameSystems;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    [Serializable]
    class CharacterPicker
    {
        public int RootCharacterId = -1;

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

        public virtual Appearance GetAppearance()
        {
            Appearance appearace = null;

            if (CharacterType != CharacterType.INVALID)
            {
                int characterId = -1;
                if (StoryCharacterId != StoryCharacterId.INVALID)
                {
                    characterId = (int)StoryCharacterId;
                }
                else if (ScenarioId != ScenarioId.INVALID)
                {
                    characterId = CharacterUtil.ScenarioIdToDBId(ScenarioId, ScenarioCharacterOffset);
                }
                else if (CreateCharacterId != CharacterConstants.INVALID_ID)
                {
                    characterId = CharacterUtil.CreateIdToDBId(CreateCharacterId);
                }

                if (characterId != -1)
                {
                    appearace = GameSystems.CharacterService.Get().GetCharacter(characterId).GetAppearance();
                }
            }
            else if (NPCId != NPCPropId.None)
            {
                appearace = LevelManagementService.Get().GetNPCAppearance(NPCId);
            }
            else if (RootCharacterId != -1)
            {
                appearace = GameSystems.CharacterService.Get().GetCharacter(RootCharacterId).GetAppearance();
            }

            return appearace;
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
                    actorId = CharacterUtil.CreateIdToDBId(CreateCharacterId);
                }
            }
            else if (NPCId != NPCPropId.None)
            {
                actorId = (int)NPCId;
            }
            else if (RootCharacterId != -1)
            {
                actorId = RootCharacterId;
            }

            return actorId;
        }

        public virtual string GetActorName()
        {
            string actorName = "NONE";

            int actorId = GetActorId();

            if (CharacterType != CharacterType.INVALID)
            {
                actorName = CharacterService.Get().GetCharacter(actorId).Name;
            }
            else if (NPCId != NPCPropId.None)
            {
                actorName = LevelManagementService.Get().GetPropInfo(actorId).Name;
            }

            return actorName;
        }
    }
}
