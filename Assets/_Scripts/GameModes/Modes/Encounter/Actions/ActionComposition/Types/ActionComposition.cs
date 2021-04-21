using MAGE.GameModes.Encounter;
using MAGE.GameSystems.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes.Combat
{
    class ActionComposition
    {
        public List<TimelineElement> ActionTimeline = new List<TimelineElement>();
        public ActionResult ActionResults = null;
    }
}
