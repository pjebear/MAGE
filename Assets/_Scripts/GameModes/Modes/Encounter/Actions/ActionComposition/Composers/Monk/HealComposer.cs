using MAGE.GameModes.Combat;
using MAGE.GameModes.SceneElements;
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
    class HealComposer : AOESpellComposerBase
    {
        public HealComposer(CombatEntity owner) : base (owner)
        {
            
        }

        protected override ActionInfo PopulateActionInfo()
        {
            ActionInfo actionInfo = new ActionInfo();

            actionInfo.ActionId = ActionId.Heal;
            actionInfo.AnimationInfo.AnimationId = GameSystems.AnimationId.Cast;
            actionInfo.ActionCost = new StateChange(StateChangeType.ActionCost, 0, -10);
            actionInfo.ActionRange = ActionRange.Projectile;
            actionInfo.ActionSource = ActionSource.Cast;
            actionInfo.EffectInfo.EffectId = EffectType.AOE_Heal;
            actionInfo.CanGroundTarget = true;

            actionInfo.CastRange = new RangeInfo()
            {
                AreaType = AreaType.Circle,
                MaxRange = 5,
                TargetingType = TargetingType.Allies
            };

            actionInfo.EffectRange = new RangeInfo()
            {
                AreaType = AreaType.Circle,
                MaxRange = 3,
                TargetingType = TargetingType.Allies
            };

            return actionInfo;
        }

        protected override IDeferredVar<StateChange> GetStateChange()
        {
            SpellEffectivenessCalculator calculator = new SpellEffectivenessCalculator();
            calculator.BaseEffectiveness = ActionInfo.Effectiveness;
            calculator.DeferredCaster = new DeferredMonoConversion<CombatEntity, StatsControl>(DeferredOwner);
            calculator.IsBeneficial = true;

            DeferredStateChange deferredStateChange = new DeferredStateChange();
            deferredStateChange.HealthChange = calculator;
            return deferredStateChange;
        }
    }
}
