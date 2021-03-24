using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes.Encounter
{
    interface IActionTimelineComposer
    {
        ActionEventBlock Compose();
    }
}
