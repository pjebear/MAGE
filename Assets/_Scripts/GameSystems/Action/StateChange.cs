
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MAGE.GameSystems.Actions
{
    enum StateChangeType
    {
        None = -1,

        ActionCost,
        ActionTarget,
        StatusEffect,

        NUM
    }

    class StateChange
    {
        public static StateChange Empty { get { return new StateChange(StateChangeType.None, 0, 0); } }
        public int healthChange = 0;
        public int resourceChange = 0;
        public List<StatusEffect> statusEffects = new List<StatusEffect>();
        public StateChangeType Type;

        public StateChange(StateChangeType type, StatusEffect statusEffect)
            : this(type, 0, 0, new List<StatusEffect>() { statusEffect })
        {
            // empty
        }

        public StateChange(StateChangeType type, List<StatusEffect> statusEffects)
            : this(type, 0, 0, statusEffects)
        {
            // empty
        }

        public StateChange(StateChangeType type, int healthChange, int resourceChange, List<StatusEffect> statusEffects = null)
        {
            this.Type = type;
            this.healthChange = healthChange;
            this.resourceChange = resourceChange;
            this.statusEffects = statusEffects == null ? new List<StatusEffect>() : statusEffects;
        }

        public void Add(StateChange change)
        {
            healthChange += change.healthChange;
            resourceChange += change.resourceChange;
            statusEffects.AddRange(change.statusEffects);
        }

        public bool IsBeneficial()
        {
            bool areStatusEffectsBeneficial = true;
            foreach (StatusEffect effect in statusEffects)
            {
                areStatusEffectsBeneficial &= effect.Beneficial;
            }

            return healthChange > 0
            || resourceChange > 0
            || areStatusEffectsBeneficial;
        }

        public StateChange Copy()
        {
            return new StateChange(Type, this.healthChange, this.resourceChange, new List<StatusEffect>(this.statusEffects));
        }
    }
}

