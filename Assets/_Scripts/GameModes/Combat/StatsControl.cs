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
        public Attributes Attributes = new Attributes(GameSystems.Stats.Attributes.Empty);

        public void SetAttributes(Attributes attributes)
        {
            Attributes = attributes;

            NotifyAttributesUpdated();
        }

        public void ApplyAttributeModifiers(List<AttributeModifier> modifiers)
        {
            foreach (AttributeModifier modifier in modifiers)
            {
                Attributes.Modify(modifier);
            }

            NotifyAttributesUpdated();
        }
        public void RemoveAttributeModifiers(List<AttributeModifier> modifiers)
        {
            foreach (AttributeModifier modifier in modifiers)
            {
                Attributes.Revert(modifier);
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
