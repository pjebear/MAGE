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
    class AttackComposer : ActionComposerBase
    {
        public AttackComposer(CombatEntity owner) : base (owner)
        {
        }

        protected override ActionInfo PopulateActionInfo()
        {
            ActionInfo actionInfo = new ActionInfo();

            WeaponEquippable weapon = Owner.GetComponent<EquipmentControl>().Equipment[Equipment.Slot.RightHand] as WeaponEquippable;

            actionInfo.ActionId = ActionId.WeaponAttack;
            actionInfo.ActionCost = new StateChange(StateChangeType.ActionCost, 0, 0);
            actionInfo.ActionRange = weapon.ProjectileInfo.ProjectileId != ProjectileId.INVALID ? ActionRange.Projectile : ActionRange.Meele;
            actionInfo.ActionSource = ActionSource.Weapon;
            actionInfo.AnimationInfo = weapon.AnimationInfo;
            actionInfo.ProjectileInfo = weapon.ProjectileInfo;
            actionInfo.CastRange = weapon.Range;

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
            CompositionNode composition = new AnimationComposer()
            {
                // AnimationConstructor
                ToAnimate = new MonoConversion<CombatEntity, ActorAnimator>(Owner)
                ,
                AnimationTarget = new DeferredTargetPosition(Target)
                ,
                AnimationInfo = new ConcreteVar<AnimationInfo>(AnimationFactory.CheckoutAnimation(ActionInfo.AnimationInfo.AnimationId))
            };

            if (ActionInfo.ActionRange == ActionRange.Projectile)
            {
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
            }
            else
            {
                composition.ChildComposers = new List<CompositionLink<CompositionNode>>()
                {
                    new CompositionLink<CompositionNode>(AllignmentPosition.Interaction, AllignmentPosition.Interaction,
                        new InteractionTargetComposer()
                        {
                            Target = Target,
                            Caster = DeferredOwner,
                            InteractionSolver = mInteractionSolver
                        }
                    )
                };
            }

            return composition;
        }

        public override ActionComposition Compose(Target target)
        {
            return base.Compose(target);
        }
    }
}
