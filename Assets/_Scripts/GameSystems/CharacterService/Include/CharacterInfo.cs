using MAGE.GameSystems.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Characters
{
    class CharacterInfo
    {
        public int Id;
        public string Name;
        public int Experience;
        public int Level;
        public Dictionary<SpecializationType, SpecializationProgress> SpecializationsProgress = new Dictionary<SpecializationType, SpecializationProgress>();
        public SpecializationProgress CurrentSpecializationProgress { get { return SpecializationsProgress[CurrentSpecializationType]; } }
        public SpecializationType CurrentSpecializationType;
        public Attributes Attributes = null;
        public int AppearanceId = -1;
        public EquippableId[] EquippedItems = Enumerable.Repeat(EquippableId.INVALID, (int)Equipment.Slot.NUM).ToArray();
    }

    class Character
    {
        private string TAG = "Character";
        // Accessors
        public bool IsAlive { get { return mCurrentResources[ResourceType.Health].Ratio > 0; } }
        public bool IsClockGuageFull { get { return mCurrentResources[ResourceType.Clock].Current >= CharacterConstants.CLOCK_GUAGE_THRESHOLD; } }
        public bool HasActed { get { return BaseAttributes[StatusType.ActionsAvailable] == 0; } }
        public bool CanAct { get { return BaseAttributes[StatusType.ActionsAvailable] > 0; } }
        public bool HasMoved { get { return BaseAttributes[StatusType.MovesAvailable] == 0; } }
        public bool CanMove { get { return BaseAttributes[StatusType.MovesAvailable] > 0; } }

        public TeamSide TeamSide;
        public int Id;
        public string Name;
        public int Experience;
        public int Level;
        public Dictionary<SpecializationType, Specialization> Specializations = new Dictionary<SpecializationType, Specialization>();
        public SpecializationType CurrentSpecializationType;
        public Specialization CurrentSpecialization { get { return Specializations[CurrentSpecializationType]; } }

        protected List<ActionResponderBase> ActionResponders = new List<ActionResponderBase>();
        public Equipment Equipment = new Equipment();
        public List<StatusEffect> StatusEffects = new List<StatusEffect>();

        protected Attributes BaseAttributes = null;
        protected Attributes mCurrentAttributes = null;
        public Attributes CurrentAttributes { get { return mCurrentAttributes; } }
        protected Resources mCurrentResources = null;
        public Resources CurrentResources { get { return mCurrentResources; } }

        protected Appearance mBaseAppearance = new Appearance();


        public CharacterInfo GetInfo()
        {
            CharacterInfo characterInfo = new CharacterInfo();

            characterInfo.Id = Id;
            characterInfo.Name = Name;
            characterInfo.Level = Level;
            characterInfo.Experience = Experience;

            // Specializations
            characterInfo.CurrentSpecializationType = CurrentSpecializationType;
            foreach (var specializationPair in Specializations)
            {
                characterInfo.SpecializationsProgress.Add(specializationPair.Key, specializationPair.Value.GetProgress());
            }

            // Appearance Overrides
            characterInfo.AppearanceId = mBaseAppearance.AppearanceId;

            // Equipment
            for (int i = 0; i < (int)Equipment.Slot.NUM; ++i)
            {
                characterInfo.EquippedItems[i] = EquipmentUtil.IsSlotEmpty(Equipment, (Equipment.Slot)i) ? EquippableId.INVALID : Equipment[(Equipment.Slot)i].EquipmentId;
            }

            characterInfo.Attributes = new Attributes(BaseAttributes);

            // TEMP
            characterInfo.Attributes.Clear(AttributeCategory.Status);

            return characterInfo;
        }

        // Members
        // Public
        public Character(CharacterInfo characterInfo)
        {
            Id = characterInfo.Id;
            Name = characterInfo.Name;
            Experience = characterInfo.Experience;
            Level = characterInfo.Level;

            // Specializations
            CurrentSpecializationType = characterInfo.CurrentSpecializationType;
            foreach (var specializationPair in characterInfo.SpecializationsProgress)
            {
                Specializations.Add(specializationPair.Key,
                    Internal.SpecializationFactory.CheckoutSpecialization(specializationPair.Key, specializationPair.Value));
            }
            

            // Appearance Overrides
            if (characterInfo.AppearanceId != -1)
            {
                mBaseAppearance = AppearanceUtil.FromDB(DBService.Get().LoadAppearance(characterInfo.AppearanceId));
            }
            
            // Assorted things
            foreach (ActionResponseId actionResponseId in GetActionResponseIds())
            {
                ActionResponders.Add(ActionResponderFactory.CheckoutResponder(actionResponseId));
            }

            // Attributes
            BaseAttributes = new Attributes(characterInfo.Attributes);

            // Now that everything is set, update attributes/resources to reflect all the badass shit this character has going on
            UpdateCurrentAttributes();
            mCurrentResources = new Resources(
               AttributeUtil.ResourceFromAttribtues(ResourceType.Health, mCurrentAttributes),
               AttributeUtil.ResourceFromAttribtues(ResourceType.Mana, mCurrentAttributes),
               AttributeUtil.ResourceFromAttribtues(ResourceType.Endurance, mCurrentAttributes),
               AttributeUtil.ResourceFromAttribtues(ResourceType.Clock, mCurrentAttributes));
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
                        ApplyStatusEffect(effect);
                    }
                }

                OnCharacterStateChanged();
            }

            mCurrentResources[ResourceType.Health].Modify(stateChange.healthChange);
            mCurrentResources[ResourceType.Mana].Modify(stateChange.resourceChange);
            mCurrentResources[ResourceType.Endurance].Modify(stateChange.resourceChange);
        }

        //  ------------------------------------------------------------------------------
        public AuraInfo GetAuraInfo(AuraType auraType)
        {
            AuraInfo info = AuraFactory.CheckoutAuraInfo(auraType);

            // todo: modify

            return info;
        }

        public List<AuraType> GetAuras()
        {
            List<AuraType> auras = new List<AuraType>();

            auras.AddRange(CurrentSpecialization.GetAuras());

            return auras;
        }

        #region Actions
        public ActionResponseInfo GetActionResponseInfo(ActionResponseId responseId)
        {
            ActionResponseInfo responseInfo = ActionFactory.CreateActionResponseInfoFromId(responseId);
            
            // todo: modify

            return responseInfo;
        }

        //  ------------------------------------------------------------------------------
        public List<ActionResponseBase> RespondToAction(ActionResult actionResult, Map map)
        {
            List<ActionResponseBase> responses = new List<ActionResponseBase>();

            if (IsAlive)
            {
                foreach (ActionResponderBase responder in ActionResponders)
                {
                    responses.AddRange(responder.RespondToActionResult(this, actionResult, map));
                }
            }

            return responses;
        }

        //  ------------------------------------------------------------------------------
        public ActionInfo GetActionInfo(ActionId actionId)
        {
            ActionInfo actionInfo = ActionFactory.CreateActionInfoFromId(actionId, this);

            foreach (IActionModifier modifier in GetActionModifiers().Where(x => x.ActionId == actionId))
            {
                modifier.Modify(actionInfo);
            }

            return actionInfo;
        }

        public List<ActionId> GetActionIds()
        {
            List<ActionId> actions = new List<ActionId>();

            actions.Add(ActionId.WeaponAttack);
            actions.AddRange(CurrentSpecialization.GetActions());

            return actions;
        }

        //  ------------------------------------------------------------------------------
        protected List<IActionModifier> GetActionModifiers()
        {
            List<IActionModifier> actionModifiers = new List<IActionModifier>();

            actionModifiers.AddRange(CurrentSpecialization.GetActionModifiers());

            return actionModifiers;
        }

        //  ------------------------------------------------------------------------------
        protected List<ActionResponseId> GetActionResponseIds()
        {
            List<ActionResponseId> actionResponseIds = new List<ActionResponseId>();

            actionResponseIds.AddRange(CurrentSpecialization.GetActionResponses());

            return actionResponseIds;
        }
        #endregion //Actions

        #region Equipment
        public List<int> Equip(Equippable equippable, Equipment.Slot inSlot)
        {
            List<int> unequippedItems = new List<int>();

            bool fitsInSlot = EquipmentUtil.FitsInSlot(equippable.EquipmentTag.Category, inSlot);
            bool hasProficiency = EquipmentUtil.HasProficiencyFor(GetProficiencies(), equippable);

            Logger.Assert(fitsInSlot && hasProficiency, LogTag.Character, TAG, string.Format("EquipCharacter() - Item [{0}] doesn't fit in slot [{1}].", equippable.EquipmentId.ToString(), inSlot.ToString()));
            if (fitsInSlot && hasProficiency)
            {
                // Apply modifiers to the equipment itself incase it changes how it is equipped
                foreach (EquippableModifier equippableModifier in GetEquipmentModifiers())
                {
                    equippableModifier.Modify(equippable);
                }

                // unequip slots that new item will now occupy
                if (!EquipmentUtil.IsSlotEmpty(Equipment, inSlot))
                {
                    unequippedItems.Add(UnEquip(inSlot).Value);
                }

                if (inSlot == Equipment.Slot.LeftHand || inSlot == Equipment.Slot.RightHand)
                {
                    int numHands = (equippable as HeldEquippable).NumHandsRequired;
                    if (numHands == 2)
                    {
                        Equipment.Slot otherSlot = inSlot == Equipment.Slot.LeftHand ? Equipment.Slot.RightHand : Equipment.Slot.LeftHand;
                        if (!EquipmentUtil.IsSlotEmpty(Equipment, otherSlot))
                        {
                            unequippedItems.Add(UnEquip(otherSlot).Value);
                        }
                    }
                }

                // Finall, equip the item
                Equipment[inSlot] = equippable;
            }
            else
            {
                unequippedItems.Add((int)equippable.EquipmentId);
            }

            return unequippedItems;
        }

        public Optional<int> UnEquip(Equipment.Slot inSlot)
        {
            Optional<int> unequipped = Optional<int>.Empty;
            if (!EquipmentUtil.IsSlotEmpty(Equipment, inSlot))
            {
                Equippable unEquiped = Equipment[inSlot];

                unequipped = (int)unEquiped.EquipmentId;

                if (EquipmentUtil.IsHeld(inSlot)) // replace with fists
                {
                    Equipment[inSlot] = ItemFactory.LoadEquipable(EquippableId.Fists_0);
                    // Apply modifiers to the equipment itself incase it changes how it is equipped
                    foreach (EquippableModifier equippableModifier in GetEquipmentModifiers())
                    {
                        equippableModifier.Modify(Equipment[inSlot]);
                    }
                }
                else
                {
                    Equipment[inSlot] = Equipment.NO_EQUIPMENT;
                }
            }

            return unequipped;
        }

        public List<ProficiencyType> GetProficiencies()
        {
            return CurrentSpecialization.GetProficiencies();
        }

        public List<EquippableModifier> GetEquipmentModifiers()
        {
            return CurrentSpecialization.GetEquippableModifiers();
        }
        #endregion

        #region StatusEffects
        //  ------------------------------------------------------------------------------
        public void ApplyStatusEffect(StatusEffect effect)
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

            OnCharacterStateChanged();
        }

        //  ------------------------------------------------------------------------------
        public void RemoveStatusEffect(StatusEffect effect)
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

            OnCharacterStateChanged();
        }

        //  ------------------------------------------------------------------------------
        public int GetStackCountForStatus(StatusEffectId statusEffectId, Character statusCreator)
        {
            int stackCount = 0;

            Optional<StatusEffect> optEffect = GetStatusEffect(statusEffectId, statusCreator.Id);
            if (optEffect.HasValue)
            {
                stackCount = optEffect.Value.StackCount;
            }

            return stackCount;
        }

        //  ------------------------------------------------------------------------------
        public List<StatusEffect> GetStatusEffects(StatusEffectId statusEffectId)
        {
            List<StatusEffect> statusEffects = StatusEffects.FindAll(
                x =>
                x.EffectType == statusEffectId
               );

            return statusEffects;
        }

        //  ------------------------------------------------------------------------------
        public Optional<StatusEffect> GetStatusEffect(StatusEffectId statusEffectId, int ownedBy)
        {
            Optional<StatusEffect> optEffect = new Optional<StatusEffect>();

            StatusEffect statusEffect = StatusEffects.Find(
                x =>
                x.EffectType == statusEffectId 
                && ownedBy == x.CreatedBy.Id);

            if (statusEffect != null)
            {
                optEffect = statusEffect;
            }

            return optEffect;
        }

        //  ------------------------------------------------------------------------------
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

        //  ------------------------------------------------------------------------------
        public List<StateChange> GetTurnStartStateChanges()
        {
            List<StateChange> turnStateChanges = new List<StateChange>();

            foreach (StatusEffect effect in StatusEffects)
            {
                turnStateChanges.Add(effect.GetTurnStartStateChange());
            }

            return turnStateChanges;
        }
        #endregion// StatusEffects

        #region StateManagement
        //  ------------------------------------------------------------------------------
        public void IncrementClock()
        {
            int clockIncrement = (int)mCurrentAttributes[TertiaryStat.Speed];
            mCurrentResources[ResourceType.Clock].Modify(clockIncrement);
        }

        //  ------------------------------------------------------------------------------
        public void DecrementClock()
        {
            int clockDecrement = 40;
            if (HasActed && HasMoved)
            {
                clockDecrement = 80;
            }
            else if (HasActed || HasMoved)
            {
                clockDecrement = 60;
            }

            mCurrentResources[ResourceType.Clock].Modify(clockDecrement);
        }

        //  ------------------------------------------------------------------------------
        public void UpdateOnMoved()
        {
            BaseAttributes.Modify(new AttributeModifier(StatusType.MovesAvailable, ModifierType.Increment, -1));
        }

        //  ------------------------------------------------------------------------------
        public void UpdateOnActed()
        {
            BaseAttributes.Modify(new AttributeModifier(StatusType.ActionsAvailable, ModifierType.Increment, -1));
        }

        //  ------------------------------------------------------------------------------
        public void RolloverClock()
        {
            mCurrentResources[ResourceType.Clock].Modify(-CharacterConstants.CLOCK_GUAGE_THRESHOLD);

            if (BaseAttributes[StatusType.ActionsAvailable] < CharacterConstants.DEFAULT_ACTIONS_A_TURN)
            {
                BaseAttributes.Modify(new AttributeModifier(StatusType.ActionsAvailable, ModifierType.Increment, CharacterConstants.DEFAULT_ACTIONS_A_TURN));
            }

            if (BaseAttributes[StatusType.MovesAvailable] < CharacterConstants.DEFAULT_MOVES_A_TURN)
            {
                BaseAttributes.Modify(new AttributeModifier(StatusType.MovesAvailable, ModifierType.Increment, CharacterConstants.DEFAULT_MOVES_A_TURN));
            }
        }
        #endregion //StateManagement
        // Private:
        //  ------------------------------------------------------------------------------
        private void OnCharacterStateChanged()
        {
            UpdateCurrentAttributes();
            UpdateCurrentResources();
        }

        //  ------------------------------------------------------------------------------
        protected void UpdateCurrentAttributes()
        {
            mCurrentAttributes = new Attributes(BaseAttributes);

            foreach (AttributeModifier attributeModifier in CurrentSpecialization.GetAttributeModifiers())
            {
                mCurrentAttributes.Modify(attributeModifier);
            }

            foreach (AttributeModifier attributeModifier in Equipment.GetAttributeModifiers())
            {
                mCurrentAttributes.Modify(attributeModifier);
            }

            foreach (StatusEffect statusEffect in StatusEffects)
            {
                foreach (AttributeModifier attributeModifier in statusEffect.GetAttributeModifiers())
                {
                    mCurrentAttributes.Modify(attributeModifier);
                }
            }
        }

        //  ------------------------------------------------------------------------------
        protected void UpdateCurrentResources()
        {
            for (int i = 0; i < (int)ResourceType.NUM; ++i)
            {
                ResourceType resource = (ResourceType)i;
                mCurrentResources[resource].SetMax(AttributeUtil.ResourceFromAttribtues(resource, mCurrentAttributes));
            }
        }

        public Appearance GetAppearance()
        {
            Appearance appearance = new Appearance();

            appearance.AppearanceId = mBaseAppearance.AppearanceId;
            
            // Portrait
            if (mBaseAppearance.PortraitSpriteId != PortraitSpriteId.INVALID)
            {
                appearance.PortraitSpriteId = mBaseAppearance.PortraitSpriteId;
            }
            else
            {
                appearance.PortraitSpriteId = SpecializationUtil.GetPortraitSpriteIdForSpecialization(CurrentSpecializationType);
            }

            // Update Body Type
            if (mBaseAppearance.BodyType != BodyType.NUM)
            {
                appearance.BodyType = mBaseAppearance.BodyType;
            }
            else
            {
                appearance.BodyType = BodyType.Body_0;
            }

            // Update Equipment
            for (int equipmentSlotIdx = 0; equipmentSlotIdx < (int)Equipment.Slot.NUM; ++equipmentSlotIdx)
            {
                Equipment.Slot slot = (Equipment.Slot)equipmentSlotIdx;
                AppearancePrefabId prefabId = Equipment[slot] == Equipment.NO_EQUIPMENT ? AppearancePrefabId.prefab_none : Equipment[slot].PrefabId;

                switch (slot)
                {
                    case Equipment.Slot.Accessory:      /* empty */ break;
                    case Equipment.Slot.Armor: appearance.ArmorId = prefabId; break;
                    case Equipment.Slot.LeftHand: appearance.LeftHeldId = prefabId; break;
                    case Equipment.Slot.RightHand: appearance.RightHeldId = prefabId; break;
                }
            }

            return appearance;
        }
    }
}



