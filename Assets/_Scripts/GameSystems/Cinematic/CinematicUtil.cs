using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems
{
    static class CinematicUtil
    {
        public static CinematicInfo FromDB(DB.DBCinematicInfo db)
        {
            CinematicInfo info = new CinematicInfo();
            info.CinematicId = (CinematicId)db.Id;
            info.IsActive = db.IsActive;
            return info;
        }

        public static DB.DBCinematicInfo ToDB(CinematicInfo info)
        {
            DB.DBCinematicInfo db = new DB.DBCinematicInfo();
            db.Id = (int)info.CinematicId;
            db.Name = info.CinematicId.ToString();
            db.IsActive = info.IsActive;
            return db;
        }
    }
}
