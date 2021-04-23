using MAGE.GameSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.DebugFlow
{
    class DebugSettings : MonoBehaviour
    {
        public bool SkipMainMenu = false;
        public bool MuteMusic = false;
        public LevelId OverrideLevelId = LevelId.INVALID;
    }
}
