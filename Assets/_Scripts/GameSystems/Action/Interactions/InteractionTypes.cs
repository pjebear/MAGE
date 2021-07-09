using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Items;
using MAGE.GameSystems.Stats;
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
        public Equipment.Slot EquipmentInteractedWith = Equipment.Slot.INVALID;
        public StateChange StateChange;

        public InteractionResult(InteractionResultType interactionResultType, StateChange stateChange, Equipment.Slot slotInteractedWith)
        {
            InteractionResultType = interactionResultType;
            StateChange = stateChange;
            EquipmentInteractedWith = slotInteractedWith;
        }
    }
}
