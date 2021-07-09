using MAGE.GameModes.Combat;
using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Items;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Encounter
{
    class SwipeComposer : AOESpellComposerBase
    {
        public SwipeComposer(CombatEntity owner) : base (owner)
        {
            
        }

        protected override ActionInfo PopulateActionInfo()
        {
            ActionInfo actionInfo = new ActionInfo();

            actionInfo.Effectiveness = 10;

            actionInfo.ActionId = ActionId.Swipe;
            actionInfo.AnimationInfo.AnimationId = AnimationId.Cleave;
            actionInfo.EquipmentRequirement.Requirement = ProficiencyType.OneHands;
            actionInfo.ActionCost = new StateChange(StateChangeType.ActionCost, 0, -3);
            actionInfo.ActionRange = ActionRange.Meele;
            actionInfo.ActionSource = ActionSource.Weapon;
            actionInfo.CastSpeed = CastSpeed.Instant;
            actionInfo.EffectInfo.EffectId = EffectType.INVALID;
            actionInfo.Effectiveness = 5;
            actionInfo.CanGroundTarget = true;

            actionInfo.CastRange = new RangeInfo()
            {
                AreaType = AreaType.Point
            };

            actionInfo.EffectRange = new RangeInfo()
            {
                AreaType = AreaType.Cone,
                MaxRange = 2,
                TargetingType = TargetingType.Enemies
            };

            return actionInfo;
        }

        protected override IDeferredVar<StateChange> GetStateChange()
        {
            DeferredStateChange deferredStateChange = new DeferredStateChange();
            deferredStateChange.HealthChange = new WeaponEffectivenessCalculator(Equipment.Slot.RightHand, ActionInfo.Effectiveness, 0.5f) { DeferredCombatEntity = DeferredOwner };
            deferredStateChange.StatusEffects = new ConcreteVar<List<StatusEffectId>>( new List<StatusEffectId>() { StatusEffectId.Hamstring });

            return deferredStateChange;
        }
    }
}
