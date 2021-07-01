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

        public void ApplyStatusEffects(List<StatusEffect> statusEffects, bool display = true)
        {
            if (statusEffects.Count == 0) return;
            
            foreach (StatusEffect effect in statusEffects)
            {
                if (!mStatusEffectLookup.ContainsKey(effect.EffectType))
                {
                    mStatusEffectLookup.Add(effect.EffectType, effect);
                }
                else
                {
                    mStatusEffectLookup[effect.EffectType].StackEffect(effect.StackCount);
                    mStatusEffectLookup[effect.EffectType].ResetDuration();
                }
            }

            BroadcastMessage("OnStatusEffectsChanged", SendMessageOptions.DontRequireReceiver);

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
            if (statusEffects.Count == 0) return;

            foreach (StatusEffect effect in statusEffects)
            {
                if (mStatusEffectLookup.ContainsKey(effect.EffectType))
                {
                    StatusEffect appliedEffect = mStatusEffectLookup[effect.EffectType];
                    if (appliedEffect.UnStackEffect(effect.StackCount))
                    {
                        mStatusEffectLookup.Remove(effect.EffectType);
                    }
                }
            }

            BroadcastMessage("OnStatusEffectsChanged", SendMessageOptions.DontRequireReceiver);

            foreach (StatusEffect effect in statusEffects)
            {
                if (display)
                {
                    Billboard.Params param = new Billboard.Params();
                    param.anchor = transform;
                    param.offset = new Vector3(1f, 2f, 0);
                    param.text = "-" + effect.EffectType.ToString();
                    GetComponent<BillboardEmitter>().Emitt(param, 2f);
                }
            }
        }

        //  ------------------------------------------------------------------------------
        public List<StatusEffect> GetSingleTurnStatusEffects()
        {
            List<StatusEffect> statusEffects = new List<StatusEffect>();
            foreach (StatusEffect effect in mStatusEffectLookup.Values)
            {
                if (effect.MaxDuration == StatusEffectConstants.UNTIL_NEXT_TURN)
                {
                    statusEffects.Add(effect);
                }
            }
            return statusEffects;
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

            if (mStatusEffectLookup.ContainsKey(statusEffectId))
            {
                optEffect = mStatusEffectLookup[statusEffectId];

            }

            return optEffect;
        }

        public void TickStatusEffects()
        {
            List<StatusEffect> expiredEffects = new List<StatusEffect>();
            foreach (StatusEffect effect in mStatusEffectLookup.Values)
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
            RemoveStatusEffects(new List<StatusEffect>(mStatusEffectLookup.Values));
        }
    }
}
