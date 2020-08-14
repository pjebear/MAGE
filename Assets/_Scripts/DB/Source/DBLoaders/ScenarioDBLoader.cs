using MAGE.GameServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.DB.Internal
{
    class ScenarioDBLoader
    {
        public static void LoadDB()
        {
            { // TheGreatHoldUp
                ScenarioId scenarioId = ScenarioId.TheGreatHoldUp;

                DBScenarioInfo scenarioInfo = new DBScenarioInfo();
                scenarioInfo.Id = (int)scenarioId;
                scenarioInfo.Name = scenarioId.ToString();
                scenarioInfo.IsActive = false;

                DBService.Get().WriteScenarioInfo((int)scenarioId, scenarioInfo);
            }

            DBService.Get().UpdateScenarioDB();
        }
    }
}



