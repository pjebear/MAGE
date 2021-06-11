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
    class ShieldBashComposer : AOESpellComposerBase
    {
        public ShieldBashComposer(CombatEntity owner) : base (owner)
        {
            
        }

        protected override ActionInfo PopulateActionInfo()
        {
            ActionInfo actionInfo = new ActionInfo();

            actionInfo.Effectiveness = 5;
            actionInfo.ActionId = ActionId.ShieldBash;
            actionInfo.AnimationInfo.AnimationId = AnimationId.Block;
            actionInfo.ActionCost = new StateChange(StateChangeType.ActionCost, 0, -4);
            actionInfo.ActionRange = ActionRange.Meele;
            actionInfo.ActionSource = ActionSource.Weapon;
            actionInfo.EffectInfo.EffectId = EffectType.INVALID;

            actionInfo.CastRange = new RangeInfo()
            {
                AreaType = AreaType.Circle,
                MaxRange = 2,
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
            float damage = ActionInfo.Effectiveness;

            EquipmentControl equipmentControl = Owner.GetComponent<EquipmentControl>();
            if (equipmentControl != null)
            {
                HeldEquippable heldEquippable = equipmentControl.Equipment[GameSystems.Items.Equipment.Slot.LeftHand] as HeldEquippable;
                if (heldEquippable != null)
                {
                    damage *= 1 + heldEquippable.BlockChance;
                }
            }

            List<StatusEffect> statusEffects = new List<StatusEffect>()
            {
                StatusEffectFactory.CheckoutStatusEffect(StatusEffectId.Daze)
            };

            return new ConcreteVar<StateChange>( new StateChange(StateChangeType.ActionTarget, -(int)damage, 0, statusEffects));
        }
    }
}
