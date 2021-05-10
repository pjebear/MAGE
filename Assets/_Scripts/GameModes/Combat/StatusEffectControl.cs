using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MAGE.GameSystems.Stats;

namespace MAGE.GameModes.Combat
{
    class StatusEffectControl : MonoBehaviour
    {
        public Dictionary<StatusEffectId, StatusEffect> mStatusEffectLookup = new Dictionary<StatusEffectId, StatusEffect>();
        public List<StatusEffect> StatusEffects = new List<StatusEffect>();

        public void ApplyStatusEffects(List<StatusEffect> statusEffects, bool display = true)
        {
            StatusEffects.AddRange(statusEffects);
            List<AttributeModifier> attributeModifiers = new List<AttributeModifier>();
            foreach (StatusEffect effect in statusEffects)
            {
                attributeModifiers.AddRange(effect.GetAttributeModifiers());
            }
            GetComponent<StatsControl>().ApplyAttributeModifiers(attributeModifiers);

            if (display)
            {
                foreach (StatusEffect effect in statusEffects)
                {
                    Billboard.Params param = new Billboard.Params();
                    param.anchor = transform;
                    param.offset = new Vector3(1f, 2f, 0);
                    param.text = "+" + effect.EffectType.ToString();
                    GetComponent<BillboardEmitter>().Emitt(param, 2f);
                }
            }   
        }

        public void RemoveStatusEffects(List<StatusEffect> statusEffects, bool display = true)
        {
            List<AttributeModifier> removedModifiers = new List<AttributeModifier>();

            foreach (StatusEffect effect in statusEffects)
            {
                removedModifiers.AddRange(effect.GetAttributeModifiers());
                StatusEffects.Remove(effect);
                if (display)
                {
                    Billboard.Params param = new Billboard.Params();
                    param.anchor = transform;
                    param.offset = new Vector3(1f, 2f, 0);
                    param.text = "-" + effect.EffectType.ToString();
                    GetComponent<BillboardEmitter>().Emitt(param, 2f);
                }
            }

            GetComponent<StatsControl>().RemoveAttributeModifiers(removedModifiers);
        }

        public List<StatusEffect> GetSingleTurnStatusEffects()
        {
            List<StatusEffect> statusEffects = new List<StatusEffect>();
            foreach (StatusEffect effect in StatusEffects)
            {
                if (effect.MaxDuration == StatusEffectConstants.UNTIL_NEXT_TURN)
                {
                    statusEffects.Add(effect);
                }
            }
            return statusEffects;
        }

        public List<StateChange> GetTurnStartStateChanges()
        {
            List<StateChange> stateChanges = new List<StateChange>();
            foreach (StatusEffect effect in StatusEffects)
            {
                stateChanges.Add(effect.GetTurnStartStateChange());
            }
            return stateChanges;
        }

        //  ------------------------------------------------------------------------------
        public int GetStackCountForStatus(StatusEffectId statusEffectId, int statusCreator)
        {
            int stackCount = 0;

            Optional<StatusEffect> optEffect = GetStatusEffect(statusEffectId, statusCreator);
            if (optEffect.HasValue)
            {
                stackCount = optEffect.Value.StackCount;
            }

            return stackCount;
        }

        //  ------------------------------------------------------------------------------
        public Optional<StatusEffect> GetStatusEffect(StatusEffectId statusEffectId, int ownedBy)
        {
            Optional<StatusEffect> optEffect = new Optional<StatusEffect>();

            StatusEffect statusEffect = StatusEffects.Find(
                x =>
                x.EffectType == statusEffectId
                /*&& ownedBy == x.CreatedBy.Id*/);

            if (statusEffect != null)
            {
                optEffect = statusEffect;
            }

            return optEffect;
        }

        public void TickStatusEffects()
        {
            List<StatusEffect> expiredEffects = new List<StatusEffect>();
            foreach (StatusEffect effect in StatusEffects)
            {
                effect.ProgressDuration();
                if (effect.HasExpired())
                {
                    expiredEffects.Add(effect);
                }
            }

            RemoveStatusEffects(expiredEffects);
        }

        public void OnDeath()
        {
            RemoveStatusEffects(new List<StatusEffect>(StatusEffects));
        }
    }
}
