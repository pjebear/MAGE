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
        public ConcreteVar<CombatTarget> Targeting = new ConcreteVar<CombatTarget>();
        public ConcreteVar<CombatEntity> Caster = new ConcreteVar<CombatEntity>();
        public WeaponStateChangeCalculator StateChange = new WeaponStateChangeCalculator();
        public WeaponInteractionSolver InteractionSolver = new WeaponInteractionSolver();
    
        private CompositionNode mRootComposition = null;

        public AttackComposer(CombatEntity owner) : base (owner)
        {
            Caster.Set(owner);

            StateChange.DeferredCombatEntity = Caster;
            InteractionSolver.Attacker = Caster;
            InteractionSolver.Target = Targeting;
            InteractionSolver.StateChange = StateChange;

            mRootComposition = new AnimationComposer()
            {
                // AnimationConstructor
                ToAnimate = new MonoConversion<CombatEntity, ActorAnimator>(Caster)
                , AnimationTarget = Targeting
                , AnimationInfo = new ConcreteVar<AnimationInfo>(AnimationFactory.CheckoutAnimation(ActionInfo.AnimationInfo.AnimationId))
            };

            if (ActionInfo.ActionRange == ActionRange.Projectile)
            {
                mRootComposition.ChildComposers.Add(new CompositionLink<CompositionNode>(AllignmentPosition.Interaction, AllignmentPosition.Start,
                    new ProjectileSpawnComposer()
                    {
                        Caster = new DeferredTransform<CombatEntity>(Caster),
                        Target = new DeferredTransform<CombatTarget>(Targeting),
                        ProjectileType = ActionInfo.ProjectileInfo.ProjectileId,

                        ChildComposers = new List<CompositionLink<CompositionNode>>()
                        {
                            new CompositionLink<CompositionNode>(AllignmentPosition.Interaction, AllignmentPosition.Interaction,
                                new InteractionTargetComposer()
                                {
                                    Target = Targeting,
                                    Caster = Caster,
                                    InteractionResult = InteractionSolver.InteractionResult
                                }
                            )
                        }
                    }));
            }
            else
            {
                mRootComposition.ChildComposers = new List<CompositionLink<CompositionNode>>()
                {
                    new CompositionLink<CompositionNode>(AllignmentPosition.Interaction, AllignmentPosition.Interaction,
                        new InteractionTargetComposer()
                        {
                            Target = Targeting,
                            Caster = Caster,
                            InteractionResult = InteractionSolver.InteractionResult
                        }
                    )
                };
            }
        }

        protected override ActionInfo PopulateActionInfo()
        {
            ActionInfo actionInfo = new ActionInfo();

            WeaponEquippable weapon = Owner.GetComponent<EquipmentControl>().Equipment[Equipment.Slot.RightHand] as WeaponEquippable;

            actionInfo.ActionCost = new StateChange(StateChangeType.ActionCost, 0, 0);
            actionInfo.ActionRange = weapon.ProjectileInfo.ProjectileId != ProjectileId.INVALID ? ActionRange.Projectile : ActionRange.Meele;
            actionInfo.ActionSource = ActionSource.Weapon;
            actionInfo.AnimationInfo = weapon.AnimationInfo;
            actionInfo.ProjectileInfo = weapon.ProjectileInfo;
            actionInfo.CastRange = weapon.Range;

            return actionInfo;
        }

        public override ActionComposition Compose(Target target)
        {
            Targeting.Set(target.FocalTarget);
            InteractionSolver.Solve();

            ActionComposition actionComposition = new ActionComposition();
            actionComposition.ActionTimeline = mRootComposition.Compose();
            actionComposition.ActionResults = new GameModes.Combat.ActionResult(
                Owner, 
                ActionInfo, 
                new InteractionResult(InteractionUtil.GetOwnerResultTypeFromResults(InteractionSolver.Results.Values.ToList()), ActionInfo.ActionCost),
                InteractionSolver.Results);

            return actionComposition;
        }
    }
}
