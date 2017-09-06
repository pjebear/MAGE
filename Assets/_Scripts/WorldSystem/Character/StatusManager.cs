using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common.AttributeEnums;
using Common.AttributeTypes;
using Common.StatusEnums;
using Common.StatusTypes;
using Common.CommonUtil.AttributeUtil;
using Screens.Payloads;

namespace WorldSystem.Character
{
    public class StatusManager
    {
        private AttributeContainer rBaseAttributes;
        private AttributeContainer rAttribtues;
        public Dictionary<StatusEffectIndex, StatusEffect> StatusEffectMap { get; private set; }
        private List<StatusEffectIndex> mTemporaryStatusEffecs; // keep track of all status effects that will only last the encounter
        public Dictionary<StatusEffectIndex, AttributeModifier> PersistentEffectMap { get; private set; }
        public Dictionary<StatusEffectIndex, AttributeModifier> StatusPhaseEffectMap { get; private set; }

        private bool mStatsChanged;
        private bool mStatusChanged;

        public StatusManager()
        {
            mTemporaryStatusEffecs = new List<StatusEffectIndex>();
            StatusEffectMap = new Dictionary<StatusEffectIndex, StatusEffect>();
            PersistentEffectMap = new Dictionary<StatusEffectIndex, AttributeModifier>();
            StatusPhaseEffectMap = new Dictionary<StatusEffectIndex, AttributeModifier>();

            mStatsChanged = false;
            mStatusChanged = false;
        }

        public void Initialize(AttributeContainer baseAttributes, AttributeContainer attributes)
        {
            rBaseAttributes = baseAttributes;
            rAttribtues = attributes;
        }

        public void SetToDefaultStatusEffects()
        {
            // Index is removed from TemporaryEffects in RemoveStatusEffect so cant enumerate through list
            while (mTemporaryStatusEffecs.Count > 0)
            {
                RemoveStatusEffect(mTemporaryStatusEffecs[mTemporaryStatusEffecs.Count - 1]);
            }
        }

        public void ApplyStatusEffect(StatusEffect toApply, int numStacks = 1, bool isTemporary = true)
        {
            if (!isTemporary)
            {
                UnityEngine.Debug.Assert(toApply.TicksRemaining == -1 && toApply.IsDispelable == false);
            }
           
            if (toApply.CanStack)
            {
                UnityEngine.Debug.Assert(numStacks >= 1, String.Format("Attempting to apply {0} stacks to status {1} that is stackable", numStacks, toApply.Index.ToString()));
            }
            else
            {
                UnityEngine.Debug.Assert(numStacks == 1, String.Format("Attempting to apply {0} stacks to status {1} that isn't stackable", numStacks, toApply.Index.ToString()));
            }
            
            if (StatusEffectMap.ContainsKey(toApply.Index))
            {
                if (toApply.CanStack)
                {
                    StatusEffectMap[toApply.Index].StackCount += numStacks;
                    if (toApply.PersistentEffect != null)
                    {
                        //update persistent effect map with new stack strength
                        PersistentEffectMap[toApply.Index] = toApply.PersistentEffect(toApply.StackCount);
                    }
                    if (toApply.StatusPhaseEffect != null)
                    {
                        StatusPhaseEffectMap[toApply.Index] = toApply.StatusPhaseEffect(toApply.StackCount);
                    }
                }
                // reset counter TODO: Do we want this to always happen
                StatusEffectMap[toApply.Index].TicksRemaining = toApply.TicksRemaining; 
            }
            else
            {
                toApply.StackCount = numStacks - 1; // first stack = 0
                StatusEffectMap.Add(toApply.Index, toApply);
                if (isTemporary) mTemporaryStatusEffecs.Add(toApply.Index);

                if (toApply.PersistentEffect != null)
                {
                    PersistentEffectMap.Add(toApply.Index, toApply.PersistentEffect(toApply.StackCount));
                }
                if (toApply.StatusPhaseEffect != null)
                {
                    StatusPhaseEffectMap.Add(toApply.Index, toApply.StatusPhaseEffect(toApply.StackCount));
                }
            }
            SetDirtyFlagsOnAddRemove(toApply);
            RebuildAttributes();
        }

        public bool RemoveStatusEffect(StatusEffectIndex toRemove, int numStacks = -1 /* -1 will remove all stacks*/)
        {
            if (StatusEffectMap.ContainsKey(toRemove))
            {
                bool needsCompleteRemoval = false;
                StatusEffect effect = StatusEffectMap[toRemove];
                
                if (numStacks == -1 || !effect.CanStack)
                {
                    needsCompleteRemoval = true;
                }
                else
                {
                    UnityEngine.Debug.Assert(numStacks >= 1, String.Format("Attempting to remove invalid number of stacks from status {1}. Got {0}", numStacks, toRemove.ToString()));
                    effect.StackCount -= numStacks;

                    if (effect.StackCount < 0) // 0 based stack count
                    {
                        needsCompleteRemoval = true;
                    } 
                    else if (effect.PersistentEffect != null) // doesn't need to be removed, update effect
                    {
                        PersistentEffectMap[effect.Index] = effect.PersistentEffect(effect.StackCount);
                    }
                }
                

                if (needsCompleteRemoval)
                {
                    StatusEffectMap.Remove(toRemove);
                    if (effect.PersistentEffect != null)
                    {
                        PersistentEffectMap.Remove(toRemove);
                    }
                    if (effect.StatusPhaseEffect != null)
                    {
                        StatusPhaseEffectMap.Remove(toRemove);
                    }
                    mTemporaryStatusEffecs.Remove(toRemove);
                }
               
                SetDirtyFlagsOnAddRemove(effect);
                RebuildAttributes();

                return needsCompleteRemoval;
            }
            return false;
        }

        public List<StatusEffectIconPayload> GetStatusEffectPayloads()
        {
            List<StatusEffectIconPayload> payloads = new List<StatusEffectIconPayload>();

            foreach (StatusEffect effect in StatusEffectMap.Values)
            {
                if (!effect.IsHidden)
                {
                    StatusEffectIconPayload iconPayload = new StatusEffectIconPayload((int)effect.Index, effect.ToolTip, effect.StackCount, effect.Beneficial);
                    payloads.Add(iconPayload);
                }
            }
            return payloads;
        }

        public void RebuildAttributes(bool forceStatRebuild = false, bool forceStatusRebuild = false)
        {
            if (mStatsChanged || forceStatRebuild)
            {
                SetToDefaultAttributes(AttributeType.Stat);
                SetToDefaultAttributes(AttributeType.Resource);
                ReapplyPersistentEffects(AttributeType.Stat);
                //set base resources based on new stats
                AttributeUtil.CalculateResourcesFromStats(rBaseAttributes[AttributeType.Stat], rAttribtues[AttributeType.Resource]);
                ReapplyPersistentEffects(AttributeType.Resource);
                mStatsChanged = false;
            }
            if (mStatusChanged || forceStatusRebuild)
            {
                SetToDefaultAttributes(AttributeType.Status);
                ReapplyPersistentEffects(AttributeType.Status);
                mStatusChanged = false;
            }
        }

        // -----------------------------------------------------------------------------------
        private void ReapplyPersistentEffects(AttributeType type)
        {
            List<AttributeModifier> modifiersOfType = PersistentEffectMap.Where(pair =>
               pair.Value.AttributeToModify.Type == type).Select(pair => pair.Value).ToList();

            switch (type)
            {
                case (AttributeType.Stat):
                    modifiersOfType.Sort(delegate (AttributeModifier x, AttributeModifier y)
                    {
                        return x.ModifierType.CompareTo(y.ModifierType);
                    });

                    foreach (AttributeModifier modifier in modifiersOfType)
                    {
                        switch (modifier.ModifierType)
                        {
                            case (ModifierType.Additive):
                                rAttribtues[modifier.AttributeToModify] += modifier.GetModifierValue(rAttribtues);
                                break;
                            case (ModifierType.Multiplicative):
                                rAttribtues[modifier.AttributeToModify] *= modifier.GetModifierValue(rAttribtues);
                                break;
                        }
                    }
                    break;

                case (AttributeType.Resource): // can only be multiplicative if a resource
                    foreach (AttributeModifier modifier in modifiersOfType)
                    {
                        UnityEngine.Debug.Assert(modifier.ModifierType == ModifierType.Multiplicative, "Found Additive Modifier while rebuilding Resource Modifiers modifiers");

                        rAttribtues[modifier.AttributeToModify] *= modifier.GetModifierValue(rAttribtues);
                        rAttribtues[modifier.AttributeToModify.Type][modifier.AttributeToModify.Index - 3] *= modifier.GetModifierValue(rAttribtues);
                    }
                    break;

                case (AttributeType.Status): // can only be additive if a status
                    foreach (AttributeModifier modifier in modifiersOfType)
                    {
                        UnityEngine.Debug.Assert(modifier.ModifierType == ModifierType.Additive, "Found multiplicative Modifier while rebuilding Status modifiers");
                        rAttribtues[modifier.AttributeToModify] += modifier.GetModifierValue(rAttribtues);
                    }
                    break;
            }
        }

        // -----------------------------------------------------------------------------------
        private void SetToDefaultAttributes(AttributeType toDefault)
        {
            // Stats
            switch (toDefault)
            {
                case (AttributeType.Stat):
                    for (int statIndex = 0; statIndex < (int)CharacterStats.NUM; statIndex++)
                    {
                        rAttribtues[AttributeType.Stat][statIndex] = rBaseAttributes[AttributeType.Stat][statIndex];
                    }
                    break;

                case (AttributeType.Resource):
                    // Maintain current resource percentages
                    for (int resourceIndex = 0; resourceIndex < 3 /* just parse base resources*/; resourceIndex++)
                    {
                        rAttribtues[AttributeType.Resource][resourceIndex] = rBaseAttributes[AttributeType.Resource][resourceIndex];
                        rAttribtues[AttributeType.Resource][resourceIndex + 3] = rBaseAttributes[AttributeType.Resource][resourceIndex + 3];
                    }
                    break;

                case (AttributeType.Status):
                    for (int statusIndex = 0; statusIndex < (int)StatusType.NUM; statusIndex++)
                    {
                        rAttribtues[AttributeType.Stat][statusIndex] = rBaseAttributes[AttributeType.Stat][statusIndex];
                        UnityEngine.Debug.Assert(rBaseAttributes[AttributeType.Stat][statusIndex] == 0f, "Base Status Not zero");
                    }
                    break;
                default:
                    UnityEngine.Debug.LogErrorFormat("Unknown Attribute Type Category attempting to reset in ResetToDefaults StatusManager {0}", toDefault.ToString());
                    break;
            }
        }

        // -----------------------------------------------------------------------------------
        private void SetDirtyFlagsOnAddRemove(StatusEffect status)
        {
            if (status.PersistentEffect != null)
            {
                AttributeModifier effect = status.PersistentEffect(status.StackCount);
                mStatusChanged |= effect.AttributeToModify.Type == AttributeType.Status;
                mStatsChanged |= effect.AttributeToModify.Type == AttributeType.Stat || effect.AttributeToModify.Type == AttributeType.Resource;
            }
        }
    }
}
