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
        public ConcreteVar<CombatEntity> Caster = new ConcreteVar<CombatEntity>();
        public ConcreteVar<CombatTarget> Target = new ConcreteVar<CombatTarget>();
        
        public AttackComposer(CombatEntity owner) : base (owner)
        {
            Caster.Set(owner);
        }

        protected override ActionInfo PopulateActionInfo()
        {
            ActionInfo actionInfo = new ActionInfo();

            WeaponEquippable weapon = Owner.GetComponent<EquipmentControl>().Equipment[Equipment.Slot.RightHand] as WeaponEquippable;

            actionInfo.ActionId = ActionId.MeeleWeaponAttack;
            actionInfo.ActionCost = new StateChange(StateChangeType.ActionCost, 0, 0);
            actionInfo.ActionRange = weapon.ProjectileInfo.ProjectileId != ProjectileId.INVALID ? ActionRange.Projectile : ActionRange.Meele;
            actionInfo.ActionSource = ActionSource.Weapon;
            actionInfo.AnimationInfo = weapon.AnimationInfo;
            actionInfo.ProjectileInfo = weapon.ProjectileInfo;
            actionInfo.CastRange = weapon.Range;

            return actionInfo;
        }

        protected override InteractionSolverBase PopulateInteractionSolver()
        {
            WeaponInteractionSolver interactionSolver = new WeaponInteractionSolver();
            interactionSolver.Attacker = Caster;
            interactionSolver.Target = Target;
            interactionSolver.StateChange = new WeaponStateChangeCalculator() { DeferredCombatEntity = Caster };

            return interactionSolver;
        }

        protected override CompositionNode PopulateComposition()
        {
            CompositionNode composition = new AnimationComposer()
            {
                // AnimationConstructor
                ToAnimate = new MonoConversion<CombatEntity, ActorAnimator>(Caster)
                ,
                AnimationTarget = Target
                ,
                AnimationInfo = new ConcreteVar<AnimationInfo>(AnimationFactory.CheckoutAnimation(ActionInfo.AnimationInfo.AnimationId))
            };

            if (ActionInfo.ActionRange == ActionRange.Projectile)
            {
                composition.ChildComposers.Add(new CompositionLink<CompositionNode>(AllignmentPosition.Interaction, AllignmentPosition.Start,
                    new ProjectileSpawnComposer()
                    {
                        Caster = new DeferredTransform<CombatEntity>(Caster),
                        Target = new DeferredTransform<CombatTarget>(Target),
                        ProjectileType = ActionInfo.ProjectileInfo.ProjectileId,

                        ChildComposers = new List<CompositionLink<CompositionNode>>()
                        {
                            new CompositionLink<CompositionNode>(AllignmentPosition.Interaction, AllignmentPosition.Interaction,
                                new InteractionTargetComposer()
                                {
                                    Target = Target,
                                    Caster = Caster,
                                    InteractionResult = mInteractionSolver.InteractionResult
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
                            Caster = Caster,
                            InteractionResult = mInteractionSolver.InteractionResult
                        }
                    )
                };
            }

            return composition;
        }

        public override ActionComposition Compose(Target target)
        {
            Target.Set(target.FocalTarget);
            mInteractionSolver.Solve();

            return base.Compose(target);
        }
    }
}
