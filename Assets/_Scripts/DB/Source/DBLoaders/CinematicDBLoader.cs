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
            { // Intro Cinematic
                CinematicId cinematicId = CinematicId.Demo_IntroCinematic;

                DBCinematicInfo cinematicInfo = new DBCinematicInfo();
                cinematicInfo.Id = (int)cinematicId;
                cinematicInfo.Name = cinematicId.ToString();
                cinematicInfo.IsActive = false;

                DBService.Get().WriteCinematicInfo(cinematicInfo.Id, cinematicInfo);
            }

            { // Town hall cinematic
                CinematicId cinematicId = CinematicId.Demo_TownHallCinematic;

                DBCinematicInfo cinematicInfo = new DBCinematicInfo();
                cinematicInfo.Id = (int)cinematicId;
                cinematicInfo.Name = cinematicId.ToString();
                cinematicInfo.IsActive = false;

                DBService.Get().WriteCinematicInfo(cinematicInfo.Id, cinematicInfo);
            }

            { // City exit cinematic
                CinematicId cinematicId = CinematicId.Demo_CityExit;

                DBCinematicInfo cinematicInfo = new DBCinematicInfo();
                cinematicInfo.Id = (int)cinematicId;
                cinematicInfo.Name = cinematicId.ToString();
                cinematicInfo.IsActive = false;

                DBService.Get().WriteCinematicInfo(cinematicInfo.Id, cinematicInfo);
            }

            DBService.Get().UpdateConversationDB();
        }
    }
}



