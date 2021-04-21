using MAGE.GameModes.Encounter;
using MAGE.GameSystems.Actions;
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

        private CompositionNode mRootComposition = null;
        protected InteractionSolverBase mInteractionSolver = null;

        public ActionComposerBase(CombatEntity owner)
        {
            Owner = owner;
            ActionInfo = PopulateActionInfo();
            mInteractionSolver = PopulateInteractionSolver();
            mRootComposition = PopulateComposition();
        }

        protected abstract ActionInfo PopulateActionInfo();
        protected abstract CompositionNode PopulateComposition();
        protected abstract InteractionSolverBase PopulateInteractionSolver();

        public virtual ActionComposition Compose(Target target)
        {
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
