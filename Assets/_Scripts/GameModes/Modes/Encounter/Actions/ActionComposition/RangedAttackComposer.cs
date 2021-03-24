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
        public ConcreteVar<CombatTarget> Targeting = new ConcreteVar<CombatTarget>();
        public ConcreteVar<CombatEntity> Caster = new ConcreteVar<CombatEntity>();
        public WeaponStateChangeCalculator StateChange = new WeaponStateChangeCalculator();
        public WeaponInteractionSolver InteractionSolver = new WeaponInteractionSolver();

        //private CompositionHeirarchy mRootComposition = null;

        public RangedAttackComposer(CombatEntity owner) : base (owner)
        {
            StateChange.DeferredCombatEntity = Caster;

            InteractionSolver.Attacker = Caster;
            InteractionSolver.Target = Targeting;
            InteractionSolver.StateChange = StateChange;

            //mRootComposition = new CompositionHeirarchy()
            //{
            //    Composer = new AnimationComposer()
            //    {
            //        ToAnimate = Caster,
            //        AnimationTarget = Targeting,
            //        AnimationInfo = new ConcreteVar<AnimationInfo>(AnimationFactory.CheckoutAnimation(GameSystems.AnimationId.BowDraw)),
            //    }
            //    ,
            //    Children = new List<CompositionHeirarchy>()
            //    {
            //        new CompositionHeirarchy(AllignmentPosition.Interaction, AllignmentPosition.Start)
            //        {
            //            Composer =  new ProjectileSpawnComposer()
            //            {
            //                Caster = Caster,
            //                Target = Targeting,
            //            }
            //            , Children = new List<CompositionHeirarchy>()
            //            {
            //                new CompositionHeirarchy(AllignmentPosition.Interaction, AllignmentPosition.Interaction)
            //                {
            //                    Composer = new AnimationComposer()
            //                    {
            //                        ToAnimate = new MonoConversion<CombatTarget, ActorAnimator>(Targeting),
            //                        AnimationTarget = Caster,
            //                        AnimationInfo = new InteractionResultToAnimation(InteractionSolver.InteractionResult)
            //                    }
            //                }
            //                , new CompositionHeirarchy(AllignmentPosition.Interaction, AllignmentPosition.Interaction)
            //                {
            //                    Composer = new StateChangeComposer()
            //                    {
            //                        Target = Targeting,
            //                        StateChange = InteractionSolver.InteractionResult
            //                    }
            //                }
            //            }
            //        }
            //    }
            //};
        }

        protected override ActionInfo PopulateActionInfo()
        {
            ActionInfo actionInfo = new ActionInfo();

            WeaponEquippable weapon = Owner.GetComponent<EquipmentControl>().Equipment[Equipment.Slot.RightHand] as WeaponEquippable;

            actionInfo.ActionCost = new StateChange(StateChangeType.ActionCost, 0, 0);
            actionInfo.ActionRange = ActionRange.Projectile;
            actionInfo.ActionSource = ActionSource.Weapon;
            actionInfo.AnimationInfo = weapon.AnimationInfo;
            actionInfo.CastRange = weapon.Range;

            return actionInfo;
        }

        public override ActionComposition Compose(Target target)
        {
            Caster.Set(Owner);
            Targeting.Set(target.FocalTarget);
            InteractionSolver.Solve();

            ActionComposition actionComposition = new ActionComposition();
            //actionComposition.ActionProposal = mActionProposal;

            //actionComposition.ActionTimeline = mRootComposition.Compose();
            //actionComposition.ActionResults = InteractionSolver.Results;

            return actionComposition;
        }
    }
}
