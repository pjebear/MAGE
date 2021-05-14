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
    [RequireComponent(typeof(CharacterPickerControl))]
    public class StoryCharacterPickerControl : MonoBehaviour
    {
        [System.Serializable] class StoryCharacterEnumPicker : EditorEnumPicker<StoryCharacterId> { }
        [HideInInspector] [SerializeField] StoryCharacterEnumPicker mStoryCharacterPicker = new StoryCharacterEnumPicker();
        public IEditorEnumPicker StoryCharacterPicker { get { return mStoryCharacterPicker; } }

        private void Start()
        {
            if (mStoryCharacterPicker.PickedOption != "")
            {
                GetComponent<CharacterPickerControl>().CharacterId = (int)mStoryCharacterPicker.Val;
            }
            else
            {
                Debug.LogWarningFormat("[{0}] StoryCharacterId not set!", gameObject.name);
            }
        }
    }
}
