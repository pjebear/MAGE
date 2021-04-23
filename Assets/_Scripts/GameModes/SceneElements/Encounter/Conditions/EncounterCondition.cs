using MAGE.GameModes.Combat;
using MAGE.GameModes.Encounter;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Stats;
using MAGE.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements.Encounters
{
    abstract class EncounterCondition : MonoBehaviour
    {
        public abstract bool IsConditionMet(EncounterModel model);
    }
}
    
