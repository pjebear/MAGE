using MAGE.GameSystems;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements.Encounters
{
    class ExistingCharacterParams : MonoBehaviour
    {
        public CharacterPicker CharacterPicker;
        public int LevelOverride = -1;
    }
}
