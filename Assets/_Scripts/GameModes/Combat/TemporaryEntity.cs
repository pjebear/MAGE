using MAGE.GameModes.SceneElements;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Combat
{
    class TemporaryEntity : CombatEntity
    {
        public int Duration = -1;

        private void Start()
        {
            if (GetComponent<SummonHeirarchy>().Owner)
            {
                TeamSide = GetComponent<SummonHeirarchy>().Owner.GetComponent<CombatEntity>().TeamSide;
            }

            GetComponentInParent<EncounterContainer>().EncounterModel.AddTemporaryEntity(this);
        }

        public override void OnTurnTick()
        {
            if (Duration > 0)
            {
                --Duration;

                if (Duration == 0)
                {
                    OnDeath();
                }
            }
        }

        public override void OnDeath()
        {
            GetComponentInParent<EncounterContainer>().EncounterModel.RemoveTemporaryEntity(this);
            Destroy(gameObject);
        }
    }
}
