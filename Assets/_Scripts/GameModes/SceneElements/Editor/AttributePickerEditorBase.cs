using UnityEngine;
using UnityEditor;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using MAGE.Utility.Enums;

namespace MAGE.GameModes.SceneElements.Editors
{
    public abstract class AttributePickerEditorBase<T> : Editor where T : class
    {
        protected class PickerSelectionInfo
        {
            public string[] Options;
            public int selection = 0;
        }

        protected T mTarget;
       
        private void OnEnable()
        {
            mTarget = target as T;
            SetupAttributePickers();
        }

        protected abstract void SetupAttributePickers();
        protected abstract void DrawAttributePickers();
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawAttributePickers();
        }

        protected void SetupEnumPicker(PickerSelectionInfo selectionInfo, IEditorEnumPicker picker)
        {
            List<string> options = picker.PickerOptions;
            options.Insert(0, "< Not Selected >");
            if (picker.PickedOption != "")
            {
                int selection = options.FindIndex(x => x == picker.PickedOption);
                if (selection != -1)
                {
                    selectionInfo.selection = selection;
                }
                else
                {
                    Debug.LogErrorFormat("AttributePickerEditorBase - Failed to find {0} in Enum list. GameObject [{1}]", picker.PickedOption, (target as MonoBehaviour).gameObject.name);
                    selectionInfo.selection = 0;
                    picker.PickedOption = "";
                }
            }
            else
            {
                selectionInfo.selection = 0;
            }

            selectionInfo.Options = options.ToArray();
        }

        protected void AddPickerToEditor(string pickerName, PickerSelectionInfo selectionInfo, IEditorEnumPicker picker)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(pickerName);
            GUILayout.Label(selectionInfo.selection == 0 ? "< Not Selected >" : picker.PickedOption);
            int updatedSelection = EditorGUILayout.Popup(selectionInfo.selection, selectionInfo.Options, GUILayout.Width(20), GUILayout.Height(20));
            if (selectionInfo.selection != updatedSelection)
            {
                EditorUtility.SetDirty(target);
                selectionInfo.selection = updatedSelection;
                if (selectionInfo.selection == 0)
                {
                    picker.PickedOption = "";
                }
                else
                {
                    picker.PickedOption = selectionInfo.Options[selectionInfo.selection];
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}
