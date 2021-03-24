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

        public ActionComposerBase(CombatEntity owner)
        {
            Owner = owner;
            ActionInfo = PopulateActionInfo();
        }

        protected abstract ActionInfo PopulateActionInfo();
        public abstract ActionComposition Compose(Target target);
    }
}
