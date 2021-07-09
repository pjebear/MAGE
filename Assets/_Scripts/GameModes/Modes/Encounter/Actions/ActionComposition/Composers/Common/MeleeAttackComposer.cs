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
    class MeleeAttackComposer : ActionComposerBase
    {
        Equipment.Slot MainHand = Equipment.Slot.RightHand;
        Equipment.Slot OffHand = Equipment.Slot.LeftHand;

        WeaponInteractionSolver LeftHeldSolver = new WeaponInteractionSolver();
        WeaponInteractionSolver RightHeldSolver = new WeaponInteractionSolver();

        public MeleeAttackComposer(CombatEntity owner) : base (owner)
        {
            // Left
            {
                DeferredStateChange deferredStateChange = new DeferredStateChange();
                deferredStateChange.HealthChange = new WeaponEffectivenessCalculator(Equipment.Slot.LeftHand) { DeferredCombatEntity = DeferredOwner };
                LeftHeldSolver.StateChange = deferredStateChange;
                LeftHeldSolver.Results = mActionResults;
            }
            
            // Right
            {
                DeferredStateChange deferredStateChange = new DeferredStateChange();
                deferredStateChange.HealthChange = new WeaponEffectivenessCalculator(Equipment.Slot.RightHand) { DeferredCombatEntity = DeferredOwner };
                RightHeldSolver.StateChange = deferredStateChange;
                RightHeldSolver.Results = mActionResults;
            }

            
            if (!EquipmentUtil.HasProficiency(Owner.GetComponent<ControllableEntity>().Character.GetProficiencies(), ProficiencyType.DualWeild))
            {
                if ((Owner.GetComponent<EquipmentControl>().Equipment[Equipment.Slot.LeftHand] as WeaponEquippable) != null
                    && (Owner.GetComponent<EquipmentControl>().Equipment[Equipment.Slot.RightHand] as WeaponEquippable) == null)
                {
                    MainHand = Equipment.Slot.LeftHand;
                    OffHand = Equipment.Slot.RightHand;
                }
            }
        }

        protected override ActionInfo PopulateActionInfo()
        {
            ActionInfo actionInfo = new ActionInfo();

            WeaponEquippable weapon = Owner.GetComponent<EquipmentControl>().Equipment[MainHand] as WeaponEquippable;

            actionInfo.ActionId = ActionId.MeleeAttack;
            actionInfo.ActionCost = new StateChange(StateChangeType.ActionCost, 0, 0);
            actionInfo.ActionRange = ActionRange.Meele;
            actionInfo.ActionSource = ActionSource.Weapon;
            actionInfo.AnimationInfo = weapon.AnimationInfo;
            actionInfo.ProjectileInfo = weapon.ProjectileInfo;
            actionInfo.CastRange = weapon.Range;
            actionInfo.CastSpeed = CastSpeed.Instant;

            return actionInfo;
        }

        protected override InteractionSolverBase CreateInteractionSolver()
        {
            return null;
        }

        protected override void InitInteractionSolver(InteractionSolverBase interactionSolverBase)
        {
            // empty
        }

        protected override CompositionNode PopulateComposition()
        {
            

            CompositionNode composition = new AnimationComposer()
            {
                // AnimationConstructor
                ToAnimate = new MonoConversion<CombatEntity, ActorAnimator>(Owner)
                ,
                AnimationTarget = new DeferredTargetPosition(Target)
                ,
                AnimationInfo = new ConcreteVar<AnimationInfo>(GetWeaponAnimationInfoForSlot(MainHand))
            };

            // first attack composition
            CompositionLink<CompositionNode> firstStrikeComposition = new CompositionLink<CompositionNode>(AllignmentPosition.Interaction, AllignmentPosition.Interaction,
                    new InteractionTargetComposer()
                    {
                        Target = Target,
                        Caster = DeferredOwner,
                        InteractionSolver = MainHand == Equipment.Slot.RightHand ? RightHeldSolver : LeftHeldSolver
                    }
                );

            if (Owner.GetComponent<ControllableEntity>() != null
                && EquipmentUtil.HasProficiency(Owner.GetComponent<ControllableEntity>().Character.GetProficiencies(), ProficiencyType.DualWeild))
            {

                // second attack composition
                CompositionNode secondStrikeComposition = new AnimationComposer()
                {
                    // AnimationConstructor
                    ToAnimate = new MonoConversion<CombatEntity, ActorAnimator>(Owner),
                    AnimationTarget = new DeferredTargetPosition(Target),
                    AnimationInfo = new ConcreteVar<AnimationInfo>(GetWeaponAnimationInfoForSlot(OffHand)),
                    ChildComposers = new List<CompositionLink<CompositionNode>>()
                    {
                        new CompositionLink<CompositionNode>(AllignmentPosition.Interaction, AllignmentPosition.Start,
                        new InteractionTargetComposer()
                        {
                            Target = Target,
                            Caster = DeferredOwner,
                            InteractionSolver = OffHand == Equipment.Slot.RightHand ? RightHeldSolver : LeftHeldSolver
                        })
                    }
                };

                firstStrikeComposition.Child.ChildComposers.Add(new CompositionLink<CompositionNode>(AllignmentPosition.End, AllignmentPosition.Start, secondStrikeComposition));
            }

            composition.ChildComposers.Add(firstStrikeComposition);

            return composition;
        }

        public override ActionComposition Compose(Target target)
        {
            return base.Compose(target);
        }
    }
}
