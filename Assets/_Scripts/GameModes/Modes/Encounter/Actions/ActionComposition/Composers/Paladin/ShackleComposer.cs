using MAGE.GameModes.Combat;
using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Encounter
{
    class ShackleComposer : AOESpellComposerBase
    {
        public ShackleComposer(CombatEntity owner) : base(owner)
        {

        }

        protected override ActionInfo PopulateActionInfo()
        {
            ActionInfo actionInfo = new ActionInfo();

            actionInfo.ActionId = ActionId.Shackle;
            actionInfo.AnimationInfo.AnimationId = AnimationId.Cast;
            actionInfo.ActionCost = new StateChange(StateChangeType.ActionCost, 0, -5);
            actionInfo.ActionRange = ActionRange.Projectile;
            actionInfo.ActionSource = ActionSource.Cast;
            actionInfo.EffectInfo.EffectId = EffectType.Smite;
            actionInfo.Effectiveness = .25f;

            actionInfo.CastRange = new RangeInfo()
            {
                AreaType = AreaType.Circle,
                MaxRange = 7
            };

            actionInfo.EffectRange = new RangeInfo()
            {
                AreaType = AreaType.Point,
                TargetingType = TargetingType.Enemies
            };

            return actionInfo;
        }

        protected override IDeferredVar<StateChange> GetStateChange()
        {
            SpellEffectivenessCalculator calculator = new SpellEffectivenessCalculator();
            calculator.BaseEffectiveness = ActionInfo.Effectiveness;
            calculator.DeferredCaster = new DeferredMonoConversion<CombatEntity, StatsControl>(DeferredOwner);
            calculator.IsBeneficial = false;

            DeferredStateChange deferredStateChange = new DeferredStateChange();
            deferredStateChange.HealthChange = calculator;
            deferredStateChange.StatusEffects = new ConcreteVar<List<StatusEffectId>>(new List<StatusEffectId>() { StatusEffectId.Disarm });

            return deferredStateChange;
        }
    }
}
