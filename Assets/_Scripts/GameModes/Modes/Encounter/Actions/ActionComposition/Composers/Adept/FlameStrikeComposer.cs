using MAGE.GameModes.Combat;
using MAGE.GameModes.SceneElements;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Encounter
{

    class FlameStrikeComposer : AOESpellComposerBase
    {
        public FlameStrikeComposer(CombatEntity owner) : base (owner)
        {
            
        }

        protected override ActionInfo PopulateActionInfo()
        {
            ActionInfo actionInfo = new ActionInfo();

            actionInfo.Effectiveness = 12;
            actionInfo.ActionId = ActionId.FlameStrike;
            actionInfo.AnimationInfo.AnimationId = GameSystems.AnimationId.Cast;
            actionInfo.ActionCost = new StateChange(StateChangeType.ActionCost, 0, -5);
            actionInfo.ActionRange = ActionRange.Projectile;
            actionInfo.ActionSource = ActionSource.Cast;
            actionInfo.EffectInfo.EffectId = EffectType.FlameStrike;
            actionInfo.SummonInfo.SummonType = SummonType.ScorchedEarth;
            actionInfo.CanGroundTarget = true;

            actionInfo.CastRange = new RangeInfo()
            {
                AreaType = AreaType.Circle,
                MaxRange = 5,
                TargetingType = TargetingType.Enemies
            };

            actionInfo.EffectRange = new RangeInfo()
            {
                AreaType = AreaType.Circle,
                MaxRange = 2,
                TargetingType = TargetingType.Any
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

        protected override CompositionNode GetFocalPointComposition()
        {
            CompositionNode focalPointComposition = base.GetFocalPointComposition();

            focalPointComposition.ChildComposers.Add(new CompositionLink<CompositionNode>(AllignmentPosition.End, AllignmentPosition.Start,
                new SpawnComposer()
                {
                    Owner = Owner.GetComponent<SummonHeirarchy>()
                     , SpawnPoint = EffectSpawnPoint
                     , SummonInfo = ActionInfo.SummonInfo
                }));

            return focalPointComposition;
        }
    }
}
