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
    class PoisonStrikeComposer : AOESpellComposerBase
    {
        public PoisonStrikeComposer(CombatEntity owner) : base (owner)
        {
            
        }

        protected override ActionInfo PopulateActionInfo()
        {
            ActionInfo actionInfo = new ActionInfo();

            actionInfo.Effectiveness = 0;
            actionInfo.ActionId = ActionId.PoisonStrike;
            actionInfo.AnimationInfo.AnimationId = AnimationId.DaggerStrike;
            actionInfo.ActionCost = new StateChange(StateChangeType.ActionCost, 0, -5);
            actionInfo.ActionRange = ActionRange.Meele;
            actionInfo.ActionSource = ActionSource.Weapon;
            actionInfo.CastSpeed = CastSpeed.Instant;
            actionInfo.EffectInfo.EffectId = EffectType.INVALID;

            actionInfo.CastRange = new RangeInfo()
            {
                AreaType = AreaType.Circle,
                MaxRange = 3,
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
            float damage = ActionInfo.Effectiveness + Owner.GetComponent<StatsControl>().Attributes[PrimaryStat.Finese] / 2f;

            List<StatusEffect> statusEffects = new List<StatusEffect>()
            {
                StatusEffectFactory.CheckoutStatusEffect(StatusEffectId.Poison)
            };

            return new ConcreteVar<StateChange>( new StateChange(StateChangeType.ActionTarget, -(int)damage, 0, statusEffects));
        }
    }
}
