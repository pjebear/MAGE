using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Combat
{
    [RequireComponent(typeof(CombatDisplayable))]
    [RequireComponent(typeof(ResourcesControl))]
    class CombatTarget : MonoBehaviour
    {
        public void ApplyStateChange(StateChange stateChange)
        {
            // StatusEffects first
            if (GetComponent<StatusEffectControl>() != null)
            {
                GetComponent<StatusEffectControl>().ApplyStatusEffects(stateChange.statusEffects);
            }
            
            GetComponent<ResourcesControl>().ApplyStateChange(stateChange);
        }
    }
}
