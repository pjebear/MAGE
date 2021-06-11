using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Combat
{
    class SummonHeirarchy : MonoBehaviour
    {
        public SummonHeirarchy Owner = null;
        public List<SummonHeirarchy> Summons = new List<SummonHeirarchy>();

        private void OnDestroy()
        {
            Owner?.Summons.Remove(this);

            foreach (SummonHeirarchy summonHeirarchy in Summons)
            {
                summonHeirarchy.Owner = Owner;
            }
        }
    }
}
