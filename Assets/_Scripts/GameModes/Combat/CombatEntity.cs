using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Combat
{
    abstract class CombatEntity : MonoBehaviour
    {
        public TeamSide TeamSide = TeamSide.AllyHuman;

        public abstract void OnTurnTick();
        public abstract void OnDeath();
    }
}
