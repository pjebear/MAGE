using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using Common.StatusEnums;
using Common.StatusTypes;
using Common.AttributeTypes;
using Common.AttributeEnums;
using WorldSystem.Character;
namespace EncounterSystem.Character.Managers
{
    using Common.ActionTypes;
    class StatusManager
    {
        private ScreenTextManager rScreenTextManager;
        private CharacterManager rCharacterManager;
        private CharacterBase rCharacterBase;

        // List of all StatusEffects applied to character in the encounter. 
        private Dictionary<StatusEffectIndex, StatusEffect> mEncounterStatusEffectMap;

        public StatusManager()
        {
            mEncounterStatusEffectMap = new Dictionary<StatusEffectIndex, StatusEffect>();
        }

        public void Initialize( CharacterManager characterManager, CharacterBase characterbase, ScreenTextManager screenTextManager)
        {
            rCharacterManager = characterManager;
            rCharacterBase = characterbase;
            rScreenTextManager = screenTextManager;
        }

        // ------------------------ Public Methods--------------------------------------------

        // -----------------------------------------------------------------------------------
        public bool ProgressStatusClock()
        {
            List<StatusEffect> timedOut = DecrementStatusCounters(StatusEffectProgression.ClockBased);

            foreach (StatusEffect effect in timedOut)
            {
                if (effect.OnRemove != null)
                {
                    // do something
                }
                RemoveStatusEffect(effect.Index);
            }
            
            return timedOut.Count > 0;
        }

        // -----------------------------------------------------------------------------------
        public bool ApplyStatusEffect(StatusEffect toApply, int numStacks = 1)
        {
            rCharacterBase.ApplyStatusEffect(toApply, numStacks);
            rScreenTextManager.DisplayStatusChangedText(toApply.Index.ToString(), numStacks);
           
            return true; //TODO: chance it doesn't happen?
        }

        // -----------------------------------------------------------------------------------
        public bool RemoveStatusEffect(StatusEffectIndex toRemove, int numStacks = -1)
        {
            bool hasStacksRemaining = !rCharacterBase.RemoveStatusEffect(toRemove, numStacks); // returns true is status was completely removed
            string screenText = toRemove.ToString();
            if (hasStacksRemaining)
            {
                screenText += " -1"; 
            }
            rScreenTextManager.DisplayStatusChangedText(toRemove.ToString(), -1);
            return true; //TODO: chance it doesn't happen?
        }

        // -----------------------------------------------------------------------------------
        public bool DispelStatusEffects(bool dispelBeneficial, bool dispelHarmful)
        {
            List<StatusEffectIndex> toRemove = new List<StatusEffectIndex>();
            foreach (var pair in rCharacterBase.GetStatusEffects())
            {
                if (pair.Value.IsDispelable && 
                    (pair.Value.Beneficial && dispelBeneficial) || (!pair.Value.Beneficial && dispelHarmful))
                {
                    toRemove.Add(pair.Key);
                }
            }
           
            foreach (StatusEffectIndex effectIndex in toRemove)
            {
                RemoveStatusEffect(effectIndex);
            }

            return toRemove.Count() > 0;
        }

        // -----------------------------------------------------------------------------------
        public bool DispelStatusEffectOfType(StatusType statusType)
        {

            List<StatusEffectIndex> toRemove = new List<StatusEffectIndex>();
            foreach (var pair in rCharacterBase.GetStatusEffects())
            {
                if (pair.Value.IsDispelable && pair.Value.PersistentEffect != null)
                {
                    AttributeModifier effect = pair.Value.PersistentEffect(pair.Value.StackCount);
                    if (effect.AttributeToModify.Type == AttributeType.Status && effect.AttributeToModify.Index == (int)statusType)
                    {
                        toRemove.Add(pair.Key);
                    }
                }
            }

            foreach (StatusEffectIndex effectIndex in toRemove)
            {
                RemoveStatusEffect(effectIndex);
            }

            return toRemove.Count() > 0;
        }

        // -----------------------------------------------------------------------------------
        public void OnTurnStart()
        {
            List<StatusEffect> timedOut = DecrementStatusCounters(StatusEffectProgression.TurnBased);

            foreach (StatusEffect effect in timedOut)
            {
                if (effect.OnRemove != null)
                {
                    // do something
                }

                RemoveStatusEffect(effect.Index);
            }

            // create a seperate list in case one kills the character and list is wiped
            List<AttributeModifier> onTurnEndModifiers = new List<AttributeModifier>();
            onTurnEndModifiers.AddRange(rCharacterBase.GetStatusPhaseEffects());

            foreach (AttributeModifier modifier in onTurnEndModifiers)
            {
                Debug.Assert(modifier.AttributeToModify.Type == AttributeType.Resource, "Per turn status effect not modifying Resource");
                ResourceChange toChange = new ResourceChange();
                toChange.Value = modifier.GetModifierValue(rCharacterBase.Attributes);
                
                switch (modifier.AttributeToModify.Index)
                {
                    case ((int)Resource.Health):
                        toChange.Resource = Resource.Health;
                        break;
                    case ((int)Resource.Mana):
                        toChange.Resource = Resource.Mana;
                        break;
                    case ((int)Resource.Endurance):
                        toChange.Resource = Resource.Endurance;
                        break;
                    default:
                        Debug.LogError("Attempting to modify non resource attribute on turn end");
                        break;
                }
                rScreenTextManager.DisplayResourceChangedText(toChange);
                rCharacterManager.ModifyCurrentResource(toChange);
            }
        }

        // ------------------------ Private Methods--------------------------------------------
        // -----------------------------------------------------------------------------------
        private List<StatusEffect> DecrementStatusCounters(StatusEffectProgression progression)
        {
            List<StatusEffect> timedOutEffects = new List<StatusEffect>();
            Dictionary<StatusEffectIndex, StatusEffect> statusEffectMap = rCharacterBase.GetStatusEffects();
            foreach (StatusEffect effect in statusEffectMap.Values)
            {
                if (effect.EffectProgression == progression && effect.TicksRemaining != 0)
                {
                    effect.TicksRemaining--;
                    if (effect.TicksRemaining == 0)
                    {
                        timedOutEffects.Add(effect);
                    }
                }
            };
            return timedOutEffects;
        }

        // DONT MODIFY THE STATUS EFFECT
        public StatusEffect QueryStatusEffect(StatusEffectIndex toFind)
        {
            if (rCharacterBase.GetStatusEffects().ContainsKey(toFind))
            {
                return rCharacterBase.GetStatusEffects()[toFind];
            }
            return null;
        }
    }
}