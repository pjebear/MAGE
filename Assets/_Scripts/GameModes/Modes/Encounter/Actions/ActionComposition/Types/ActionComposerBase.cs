using MAGE.GameModes.Encounter;
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

namespace MAGE.GameModes.Combat
{
    abstract class ActionComposerBase
    {
        public ActionInfo ActionInfo = null;
        public CombatEntity Owner;
        public ConcreteVar<CombatEntity> DeferredOwner;
        public ConcreteVar<Target> Target = new ConcreteVar<Target>();

        private CompositionNode mRootComposition = null;
        protected InteractionSolverBase mInteractionSolver = null;
        protected TargetingSolverBase mTargetingSolver = null;
        protected Dictionary<CombatTarget, List<InteractionResult>> mActionResults = new Dictionary<CombatTarget, List<InteractionResult>>();

        public ActionComposerBase(CombatEntity owner)
        {
            Owner = owner;
            DeferredOwner = new ConcreteVar<CombatEntity>(owner);
        }

        public void Init()
        {
            ActionInfo = PopulateActionInfo();
            mTargetingSolver = PopulateTargetingSolver();
            mInteractionSolver = CreateInteractionSolver();
            if (mInteractionSolver != null)
            {
                mInteractionSolver.Results = mActionResults;
                InitInteractionSolver(mInteractionSolver);
            }
            mRootComposition = PopulateComposition();
        }

        public bool AreActionPreRequisitesMet()
        {
            bool arePrerequisitesMet = false;
            if (Owner.GetComponent<ActionsControl>().HasResourcesForAction(ActionInfo.ActionCost))
            {
                if (ActionInfo.ActionSource == ActionSource.Cast)
                {
                    StatsControl statsControl = Owner.GetComponent<StatsControl>();
                    arePrerequisitesMet = statsControl.Attributes[StatusType.Silenced] == 0;
                }
                else if (AreEquipmentRequirementsMet())
                {
                    arePrerequisitesMet = true;
                }
            }
            return arePrerequisitesMet;
        }

        protected bool AreEquipmentRequirementsMet()
        {
            bool requirementsMet = GetWeaponSlotForAction() != Equipment.Slot.INVALID;

            return requirementsMet;
        }

        protected Equipment.Slot GetWeaponSlotForAction()
        {
            Equipment.Slot slot = Equipment.Slot.INVALID;

            EquipmentControl equipmentControl = Owner.GetComponent<EquipmentControl>();
            if (ActionInfo.EquipmentRequirement.Requirement != ProficiencyType.INVALID)
            {
                if (ActionInfo.EquipmentRequirement.SlotRequirement != Equipment.Slot.INVALID)
                {
                    bool hasRequiredEquipmentInSlot = equipmentControl.DoesSlotMatchProficiency(ActionInfo.EquipmentRequirement.SlotRequirement, ActionInfo.EquipmentRequirement.Requirement);
                    Debug.Assert(hasRequiredEquipmentInSlot);
                    if (hasRequiredEquipmentInSlot)
                    {
                        slot = ActionInfo.EquipmentRequirement.SlotRequirement;
                    }
                }
                else 
                {
                    slot = equipmentControl.GetSlotMatchingProficiency(ActionInfo.EquipmentRequirement.Requirement);
                }
            }
            else
            {
                slot = Equipment.Slot.RightHand;
            }

            return slot;
        }

        protected virtual AnimationComposer ComposeOwnerAnimation()
        {
            AnimationInfo animationInfo = null;
            if (ActionInfo.ActionSource == ActionSource.Cast)
            {
                animationInfo = AnimationFactory.CheckoutAnimation(ActionInfo.AnimationInfo.AnimationId, ActionInfo.AnimationInfo.AnimationSide);
            }
            else if (ActionInfo.ActionSource == ActionSource.Weapon)
            {
                animationInfo = GetWeaponAnimationInfoForAction();
            }

            AnimationComposer ownerAnimation = new AnimationComposer()
            {
                // AnimationConstructor
                ToAnimate = new DeferredMonoConversion<CombatEntity, ActorAnimator>(DeferredOwner),
                AnimationTarget = new DeferredTargetPosition(mTargetingSolver),
                AnimationInfo = new ConcreteVar<AnimationInfo>(animationInfo)
            };

            return ownerAnimation;
        }

        protected AnimationInfo GetWeaponAnimationInfoForAction()
        {
            Equipment.Slot slot = GetWeaponSlotForAction();

            if (slot == Equipment.Slot.INVALID)
            {
                slot = Equipment.Slot.RightHand;
            }
            
            return GetWeaponAnimationInfoForSlot(slot);
        }

        protected AnimationInfo GetWeaponAnimationInfoForSlot(Equipment.Slot slot)
        {
            Debug.Assert(slot != Equipment.Slot.INVALID);
            if (slot == Equipment.Slot.INVALID)
            {
                slot = Equipment.Slot.RightHand;
            }

            AnimationSide animationSide = AnimationSide.None;
            if (slot == Equipment.Slot.LeftHand)
            {
                animationSide = AnimationSide.Left;
            }
            else if (slot == Equipment.Slot.RightHand
                || slot == Equipment.Slot.RangedWeapon)
            {
                animationSide = AnimationSide.Right;
            }

            AnimationId animationId = AnimationId.INVALID;
            if (ActionInfo.AnimationInfo.AnimationId != AnimationId.INVALID)
            {
                animationId = ActionInfo.AnimationInfo.AnimationId;
            }
            else
            {
                Equippable equippable = Owner.GetComponent<EquipmentControl>().Equipment[slot];
                WeaponEquippable weaponEquippable = (WeaponEquippable)equippable;
                Debug.Assert(weaponEquippable != null);
                if (weaponEquippable != null)
                {
                    animationId = weaponEquippable.AnimationInfo.AnimationId;
                }
                else
                {
                    animationId = AnimationId.SwordSwing;
                }
            }

            return AnimationFactory.CheckoutAnimation(animationId, animationSide);
        }

        protected abstract ActionInfo PopulateActionInfo();
        protected abstract CompositionNode PopulateComposition();
        protected virtual TargetingSolverBase PopulateTargetingSolver() { return new TargetingSolver(); }
        protected abstract InteractionSolverBase CreateInteractionSolver();
        protected abstract void InitInteractionSolver(InteractionSolverBase interactionSolverBase);
        public virtual ActionComposition Compose(Target target)
        {
            Target.Set(target);

            mTargetingSolver.Targeting = DeferredOwner;
            mTargetingSolver.BeingTargeted = new TargetSelection(target, ActionInfo.EffectRange);
            mTargetingSolver.Solve();

            ActionComposition actionComposition = new ActionComposition();
            actionComposition.ActionTimeline = mRootComposition.Compose();
            actionComposition.ActionResults = new GameModes.Combat.ActionResult(
                Owner,
                ActionInfo,
                new InteractionResult(InteractionUtil.GetOwnerResultTypeFromResults(mActionResults), ActionInfo.ActionCost, Equipment.Slot.INVALID),
                mActionResults);

            return actionComposition;
        }
    }
}
