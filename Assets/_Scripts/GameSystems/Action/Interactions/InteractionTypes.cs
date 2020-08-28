using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Actions
{
    class InteractionResult
    {
        public InteractionResultType InteractionResultType;
        public StateChange StateChange;

        public InteractionResult(InteractionResultType interactionResultType, StateChange stateChange)
        {
            InteractionResultType = interactionResultType;
            StateChange = stateChange;
        }
    }
}
