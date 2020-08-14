
using MAGE.GameServices.Character;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MAGE.GameModes.Encounter
{
    using MAGE.GameServices;
    using MAGE.GameServices.Character;

    class EncounterCharacter
    {
        // Private
        private CharacterInfo mCharacter;
        private List<AttributeModifier> mAttributeModifiers;

        // Public
        public int Id;
        public TeamSide Team;
        public List<ActionResponderBase> Listeners;
        public Resources Resources;
        public Attributes Attributes { get; private set; }
        public List<StatusEffect> StatusEffects { get; private set; }
        public Equipment Equipment { get { return mCharacter.Equipment; } }
        public int AppearanceId { get { return mCharacter.AppearanceId; } }
        // Accessors
        public bool IsAlive { get { return Resources[ResourceType.Health].Ratio > 0; } }
        public bool DEBUG_IsTurnComplete = false;
        public bool DEBUG_HasActed = false;
        public bool DEBUG_HasMoved = false;
        public void DEBUG_Reset() { DEBUG_HasActed = false; DEBUG_HasMoved = false; }
        public int DEBUG_ClockGuage = 0;

        public string Name { get { return mCharacter.Name; } }
        public int Level { get { return mCharacter.Level; } }
        public int Exp { get { return mCharacter.Experience; } }
        public SpecializationType Specialization { get { return mCharacter.CurrentSpecializationType; } }
        public List<ActionId> Actions { get { return mCharacter.Actions; } }
        public List<AuraType> Auras { get { return mCharacter.Auras; } }

        public EncounterCharacter(TeamSide team, MAGE.GameServices.Character.CharacterInfo character)
        {
            Id = character.Id;
            Team = team;
            mCharacter = character;
            mAttributeModifiers = new List<AttributeModifier>();

            InitializeAttributes();
            StatusEffects = new List<StatusEffect>();

            Listeners = new List<ActionResponderBase>();
            foreach (ActionResponseId listenerType in character.Listeners)
            {
                ActionResponderBase listener = ActionResultListenerFactory.CheckoutListener(this, listenerType);
                Listeners.Add(listener);
            }
        }

        public void NotifyActionResults(ActionResult actionResult)
        {
            if (IsAlive)
            {
                foreach (ActionResponderBase listener in Listeners)
                {
                    listener.RespondToAction(actionResult);
                }
            }
        }

        public ActionInfo GetActionInfo(ActionId actionId)
        {
            ActionInfo actionInfo = ActionFactory.CreateActionInfoFromId(actionId, this);

            foreach (IActionModifier modifier in mCharacter.ActionModifiers.Where(x => x.ActionId == actionId))
            {
                modifier.Modify(actionInfo);
            }

            return actionInfo;
        }

        public ActionResponseInfo GetActionResponseInfo(ActionResponseId responseId)
        {
            ActionResponseInfo responseInfo = ActionFactory.CreateActionResponseInfoFromId(responseId);
            //if (character.ActionModifiers.ContainsKey(actionId))
            //{
            //    foreach (IActionModifier modifier in character.ActionModifiers[actionId])
            //    {
            //        modifier.Modify(actionInfo);
            //    }
            //}

            return responseInfo;
        }

        public AuraInfo GetAuraInfo(AuraType auraType)
        {
            AuraInfo info = AuraFactory.CheckoutAuraInfo(auraType);

            // todo: modify

            return info;
        }

        public void OnAuraEntered(StatusEffect auraEffect)
        {
            AddStatusEffect(auraEffect);

            UpdateAttribtues();
        }

        public void OnAuraExited(StatusEffect auraEffect)
        {
            RemoveStatusEffect(auraEffect);

            UpdateAttribtues();
        }

        public void ApplyStateChange(StateChange stateChange)
        {
            // status effects first 
            if (stateChange.statusEffects.Count > 0)
            {
                foreach (StatusEffect effect in stateChange.statusEffects)
                {
                    if (stateChange.Type == StateChangeType.ActionCost)
                    {
                        RemoveStatusEffect(effect);
                    }
                    else
                    {
                        AddStatusEffect(effect);
                    }
                }

                UpdateAttribtues();
            }

            Resources[ResourceType.Health].Modify(stateChange.healthChange);
            Resources[ResourceType.Mana].Modify(stateChange.resourceChange);
            Resources[ResourceType.Endurance].Modify(stateChange.resourceChange);
        }

        public int GetStackCountForStatus(StatusEffectType statusType, EncounterCharacter statusCreator)
        {
            int stackCount = 0;

            StatusEffect effect = StatusEffects.Find(x => x.EffectType == statusType && x.CreatedBy == statusCreator);
            if (effect != null)
            {
                stackCount = effect.StackCount;
            }

            return stackCount;
        }

        public List<StatusEffect> ProgressStatusEffects()
        {
            List<StatusEffect> expiredEffects = new List<StatusEffect>();

            foreach (StatusEffect effect in StatusEffects)
            {
                if (effect.ProgressDuration())
                {
                    expiredEffects.Add(effect);
                }
            }

            foreach (StatusEffect effect in expiredEffects)
            {
                StatusEffects.Remove(effect);
            }

            return expiredEffects;
        }

        public List<StateChange> GetTurnStartStateChanges()
        {
            List<StateChange> turnStateChanges = new List<StateChange>();

            foreach (StatusEffect effect in StatusEffects)
            {
                turnStateChanges.Add(effect.GetTurnStartStateChange());
            }

            return turnStateChanges;
        }

        public override string ToString()
        {
            return Name;
        }

        private void InitializeAttributes()
        {
            Attributes = new Attributes(mCharacter.Attributes);

            foreach (AttributeModifier attributeModifier in mAttributeModifiers)
            {
                Attributes.Modify(attributeModifier);
            }

            Resources = new Resources(
               AttributeUtil.ResourceFromAttribtues(ResourceType.Health, Attributes),
               AttributeUtil.ResourceFromAttribtues(ResourceType.Mana, Attributes),
               AttributeUtil.ResourceFromAttribtues(ResourceType.Endurance, Attributes));
        }

        private void UpdateAttribtues()
        {
            Attributes = new Attributes(mCharacter.Attributes);

            foreach (AttributeModifier attributeModifier in mAttributeModifiers)
            {
                Attributes.Modify(attributeModifier);
            }


            foreach (StatusEffect effect in StatusEffects)
            {
                foreach (AttributeModifier modifier in effect.GetAttributeModifiers())
                {
                    Attributes.Modify(modifier);
                }
            }

            for (int i = 0; i < (int)ResourceType.NUM; ++i)
            {
                ResourceType resource = (ResourceType)i;
                Resources[resource].SetMax(AttributeUtil.ResourceFromAttribtues(resource, Attributes));
            }
        }

        private void AddStatusEffect(StatusEffect effect)
        {
            StatusEffect existingEffect = StatusEffects.Find(
                (x) => x.CreatedBy == effect.CreatedBy && x.EffectType == effect.EffectType);

            if (existingEffect != null)
            {
                existingEffect.StackEffect(effect.StackCount);
            }
            else
            {
                StatusEffects.Add(effect);
            }
        }

        private void RemoveStatusEffect(StatusEffect effect)
        {
            StatusEffect existingEffect = StatusEffects.Find(
                (x) => x.CreatedBy == effect.CreatedBy && x.EffectType == effect.EffectType);

            if (existingEffect != null)
            {
                bool effectExpired = existingEffect.UnStackEffect(effect.StackCount);
                if (effectExpired)
                {
                    StatusEffects.Remove(effect);
                }
            }
            else
            {
                Debug.Assert(false);
            }
        }
    }
}
