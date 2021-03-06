﻿using MAGE.GameModes.Combat;
using MAGE.GameSystems;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Actions
{
    class ActionProposal
    {
        public CombatEntity Proposer;
        public Target Target;
        public ActionComposerBase Action;

        public ActionProposal(CombatEntity proposer, Target target, ActionComposerBase action)
        {
            Action = action;
            Proposer = proposer;
            Target = target;
        }
    }
}
