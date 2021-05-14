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
    public class NPCPickerControl : MonoBehaviour  
    {
        [System.Serializable] class NPCEnumPicker : EditorEnumPicker<NPCPropId> { }
        [HideInInspector] [SerializeField] NPCEnumPicker mNPCEnumPicker = new NPCEnumPicker();
        public IEditorEnumPicker NPCPicker { get { return mNPCEnumPicker; } }

        private void Start()
        {
            if (mNPCEnumPicker.PickedOption != "")
            {
                NPCPropId nPCPropId = mNPCEnumPicker.Val;
                GetComponent<NPCProp>().NPCId = nPCPropId;
            }
            else
            {
                Debug.LogWarningFormat("[{0}] NPCId not set!", gameObject.name);
            }
        }
    }
}
