using MAGE.GameSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.DB.Internal
{
    class EncounterDBLoader
    {
        public static void LoadDB()
        {
            { // Training Grounds
                EncounterScenarioId encounterId = EncounterScenarioId.Demo_TrainingGrounds;

                DBEncounterInfo encounterInfo = new DBEncounterInfo();
                encounterInfo.Id = (int)encounterId;
                encounterInfo.Name = encounterId.ToString();
                encounterInfo.IsVisible = true;
                encounterInfo.IsActive = false;

                DBService.Get().WriteEncounterInfo(encounterInfo.Id, encounterInfo);
            }

            { // LotharUnderAttack
                EncounterScenarioId encounterId = EncounterScenarioId.Demo_LotharUnderAttack;

                DBEncounterInfo encounterInfo = new DBEncounterInfo();
                encounterInfo.Id = (int)encounterId;
                encounterInfo.Name = encounterId.ToString();
                encounterInfo.IsVisible = true;
                encounterInfo.IsActive = false;

                DBService.Get().WriteEncounterInfo(encounterInfo.Id, encounterInfo);
            }

            DBService.Get().UpdateEncounterDB();
        }
    }
}



