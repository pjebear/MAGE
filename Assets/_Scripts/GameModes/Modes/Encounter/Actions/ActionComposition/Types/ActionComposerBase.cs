using MAGE.GameModes.Encounter;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public ActionComposerBase(CombatEntity owner)
        {
            Owner = owner;
            DeferredOwner = new ConcreteVar<CombatEntity>(owner);
            ActionInfo = PopulateActionInfo();
            mTargetingSolver = PopulateTargetingSolver();
            mInteractionSolver = PopulateInteractionSolver();
            mRootComposition = PopulateComposition();
        }

        public bool CanPerformAction()
        {
            return AreActionRequirementsMet() && AreActionRequirementsMet();
        }

        public bool AreResourceRequirementsMet()
        {
            return Owner.GetComponent<ActionsControl>().HasResourcesForAction(ActionInfo.ActionCost);
        }

        public bool AreActionRequirementsMet()
        {
            bool isBlocked = false;
            if (ActionInfo.ActionSource == ActionSource.Weapon)
            {
                StatsControl statsControl = Owner.GetComponent<StatsControl>();
                isBlocked = statsControl.Attributes[StatusType.Disarmed] == 1;
            }
            else if (ActionInfo.ActionSource == ActionSource.Cast)
            {
                StatsControl statsControl = Owner.GetComponent<StatsControl>();
                isBlocked = statsControl.Attributes[StatusType.Silenced] == 1;
            }

            return !isBlocked;
        }

        protected abstract ActionInfo PopulateActionInfo();
        protected abstract CompositionNode PopulateComposition();
        protected virtual TargetingSolverBase PopulateTargetingSolver() { return new TargetingSolver(); }
        protected abstract InteractionSolverBase PopulateInteractionSolver();
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
                new InteractionResult(InteractionUtil.GetOwnerResultTypeFromResults(mInteractionSolver.Results.Values.ToList()), ActionInfo.ActionCost),
                mInteractionSolver.Results);

            return actionComposition;
        }
    }
}
