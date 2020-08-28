using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems
{
    static class AppearanceUtil
    {
        // Appearance
        public static Appearance FromDB(DB.DBAppearance dbAppearance)
        {
            Appearance fromDB = new Appearance();

            fromDB.PortraitSpriteId = (PortraitSpriteId)dbAppearance.PortraitSpriteId;
            fromDB.BodyType = (BodyType)dbAppearance.BodyType;
            fromDB.ArmorId = (AppearancePrefabId)dbAppearance.ArmorPrefabId;
            fromDB.LeftHeldId = (AppearancePrefabId)dbAppearance.HeldLeftPrefabId;
            fromDB.RightHeldId = (AppearancePrefabId)dbAppearance.HeldRightPrefabId;

            return fromDB;
        }

        public static DB.DBAppearance ToDB(Appearance appearance)
        {
            DB.DBAppearance toDB = new DB.DBAppearance();

            toDB.PortraitSpriteId = (int)appearance.PortraitSpriteId;
            toDB.BodyType = (int)appearance.BodyType;
            toDB.ArmorPrefabId = (int)appearance.ArmorId;
            toDB.HeldLeftPrefabId = (int)appearance.LeftHeldId;
            toDB.HeldRightPrefabId = (int)appearance.RightHeldId;

            return toDB;
        }
    }
}
