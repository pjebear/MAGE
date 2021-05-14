using UnityEngine;
using UnityEditor;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using MAGE.Utility.Enums;

namespace MAGE.GameModes.SceneElements.Editors
{
    [CustomEditor(typeof(NPCPickerControl))]
    public class NPCPickerEditor : AttributePickerEditorBase<NPCPickerControl>
    {
        PickerSelectionInfo SelectionInfo = new PickerSelectionInfo();

        protected override void SetupAttributePickers()
        {
            SetupEnumPicker(SelectionInfo, mTarget.NPCPicker);
        }

        protected override void DrawAttributePickers()
        {
            AddPickerToEditor("NPCId:", SelectionInfo, mTarget.NPCPicker);
        }
    }
}
