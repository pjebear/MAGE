using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MAGE.GameSystems.Actions;
using MAGE.GameModes.Encounter;
using MAGE.GameSystems.Stats;

namespace MAGE.GameModes.Combat
{
    class AurasControl : MonoBehaviour
    {
        public List<Aura> Auras = new List<Aura>();

        public void ApplyAura(AuraType auraType)
        {
            AuraInfo info = AuraFactory.CheckoutAuraInfo(auraType);
            
            Aura aura = Instantiate(EncounterPrefabLoader.LoadAuraPrefab(), transform);
            aura.Initialize(info, GetComponent<CombatEntity>());
        }

        public void OnDeath()
        {
            foreach (Aura aura in Auras)
            {
                aura.SetActive(false);
            }
        }
    }
}
