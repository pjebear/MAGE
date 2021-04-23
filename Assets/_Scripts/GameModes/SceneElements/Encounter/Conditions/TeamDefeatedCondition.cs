using MAGE.GameModes.Combat;
using MAGE.GameModes.Encounter;
using MAGE.GameSystems.Stats;
using MAGE.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes.SceneElements.Encounters
{
    class TeamDefeatedCondition : EncounterCondition
    {
        public TeamSide mTeam;

        public override bool IsConditionMet(EncounterModel model)
        {
            Debug.Assert(model.Teams.ContainsKey(mTeam));

            bool conditionMet = true;
            if (model.Teams.ContainsKey(mTeam))
            {
                conditionMet = model.Teams[mTeam].Where(x => x.GetComponent<ResourcesControl>().IsAlive()).Count() == 0;
            }

            return conditionMet;
        }

        public override string ToString()
        {
            return string.Format("TeamDefeatedCondition: Team[{0}] ", mTeam.ToString());
        }
    }
}
