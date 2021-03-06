﻿using MAGE.GameModes.Combat;
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
    class SmiteComposer : AOESpellComposerBase
    {
        public SmiteComposer(CombatEntity owner) : base (owner)
        {
            
        }

        protected override ActionInfo PopulateActionInfo()
        {
            ActionInfo actionInfo = new ActionInfo();

            actionInfo.Effectiveness = 13;
            actionInfo.ActionId = ActionId.Smite;
            actionInfo.AnimationInfo.AnimationId = GameSystems.AnimationId.Cast;
            actionInfo.ActionCost = new StateChange(StateChangeType.ActionCost, 0, -10);
            actionInfo.ActionRange = ActionRange.Projectile;
            actionInfo.ActionSource = ActionSource.Cast;
            actionInfo.CastSpeed = CastSpeed.Slow;
            actionInfo.EffectInfo.EffectId = EffectType.Smite;

            actionInfo.CastRange = new RangeInfo()
            {
                AreaType = AreaType.Circle,
                MaxRange = 10,
                TargetingType = TargetingType.Allies
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
            return deferredStateChange;
        }
    }
}
