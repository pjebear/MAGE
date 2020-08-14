using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.UI
{

    public enum UIInteractionType
    {
        None,

        Click,
        MouseOver,
        MouseExit,

        NUM
    }

    class UIInteractionInfo
    {
        public int ComponentId;
        public UIInteractionType InteractionType;

        public UIInteractionInfo(int componentId, UIInteractionType interactionType)
        {
            ComponentId = componentId;
            InteractionType = interactionType;
        }
    }

    class ListInteractionInfo : UIInteractionInfo
    {
        public int ListIdx;

        public ListInteractionInfo(int componentId, UIInteractionInfo interactionInfo)
            : base(componentId, interactionInfo.InteractionType)
        {
            ListIdx = interactionInfo.ComponentId;
        }
    }

}
