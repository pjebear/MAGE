using MAGE.GameModes.Combat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

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

        public static LineRenderer RangeRenderer { get { return GameObject.Instantiate(UnityEngine.Resources.Load<LineRenderer>("EncounterPrefabs/RangeRenderer")); } }

        public static NavMeshObstacle Obstacle { get { return GameObject.Instantiate(UnityEngine.Resources.Load<NavMeshObstacle>("EncounterPrefabs/NavMeshObstacle")); } }
    }
}
