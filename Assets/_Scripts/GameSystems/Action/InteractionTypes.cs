using MAGE.GameServices.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameServices
{
    enum RelativeOrientation
    {
        Front,
        Left,
        Right,
        Behind
    }

    enum Orientation
    {
        Forward,
        Right,
        Left,
        Back
    }

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
