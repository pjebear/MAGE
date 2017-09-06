using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common.AttributeEnums;
using Common.StatusEnums;
using Common.AttributeTypes;
using Common.ActionEnums;
using Common.ActionTypes;
using Common.EquipmentTypes;

namespace EncounterSystem.Character.Managers
{
    using Action;
    using MapEnums;

    class OffenseManager
    {
        public bool IsChargingAction = false;
        public bool HasActed = false;
        private AttributeContainer mEncounterAttributes;

        private Dictionary<ActionContainerCategory, List<ActionBase>> mActionMap;

        public OffenseManager()
        {
            mActionMap = new Dictionary<ActionContainerCategory, List<ActionBase>>();
        }

        public void Initialize(ActionContainer characterActions, AttributeContainer encounterAttributes, List<WeaponBase> weapons)
        {
            ActionFactory factory = ActionFactory.Instance;
            foreach (var actionList in characterActions.ActionMap)
            { 
                List<ActionBase> actions = new List<ActionBase>();
                if (actionList.Key == ActionContainerCategory.Attack)
                {
                    actions.Add(factory.CheckoutAttackAction(weapons));
                }
                else
                {
                    foreach (ActionIndex actionIndex in actionList.Value)
                    {
                        ActionBase action = null;

                        action = factory.CheckoutAction(actionIndex);
                        if (characterActions.ActionModifierMap.ContainsKey(actionIndex))
                        {
                            action.ActionInfo.Modifiers.AddRange(characterActions.ActionModifierMap[actionIndex]);
                        }
                        if (characterActions.ActionStatusMap.ContainsKey(actionIndex))
                        {
                            action.ActionInfo.StatusEffects.AddRange(characterActions.ActionStatusMap[actionIndex]);
                        }
                        actions.Add(action);
                    }
                }
                
                mActionMap.Add(actionList.Key, actions);
            }
            
            mEncounterAttributes = encounterAttributes;
        }

        public ActionBase GetCharacterAction(CharacterActionIndex index)
        {
            return mActionMap[index.ActionType].Find((ActionBase action) => action.ActionInfo.ActionIndex == index.ActionIndex);
        }

        public Dictionary<ActionContainerCategory, List<ScreenActionPayload>> GetScreenActionPayloads(CharacterManager manager)
        {
            Dictionary<ActionContainerCategory, List<ScreenActionPayload>> screenActionPayloads = new Dictionary<ActionContainerCategory, List<ScreenActionPayload>>();
            bool disarmed = mEncounterAttributes[AttributeType.Status][(int)StatusType.Disarmed] > 0;
            foreach (var actionList in mActionMap)
            {
                List<ScreenActionPayload> payloads = new List<ScreenActionPayload>();

                foreach (ActionBase action in actionList.Value)
                {
                    //TODO: Get whether an action can be used
                    ResourceChange cost = GetActionResourceCost(action);

                    bool canUse = CanPerformAction(action) && HasPrerequisites(action, manager);

                    payloads.Add(new ScreenActionPayload(
                        action.name,
                        new CharacterActionIndex(actionList.Key, action.ActionInfo.ActionIndex),
                        cost,
                        canUse,
                        action.MapInteractionInfo.TargetSelectionType == TargetSelectionType.Auto
                        ));
                }
                screenActionPayloads.Add(actionList.Key, payloads);
            }
            return screenActionPayloads;
        }

        public Dictionary<ActionRootEffect, List<ActionBase>> GetAIActionPayloads(CharacterManager manager)
        {
            Dictionary<ActionRootEffect, List<ActionBase>> aiActionPayloads = new Dictionary<ActionRootEffect, List<ActionBase>>();
            aiActionPayloads.Add(ActionRootEffect.Beneficial, new List<ActionBase>());
            aiActionPayloads.Add(ActionRootEffect.Harmful, new List<ActionBase>());
            bool disarmed = mEncounterAttributes[AttributeType.Status][(int)StatusType.Disarmed] > 0;
            foreach (var actionList in mActionMap)
            {
                foreach (ActionBase action in actionList.Value)
                {
                    bool canUse = CanPerformAction(action) && HasPrerequisites(action, manager);
                    if (canUse)
                    {
                        aiActionPayloads[action.ActionInfo.RootEffect].Add(action);
                    }
                }
            }
            return aiActionPayloads;
        }

        public ActionResourceChangeInformation GetModifiedActionStrength(ActionResourceChangeInformation baseActionStrength, List<ActionModifier> modifiers)
        {

            modifiers.Sort(delegate (ActionModifier x, ActionModifier y)
            {
                return x.ModifierType.CompareTo(y.ModifierType);
            });

            foreach (var modifier in modifiers)
            {
                float extra = modifier.GetModifierValue(mEncounterAttributes);

                if (modifier.ModifierType == ModifierType.Additive)
                {
                    baseActionStrength.ResourceChange.Value += extra;
                }
                else if (modifier.ModifierType == ModifierType.Multiplicative)
                {
                    baseActionStrength.ResourceChange.Value *= extra;
                }
                    
            }

            switch (baseActionStrength.ActionBaseType)
            {
                case (ActionBaseType.Physical):
                    baseActionStrength.CriticalChance += mEncounterAttributes[AttributeType.Stat][(int)TertiaryStat.PhysicalCritChance];
                    baseActionStrength.ResourceChange.Value *= 1 + mEncounterAttributes[AttributeType.Stat][(int)TertiaryStat.PhysicalMultiplier];
                    break;
                case (ActionBaseType.Magic):
                    baseActionStrength.CriticalChance += mEncounterAttributes[AttributeType.Stat][(int)TertiaryStat.MagicalCritChance];
                    baseActionStrength.ResourceChange.Value *= 1 + mEncounterAttributes[AttributeType.Stat][(int)TertiaryStat.MagicalMultiplier];
                    break;
            }

            if (!baseActionStrength.IsBeneficial)
            {
                baseActionStrength.ResourceChange.Value *= -1;
            }

            return baseActionStrength;
        }

        public ResourceChange GetActionResourceCost(ActionBase action)
        {
            ResourceChange baseCost = action.ActionInfo.ActionCost;
            if (baseCost.Resource == Resource.INVALID)
                return baseCost;
            float newCost;
            switch (baseCost.Resource)
            {
                case (Resource.Mana):
                    newCost = baseCost.Value * (1 - mEncounterAttributes[AttributeType.Stat][(int)SecondaryStat.Attunement] / 200f);
                    break;
                case (Resource.Endurance):
                    newCost = baseCost.Value;
                    break;
                case (Resource.Health):
                    newCost = baseCost.Value;
                    break;
                default:
                    UnityEngine.Debug.LogError("Action requires Max Resource to use");
                    newCost = baseCost.Value;
                    break;
            }
            // Action costs are provided as negative values
            if (newCost > 0)
            {
                newCost = 0f;
            }

            return new ResourceChange(baseCost.Resource, newCost);
        }

        public bool CanPerformAction(ActionBase action)
        {
            ResourceChange cost = GetActionResourceCost(action);
            bool enoughResources = cost.Resource == Resource.INVALID ? true : mEncounterAttributes[AttributeType.Resource][(int)cost.Resource] >= UnityEngine.Mathf.Abs(cost.Value);


            return
                (cost.Resource == Resource.Mana ?
                mEncounterAttributes[AttributeType.Status][(int)StatusType.Silenced] == 0
                : mEncounterAttributes[AttributeType.Status][(int)StatusType.Disarmed] == 0)

                && mEncounterAttributes[AttributeType.Status][(int)StatusType.Interupt] == 0
                && enoughResources;
        }

        public bool HasPrerequisites(ActionBase action, CharacterManager manager)
        {
            bool hasPrereqs = true;
            var prereqList = action.ActionInfo.Prerequisites;
            foreach (var prereq in prereqList)
            {
                hasPrereqs &= prereq.HasPrerequisite(manager);
            }
            return hasPrereqs;
        }
    }
}
