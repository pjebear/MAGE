using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MAGE.GameSystems.Stats;

namespace MAGE.GameModes.Combat
{
    class StatsControl : MonoBehaviour
    {
        private Attributes BaseAttributes = new Attributes(GameSystems.Stats.Attributes.Empty);
        public Attributes Attributes = new Attributes(GameSystems.Stats.Attributes.Empty);

        public void SetAttributes(Attributes attributes)
        {
            BaseAttributes = new Attributes(attributes);
            Attributes = new Attributes(attributes);
        }

        public void OnStatusEffectsChanged()
        {
            Attributes = new Attributes(BaseAttributes);

            foreach (StatusEffect statusEffect in GetComponent<StatusEffectControl>().mStatusEffectLookup.Values)
            {
                foreach (AttributeModifier attributeModifier in statusEffect.GetAttributeModifiers())
                {
                    Attributes.Modify(attributeModifier);
                }
            }

            NotifyAttributesUpdated();
        }

        private void NotifyAttributesUpdated()
        {
            // temp 
            ResourcesControl resources = GetComponent<ResourcesControl>();
            resources.OnStatsUpdated();
        }
    }
}
