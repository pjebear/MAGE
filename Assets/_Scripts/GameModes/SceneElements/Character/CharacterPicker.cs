using MAGE.GameSystems;
using MAGE.GameSystems.Appearances;
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

        public virtual int GetCharacterId()
        {
            int characterId = -1;

            if (CharacterType != CharacterType.INVALID)
            {
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
            }
            else if (NPCId != NPCPropId.None)
            {
                characterId = (int)NPCId;
            }
            else if (RootCharacterId != -1)
            {
                characterId = RootCharacterId;
            }

            return characterId;
        }

        public virtual string GetActorName()
        {
            string actorName = "NONE";

            int characterId = GetCharacterId();

            if (CharacterType != CharacterType.INVALID || RootCharacterId != -1)
            {
                actorName = CharacterService.Get().GetCharacter(characterId).Name;
            }
            else if (NPCId != NPCPropId.None)
            {
                actorName = LevelManagementService.Get().GetPropInfo(characterId).Name;
            }

            return actorName;
        }

        public void Reset()
        {
            RootCharacterId = CharacterConstants.INVALID_ID;

            CharacterType = CharacterType.INVALID;
            StoryCharacterId = StoryCharacterId.INVALID;
            ScenarioId = ScenarioId.INVALID;
            CreateCharacterId = CharacterConstants.INVALID_ID;

            NPCId = NPCPropId.None;
        }
    }
}
