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
    class SummonCompanionComposer : AOESpellComposerBase
    {
        public SummonCompanionComposer(CombatEntity owner) : base(owner)
        {
        }

        protected override IDeferredVar<StateChange> GetStateChange()
        {
            return new ConcreteVar<StateChange>(StateChange.Empty);
        }

        protected override ActionInfo PopulateActionInfo()
        {
            ActionInfo actionInfo = new ActionInfo();

            actionInfo.ActionId = ActionId.SummonBear;
            actionInfo.AnimationInfo.AnimationId = GameSystems.AnimationId.Cast;
            actionInfo.ActionCost = new StateChange(StateChangeType.ActionCost, 0, -5);
            actionInfo.ActionRange = ActionRange.Projectile;
            actionInfo.ActionSource = ActionSource.Cast;
            actionInfo.CastSpeed = CastSpeed.Instant;
            actionInfo.CanGroundTarget = true;

            actionInfo.EffectInfo.EffectId = EffectType.Protect;

            actionInfo.SummonInfo.SummonType = SummonType.Bear;
            actionInfo.SummonInfo.MaxSummonCount = 1;

            actionInfo.CastRange = new RangeInfo()
            {
                AreaType = AreaType.Circle,
                MaxRange = 5,
            };

            actionInfo.EffectRange = new RangeInfo()
            {
                AreaType = AreaType.Point,
                TargetingType = TargetingType.Empty
            };

            return actionInfo;
        }

        protected override TargetingSolverBase PopulateTargetingSolver()
        {
            return new EmptySpaceSolver();
        }

        protected override CompositionNode GetPerTargetComposition()
        {
            return new SpawnComposer()
            {
                SpawnPoint = new DeferredTargetPosition(mTargetingSolver)
                , Owner = Owner.GetComponent<SummonHeirarchy>()
                , SummonInfo = ActionInfo.SummonInfo
            };
        }
    }
}
