using UnityEngine;
using UnityEditor;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using MAGE.Utility.Enums;

namespace MAGE.GameModes.SceneElements.Editors
{
    [CustomEditor(typeof(CharacterPickerControl))]
    public class CharacterPickerEditor : AttributePickerEditorBase<CharacterPickerControl>
    {
        PickerSelectionInfo CharacterTypeInfo = new PickerSelectionInfo();
        PickerSelectionInfo StoryCharacterInfo = new PickerSelectionInfo();
        PickerSelectionInfo NPCInfo = new PickerSelectionInfo();

        protected override void SetupAttributePickers()
        {
            SetupEnumPicker(CharacterTypeInfo, mTarget.CharacterTypePicker);
            SetupEnumPicker(StoryCharacterInfo, mTarget.StoryCharacterPicker);
            SetupEnumPicker(NPCInfo, mTarget.NPCPicker);
        }

        protected override void DrawAttributePickers()
        {
            AddPickerToEditor("CharacterType:", CharacterTypeInfo, mTarget.CharacterTypePicker);
            AddPickerToEditor("StoryCharacterType:", StoryCharacterInfo, mTarget.StoryCharacterPicker);
            AddPickerToEditor("NPCId:", NPCInfo, mTarget.NPCPicker);
        }
    }
}
