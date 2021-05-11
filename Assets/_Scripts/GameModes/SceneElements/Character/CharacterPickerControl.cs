using MAGE.GameSystems;
using MAGE.GameSystems.Appearances;
using MAGE.GameSystems.Characters;
using MAGE.Utility.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    public class CharacterPickerControl : MonoBehaviour
    {
        [SerializeField]
        private int CharacterIdOffset = CharacterConstants.INVALID_ID;

        [System.Serializable] class CharacterTypeEnumPicker : EditorEnumPicker<CharacterType> { }
        [HideInInspector] [SerializeField] CharacterTypeEnumPicker mCharacterTypePicker = new CharacterTypeEnumPicker();
        public IEditorEnumPicker CharacterTypePicker { get { return mCharacterTypePicker; } }

        [System.Serializable] class StoryCharacterEnumPicker : EditorEnumPicker<StoryCharacterId> { }
        [HideInInspector] [SerializeField] StoryCharacterEnumPicker mStoryCharacterPicker = new StoryCharacterEnumPicker();
        public IEditorEnumPicker StoryCharacterPicker { get { return mStoryCharacterPicker; } }

        [System.Serializable] class NPCEnumPicker : EditorEnumPicker<NPCPropId> { }
        [HideInInspector] [SerializeField] NPCEnumPicker mNPCPicker = new NPCEnumPicker();
        public IEditorEnumPicker NPCPicker { get { return mNPCPicker; } }

        public virtual int GetCharacterId()
        {
            int characterId = -1;

            if (mCharacterTypePicker.PickedOption != "")
            {
                if (mStoryCharacterPicker.PickedOption != "")
                {
                    characterId = (int)mStoryCharacterPicker.Val;
                }
                else if (CharacterIdOffset != CharacterConstants.INVALID_ID)
                {
                    characterId = CharacterUtil.CreateIdToDBId(CharacterIdOffset);
                }
            }
            else if (mNPCPicker.PickedOption != "")
            {
                characterId = (int)mNPCPicker.Val;
            }
            else if (CharacterIdOffset != -1)
            {
                characterId = CharacterIdOffset;
            }

            return characterId;
        }

        public void Reset()
        {
            CharacterIdOffset = CharacterConstants.INVALID_ID;

            mCharacterTypePicker.PickedOption = "";
            mStoryCharacterPicker.PickedOption = "";

            mNPCPicker.PickedOption = "";
        }

        public void Set(NPCPropId id)
        {
            Debug.Assert(id != NPCPropId.None);
            Reset();
            mNPCPicker.PickedOption = id.ToString();
        }

        public void Set(StoryCharacterId id)
        {
            Debug.Assert(id != StoryCharacterId.INVALID);
            Reset();
            mCharacterTypePicker.PickedOption = CharacterType.Story.ToString();
            mStoryCharacterPicker.PickedOption = id.ToString();
        }

        public void SetRootCharacterId(int rootCharacterId)
        {
            Debug.Assert(rootCharacterId != -1);
            Reset();
            CharacterIdOffset = rootCharacterId;
        }
    }
}
