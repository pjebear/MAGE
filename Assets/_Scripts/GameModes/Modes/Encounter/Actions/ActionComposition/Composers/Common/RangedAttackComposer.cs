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
    class RangedAttackComposer : ActionComposerBase
    {
        public RangedAttackComposer(CombatEntity owner) : base (owner)
        {
        }

        protected override ActionInfo PopulateActionInfo()
        {
            ActionInfo actionInfo = new ActionInfo();

            WeaponEquippable weapon = Owner.GetComponent<EquipmentControl>().Equipment[Equipment.Slot.RangedWeapon] as WeaponEquippable;

            actionInfo.ActionId = ActionId.RangedAttack;
            actionInfo.ActionCost = new StateChange(StateChangeType.ActionCost, 0, 0);
            actionInfo.ActionRange = ActionRange.Projectile;
            actionInfo.ActionSource = ActionSource.Weapon;
            actionInfo.AnimationInfo = weapon.AnimationInfo;
            actionInfo.ProjectileInfo = weapon.ProjectileInfo;
            actionInfo.CastRange = weapon.Range;
            actionInfo.CastSpeed = CastSpeed.Instant;
            actionInfo.EquipmentRequirement.Requirement = ProficiencyType.Ranged;

            return actionInfo;
        }

        protected override InteractionSolverBase CreateInteractionSolver()
        {
            return new WeaponInteractionSolver();
        }

        protected override void InitInteractionSolver(InteractionSolverBase interactionSolverBase)
        {
            DeferredStateChange deferredStateChange = new DeferredStateChange();
            deferredStateChange.HealthChange = new WeaponEffectivenessCalculator(Equipment.Slot.RightHand) { DeferredCombatEntity = DeferredOwner };
            interactionSolverBase.StateChange = deferredStateChange;
        }

        protected override CompositionNode PopulateComposition()
        {
            CompositionNode composition = ComposeOwnerAnimation();

            composition.ChildComposers.Add(new CompositionLink<CompositionNode>(AllignmentPosition.Interaction, AllignmentPosition.Start,
                new ProjectileSpawnComposer()
                {
                    CasterPosition = new PositionConversion<CombatEntity>(Owner),
                    Target = Target,
                    ProjectileType = ActionInfo.ProjectileInfo.ProjectileId,

                    ChildComposers = new List<CompositionLink<CompositionNode>>()
                    {
                        new CompositionLink<CompositionNode>(AllignmentPosition.Interaction, AllignmentPosition.Interaction,
                            new InteractionTargetComposer()
                            {
                                Target = Target,
                                Caster = DeferredOwner,
                                InteractionSolver = mInteractionSolver
                            }
                        )
                    }
                }));

            return composition;
        }

        public override ActionComposition Compose(Target target)
        {
            return base.Compose(target);
        }
    }
}
