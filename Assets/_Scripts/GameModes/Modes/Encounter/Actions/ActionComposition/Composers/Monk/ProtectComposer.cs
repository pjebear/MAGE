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
    class ProtectComposer : AOESpellComposerBase
    {
        public ProtectComposer(CombatEntity owner) : base (owner)
        {
            
        }

        protected override ActionInfo PopulateActionInfo()
        {
            ActionInfo actionInfo = new ActionInfo();

            actionInfo.AnimationInfo.AnimationId = GameSystems.AnimationId.Cast;
            actionInfo.ActionCost = new StateChange(StateChangeType.ActionCost, 0, 0);
            actionInfo.ActionRange = ActionRange.Projectile;
            actionInfo.ActionSource = ActionSource.Cast;

            actionInfo.EffectInfo.EffectId = EffectType.Protect;

            actionInfo.CastRange = new RangeInfo()
            {
                AreaType = AreaType.Circle,
                MaxRange = 5,
                TargetingType = TargetingType.Allies
            };

            actionInfo.EffectRange = new RangeInfo()
            {
                AreaType = AreaType.Point,
                TargetingType = TargetingType.Allies
            };

            return actionInfo;
        }

        protected override IDeferredVar<StateChange> GetStateChange()
        {
            return new ConcreteVar<StateChange>(new StateChange(StateChangeType.ActionTarget, StatusEffectFactory.CheckoutStatusEffect(StatusEffectId.Protection)));
        }
    }
}
