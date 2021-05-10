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
    class FireBallComposer : AOESpellComposerBase
    {
        public ConcreteVar<CombatEntity> Caster = new ConcreteVar<CombatEntity>();
        public ConcreteVar<CombatTarget> Target = new ConcreteVar<CombatTarget>();

        public FireBallComposer(CombatEntity owner) : base(owner)
        {
            Caster.Set(owner);
        }

        protected override ActionInfo PopulateActionInfo()
        {
            ActionInfo actionInfo = new ActionInfo();

            actionInfo.ActionId = ActionId.FireBall;
            actionInfo.ActionCost = new StateChange(StateChangeType.ActionCost, 0, -3);
            actionInfo.ActionRange = ActionRange.Projectile;
            actionInfo.ActionSource = ActionSource.Cast;
            actionInfo.AnimationInfo.AnimationId = AnimationId.Cast;
            actionInfo.ProjectileInfo.ProjectileId = ProjectileId.FireBall;

            actionInfo.CastRange = new RangeInfo()
            {
                AreaType = AreaType.Circle,
                MaxRange = 5,
                TargetingType = TargetingType.Enemies
            };

            actionInfo.EffectRange = new RangeInfo()
            {
                AreaType = AreaType.Point
            };

            return actionInfo;
        }

        protected override IDeferredVar<StateChange> GetStateChange()
        {
            SpellEffectivenessCalculator calculator = new SpellEffectivenessCalculator();
            calculator.BaseEffectiveness = ActionInfo.Effectiveness;
            calculator.DeferredCaster = new MonoConversion<CombatEntity, StatsControl>(Caster);
            calculator.IsBeneficial = false;

            DeferredStateChange deferredStateChange = new DeferredStateChange();
            deferredStateChange.HealthChange = calculator;
            return deferredStateChange;
        }
    }
}
