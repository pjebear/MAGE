using UnityEngine;
using UnityEditor;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using MAGE.Utility.Enums;

namespace MAGE.GameModes.SceneElements.Editors
{
    [CustomEditor(typeof(TrackPickerControl))]
    public class AudioTriggerVolumeEditor : AttributePickerEditorBase<TrackPickerControl>
    {
        PickerSelectionInfo AudioSelectionInfo = new PickerSelectionInfo();

        protected override void SetupAttributePickers()
        {
            SetupEnumPicker(AudioSelectionInfo, mTarget.TrackIdPicker);
        }

        protected override void DrawAttributePickers()
        {
            AddPickerToEditor("TrackId:", AudioSelectionInfo, mTarget.TrackIdPicker);
        }
    }
}
