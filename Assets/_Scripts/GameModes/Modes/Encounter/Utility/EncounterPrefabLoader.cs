using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes.Encounter
{
    static class EncounterPrefabLoader
    {
        public static Aura LoadAuraPrefab()
        {
            return UnityEngine.Resources.Load<Aura>("EncounterPrefabs/Aura");
        }

        public static BillboardEmitter LoadBillBoardEmitterPrefab()
        {
            return UnityEngine.Resources.Load<BillboardEmitter>("EncounterPrefabs/BillboardEmmiter");
        }

        public static DebugAnimationRig LoadAnimationRig()
        {
            return UnityEngine.Resources.Load<DebugAnimationRig>("EncounterPrefabs/DebugAnimationRig");
        }
    }
}
