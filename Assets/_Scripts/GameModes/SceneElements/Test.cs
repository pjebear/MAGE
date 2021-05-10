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
    public enum TestEnum
    {
        One,
        OneFive,
        Two,
        Three
    }

    public class Test : MonoBehaviour
    {
        [System.Serializable] public class TestEnumPicker : EditorEnumPicker<TestEnum> { };
        [HideInInspector][SerializeField] public TestEnumPicker TestPicker = new TestEnumPicker();
    }
}
