using UnityEngine;
using UnityEditor;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using MAGE.Utility.Enums;

namespace MAGE.GameModes.SceneElements.Editors
{
    [CustomEditor(typeof(StoryCharacterPickerControl))]
    public class StoryCharacterPickerEditor : AttributePickerEditorBase<StoryCharacterPickerControl>
    {
        PickerSelectionInfo StoryCharacterInfo = new PickerSelectionInfo();

        protected override void SetupAttributePickers()
        {
            SetupEnumPicker(StoryCharacterInfo, mTarget.StoryCharacterPicker);
        }

        protected override void DrawAttributePickers()
        {
            AddPickerToEditor("StoryCharacterType:", StoryCharacterInfo, mTarget.StoryCharacterPicker);
        }
    }
}
