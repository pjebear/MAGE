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
    class EntanglingRootsComposer : AOESpellComposerBase
    {
        public EntanglingRootsComposer(CombatEntity owner) : base (owner)
        {
            
        }

        protected override ActionInfo PopulateActionInfo()
        {
            ActionInfo actionInfo = new ActionInfo();

            actionInfo.ActionId = ActionId.EntanglingRoots;
            actionInfo.AnimationInfo.AnimationId = GameSystems.AnimationId.Cast;
            actionInfo.ActionCost = new StateChange(StateChangeType.ActionCost, 0, -5);
            actionInfo.ActionRange = ActionRange.Projectile;
            actionInfo.ActionSource = ActionSource.Cast;
            actionInfo.CanGroundTarget = true;
            actionInfo.CastSpeed = CastSpeed.Normal;

            actionInfo.EffectInfo.EffectId = EffectType.Protect;

            actionInfo.CastRange = new RangeInfo()
            {
                AreaType = AreaType.Circle,
                MaxRange = 7
            };

            actionInfo.EffectRange = new RangeInfo()
            {
                AreaType = AreaType.Circle,
                MaxRange = 2,
                TargetingType = TargetingType.Enemies
            };

            return actionInfo;
        }

        protected override IDeferredVar<StateChange> GetStateChange()
        {
            return new ConcreteVar<StateChange>(new StateChange(StateChangeType.ActionTarget, StatusEffectFactory.CheckoutStatusEffect(StatusEffectId.Root)));
        }
    }
}
