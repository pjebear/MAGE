using MAGE.Utility.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    public class TrackPickerControl : MonoBehaviour
    {
        [System.Serializable] class TrackIdEnumPicker : EditorEnumPicker<TrackId> { }
        [HideInInspector] [SerializeField] TrackIdEnumPicker mTrackIdPicker = new TrackIdEnumPicker();
        public IEditorEnumPicker TrackIdPicker { get{ return mTrackIdPicker; } }
    }
}
