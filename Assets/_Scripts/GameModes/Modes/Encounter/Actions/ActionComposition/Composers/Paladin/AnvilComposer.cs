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
    class AnvilComposer : AOESpellComposerBase
    {
        public AnvilComposer(CombatEntity owner) : base (owner)
        {
            
        }

        protected override ActionInfo PopulateActionInfo()
        {
            ActionInfo actionInfo = new ActionInfo();

            actionInfo.Effectiveness = 10;
            actionInfo.ActionId = ActionId.Anvil;
            actionInfo.AnimationInfo.AnimationId = AnimationId.SwordSwing;
            actionInfo.ActionCost = new StateChange(StateChangeType.ActionCost, 0, -5);
            actionInfo.ActionRange = ActionRange.Projectile;
            actionInfo.ActionSource = ActionSource.Cast;
            actionInfo.CastSpeed = CastSpeed.Fast;
            actionInfo.EffectInfo.EffectId = EffectType.Anvil;
            actionInfo.CanGroundTarget = true;

            actionInfo.CastRange = new RangeInfo()
            {
                AreaType = AreaType.Point
            };

            actionInfo.EffectRange = new RangeInfo()
            {
                AreaType = AreaType.Cone,
                MaxRange = 7,
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
            return deferredStateChange;
        }
    }
}
