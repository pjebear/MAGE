using MAGE.GameSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.DB.Internal
{
    class CinematicDBLoader
    {
        public static void LoadDB()
        {
            foreach (CinematicId cinematicId in Enum.GetValues(typeof(CinematicId)))
            {
                DBCinematicInfo cinematicInfo = new DBCinematicInfo();
                cinematicInfo.Id = (int)cinematicId;
                cinematicInfo.Name = cinematicId.ToString();
                cinematicInfo.IsActive = false;

                DBService.Get().WriteCinematicInfo(cinematicInfo.Id, cinematicInfo);
            }
        }
    }
}



