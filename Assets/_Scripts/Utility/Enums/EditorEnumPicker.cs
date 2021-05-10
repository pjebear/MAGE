using MAGE.Utility.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.Utility.Enums
{
    public interface IEditorValuePicker
    {
        string PickedOption { get; set; }
    }

    [System.Serializable] 
    public class StringPicker : IEditorValuePicker
    {
        public string PickedOption { get { return _PickedOption; } set { _PickedOption = value; } }
        [SerializeField]
        private string _PickedOption = "";
    }

    public interface IEditorEnumPicker
    {
        List<string> PickerOptions { get; }
        string PickedOption { get; set; }
    }

    [System.Serializable]
    public abstract class EditorEnumPicker<T> : IEditorEnumPicker where T : System.Enum
    {
        public List<string> PickerOptions { get { return new List<string>(Enum.GetNames(typeof(T))); } }
        public string PickedOption { get { return _PickedOptionStr; } set { _PickedOptionStr = value; } }
        public T Val { get { return EnumUtil.StringToEnum<T>(PickedOption); } }

        [SerializeField]
        private string _PickedOptionStr = "";
    }
}
