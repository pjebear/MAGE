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
        [SerializeField]
        private List<AuraType> DefaultAuras = new List<AuraType>();
        public List<Aura> Auras = new List<Aura>();

        private void Start()
        {
            foreach (AuraType auraType in DefaultAuras)
            {
                ApplyAura(auraType);
            }
        }

        public void ApplyAura(AuraType auraType)
        {
            AuraInfo info = AuraFactory.CheckoutAuraInfo(auraType);
            
            Aura aura = Instantiate(EncounterPrefabLoader.LoadAuraPrefab(), transform);

            ControllableEntity controllableEntity = GetComponent<ControllableEntity>();
            if (controllableEntity == null)
            {
                controllableEntity = GetComponent<SummonHeirarchy>().Owner?.GetComponent<ControllableEntity>();
            }

            Debug.Assert(controllableEntity != null);
            if (controllableEntity != null)
            {
                aura.Initialize(info, controllableEntity);
            }
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
